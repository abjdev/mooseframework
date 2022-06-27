//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using moose.Drivers;
using moose.Memory;

namespace moose.FileSystem
{
    public static unsafe class SATA
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HBA
        {
            public uint HostCapability;
            public uint GlobalHostControl;
            public uint InterruptStatus;
            public uint PortsImplemented;
            public uint Version;
            public uint CCCControl;
            public uint CCCPorts;
            public uint EnclosureManagementLocation;
            public uint EnclosureManagementControl;
            public uint HostCapabilitiesExtended;
            public uint BIOSHandoffControlStatus;
            public fixed byte Reserved0[0x74];
            public fixed byte Vendor[0x60];
            public HBAPort Ports;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HBAPort
        {
            public uint CommandListBase;
            public uint CommandListBaseUpper;
            public uint FISBaseAddress;
            public uint FISBaseAddressUpper;
            public uint InterruptStatus;
            public uint InterruptEnable;
            public uint CommandStatus;
            public uint Reserved0;
            public uint TaskFileData;
            public uint Signature;
            public uint SataStatus;
            public uint SataControl;
            public uint SataError;
            public uint SataActive;
            public uint CommandIssue;
            public uint SataNotification;
            public uint FISSwitchControl;
            public fixed uint Reserved1[11];
            public fixed uint Vendor[4];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct HBACommandHeader
        {
            public byte P1;
            public byte P2;
            public ushort PRDTLength;
            public uint PRDBCount;
            public uint CommandTableBaseAddress;
            public uint CommandTableBaseAddressUpper;
            public fixed uint Reserved1[4];

            public byte CommandFISLength
            {
                get => (byte)(P1 & 0x1F);

                set
                {
                    unchecked
                    { P1 &= (byte)~0x1F; }
                    P1 |= (byte)(value & 0x1F);
                }
            }

            public bool ATAPI => BitHelpers.IsBitSet(P1, 5);

            public bool Write
            {
                get => BitHelpers.IsBitSet(P1, 6);

                set
                {
                    if (value)
                    {
                        P1 |= 1 << 6;
                    } else
                    {
                        unchecked
                        { P1 &= (byte)~(1 << 6); }
                    }
                }
            }

            public bool Prefetchable => BitHelpers.IsBitSet(P1, 7);

            public bool Reset => BitHelpers.IsBitSet(P2, 0);

            public bool BIST => BitHelpers.IsBitSet(P2, 1);

            public bool ClearBusy
            {
                get => BitHelpers.IsBitSet(P2, 2);

                set
                {
                    if (value)
                    {
                        P2 |= 1 << 2;
                    } else
                    {
                        unchecked
                        { P2 &= (byte)~(1 << 2); }
                    }
                }
            }

            public bool Reserved0 => BitHelpers.IsBitSet(P2, 3);

            public byte PortMultiplier => (byte)((P2 & 0xF0) >> 4);
        };

        public enum SATAPortType
        {
            NONE = 0,
            SATA = 1,
            SEMB = 2,
            PM = 3,
            ATAPI = 4
        }

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static HBA* Controller = null;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static List<SATADevice> Ports = null;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static void Initialize()
        {
            Ports = new List<SATADevice>();
            #region FindDevice
            for (int i = 0; i < PCI.Devices.Count; i++)
            {
                if (
                    PCI.Devices[i] != null &&
                    PCI.Devices[i].ClassID == 0x01 &&
                    PCI.Devices[i].SubClassID == 0x06
                    )
                {
                    PCIDevice dev = PCI.Devices[i];

                    if (dev == null)
                    {
                        return;
                    }

                    ushort reg = dev.ReadRegister16(0x04);
                    dev.WriteRegister16(0x04, 0x04 | 0x02 | 0x01);
                    Controller = (HBA*)dev.Bar5;
                    for (int k = 0; k < 32; k++)
                    {
                        if ((Controller->PortsImplemented & (1 << k)) != 0)
                        {
                            SATAPortType type = CheckPortType(&(&Controller->Ports)[k]);
                            if (type == SATAPortType.SATA || type == SATAPortType.ATAPI)
                            {
                                SATADevice sata = new()
                                {
                                    PortType = type,
                                    HBAPort = &(&Controller->Ports)[k]
                                };
                                Ports.Add(sata);
                            }
                        }
                    }
                    if (Ports.Count != 0)
                    {
                        for (int z = 0; z < Ports.Count; z++)
                        {
                            Ports[z].Configure();

                            Console.Write("[SATA] SATA Port ");
                            Console.Write(((ulong)z).ToString());
                            Console.WriteLine(" Configured");
                        }
                    }

                    dev.WriteRegister16(0x04, reg);
                }
            }
            #endregion
        }

        public static SATAPortType CheckPortType(HBAPort* port)
        {
            uint statStat = port->SataStatus;
            byte intpowman = (byte)((statStat >> 8) & 0b111);
            byte devdetect = (byte)(statStat & 0b111);

            //Not Present
            if (devdetect != 0x3)
            {
                return SATAPortType.NONE;
            }
            //Not Active
            return intpowman != 0x1
                ? SATAPortType.NONE
                : port->Signature switch
                {
                    0x00000101 => SATAPortType.SATA,
                    0xEB140101 => SATAPortType.ATAPI,
                    0x96690101 => SATAPortType.PM,
                    0xC33C0101 => SATAPortType.SEMB,
                    _ => SATAPortType.NONE,
                };
        }

        public class SATADevice : Disk
        {
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct FIS_REG_H2D
            {
                public byte FISType;
                private byte PortMultiplier_Reserved0_CommandControl;
                public byte Command;
                public byte FeatureLow;
                public byte LBA0;
                public byte LBA1;
                public byte LBA2;
                public byte DeviceRegister;
                public byte LBA3;
                public byte LBA4;
                public byte LBA5;
                public byte FeatureHigh;
                public byte CountLow;
                public byte CountHigh;
                public byte ISOCommandCompletion;
                public byte Control;
                public fixed byte Reserved1[4];

                public byte PortMultiplier => (byte)(PortMultiplier_Reserved0_CommandControl & 0xF);

                public bool CommandControl
                {
                    get => BitHelpers.IsBitSet(PortMultiplier_Reserved0_CommandControl, 7);

                    set
                    {
                        if (value)
                        {
                            PortMultiplier_Reserved0_CommandControl |= 1 << 7;
                        } else
                        {
                            unchecked
                            { PortMultiplier_Reserved0_CommandControl &= (byte)~(1 << 7); }
                        }
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct HBACommandTable
            {
                public fixed byte CommandFIS[64];
                public fixed byte ATAPICommand[16];
                public fixed byte Reserved[48];
                public HBAPRDTEntry PRDTEntry;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct HBAPRDTEntry
            {
                public uint DataBaseAddress;
                public uint DataBaseAddressUpper;
                public uint Reserved0;
                public uint ByteCount_Reserved1_InterruptOnCompletion;

                public bool InterruptOnCompletion
                {
                    get => BitHelpers.IsBitSet(ByteCount_Reserved1_InterruptOnCompletion, 31);

                    set
                    {
                        if (value)
                        {
                            ByteCount_Reserved1_InterruptOnCompletion |= 1U << 31;
                        } else
                        {
                            unchecked
                            { ByteCount_Reserved1_InterruptOnCompletion &= ~(1U << 31); }
                        }
                    }
                }

                public uint ByteCount
                {
                    get => ByteCount_Reserved1_InterruptOnCompletion & 0x3FFFFF;

                    set
                    {
                        ByteCount_Reserved1_InterruptOnCompletion &= ~0x3FFFFFU;
                        ByteCount_Reserved1_InterruptOnCompletion |= value & 0x3FFFFFU;
                    }
                }
            };

            public SATAPortType PortType;
            public volatile HBAPort* HBAPort;

            public void Configure()
            {
                StopCommand();

                ulong newBase = (ulong)Allocator.Allocate(1024);
                HBAPort->CommandListBase = (uint)newBase;
                HBAPort->CommandListBaseUpper = (uint)(newBase >> 32);

                ulong fisBase = (ulong)Allocator.Allocate(256);
                HBAPort->FISBaseAddress = (uint)fisBase;
                HBAPort->FISBaseAddressUpper = (uint)(fisBase >> 32);

                HBACommandHeader* cmdhdr = (HBACommandHeader*)(HBAPort->CommandListBase | ((ulong)HBAPort->CommandListBaseUpper << 32));

                for (int i = 0; i < 32; i++)
                {
                    cmdhdr[i].PRDTLength = 8;
                    ulong cmdTableAddr = (ulong)Allocator.Allocate(256);
                    cmdTableAddr += (ulong)(i << 8);
                    cmdhdr[i].CommandTableBaseAddress = (uint)cmdTableAddr;
                    cmdhdr[i].CommandTableBaseAddressUpper = (uint)(cmdTableAddr >> 32);
                }

                StartCommand();
            }

            public void StartCommand()
            {
                while ((HBAPort->CommandStatus & 0x8000) != 0)
                {
                    ;
                }

                HBAPort->CommandStatus |= 0x0010;
                HBAPort->CommandStatus |= 0x0001;
            }

            public void StopCommand()
            {
                HBAPort->CommandStatus &= ~0x0001U;
                HBAPort->CommandStatus &= ~0x0010U;

                while (true)
                {
                    if ((HBAPort->CommandStatus & 0x4000) != 0)
                    {
                        continue;
                    }

                    if ((HBAPort->CommandStatus & 0x8000) != 0)
                    {
                        continue;
                    }

                    break;
                }
            }

            public const int SectorSize = 512;

            public override bool Read(ulong sector, uint count, byte* p)
            {
                for (ulong i = 0; i < count; i++)
                {
                    bool b = ReadOrWrite((uint)(sector + i), p + (i * SectorSize), false);
                    if (!b)
                    {
                        return false;
                    }
                }
                return true;
            }

            public override bool Write(ulong sector, uint count, byte* p)
            {
                for (ulong i = 0; i < count; i++)
                {
                    bool b = ReadOrWrite((uint)(sector + i), p + (i * SectorSize), false);
                    if (!b)
                    {
                        return false;
                    }
                }
                return true;
            }

            private bool ReadOrWrite(ulong Sector, byte* Buffer, bool Write)
            {
                if (PortType == SATAPortType.ATAPI && Write)
                {
                    return false;
                }

                unchecked
                { HBAPort->InterruptStatus = (uint)-1; }
                int Slot = FindSlot();
                if (Slot == -1)
                {
                    return false;
                }

                HBACommandHeader* hdr = (HBACommandHeader*)(HBAPort->CommandListBase | ((ulong)HBAPort->CommandListBaseUpper << 32));
                hdr += Slot;
                hdr->CommandFISLength = (byte)(sizeof(FIS_REG_H2D) / sizeof(uint));
                hdr->Write = Write;
                hdr->ClearBusy = true;
                hdr->PRDTLength = 1;

                HBACommandTable* table = (HBACommandTable*)(hdr->CommandTableBaseAddress | ((ulong)hdr->CommandTableBaseAddressUpper << 32));

                Native.Stosb(table, 0, (ulong)(sizeof(HBACommandTable) + ((hdr->PRDTLength - 1) * sizeof(HBAPRDTEntry))));

                table->PRDTEntry.DataBaseAddress = (uint)(ulong)Buffer;
                table->PRDTEntry.DataBaseAddressUpper = (uint)((ulong)Buffer << 32);
                table->PRDTEntry.ByteCount = 511;
                table->PRDTEntry.InterruptOnCompletion = true;

                FIS_REG_H2D* FIS = (FIS_REG_H2D*)table->CommandFIS;
                FIS->FISType = 0x27;
                FIS->CommandControl = true;
                FIS->Command = (byte)(Write ? 0x35 : 0x25);

                FIS->LBA0 = (byte)((Sector) & 0xFF);
                FIS->LBA1 = (byte)((Sector >> 8) & 0xFF);
                FIS->LBA2 = (byte)((Sector >> 16) & 0xFF);
                FIS->LBA3 = (byte)((Sector >> 24) & 0xFF);
                FIS->LBA4 = (byte)((Sector >> 32) & 0xFF);
                FIS->LBA5 = (byte)((Sector >> 40) & 0xFF);

                FIS->DeviceRegister = 1 << 6;

                FIS->CountLow = 1;
                FIS->CountHigh = 0;

                while ((HBAPort->TaskFileData & (0x80 | 0x08)) != 0)
                {
                    ;
                }

                HBAPort->CommandIssue = (uint)(1 << Slot);

                while (true)
                {
                    if ((HBAPort->CommandIssue & (1 << Slot)) == 0)
                    {
                        break;
                    }

                    if ((HBAPort->InterruptStatus & (1 << 30)) != 0)
                    {
                        return false;
                    }
                }

                if ((HBAPort->InterruptStatus & (1 << 30)) != 0)
                {
                    return false;
                }

                while (HBAPort->CommandIssue != 0)
                {
                    ;
                }

                return true;
            }

            public int FindSlot()
            {
                uint Slots = HBAPort->SataActive | HBAPort->CommandIssue;
                for (int i = 0; i < 32; i++)
                {
                    if ((Slots & (1 << i)) == 0)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }
}
