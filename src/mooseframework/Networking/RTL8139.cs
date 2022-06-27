//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using moose.Drivers;
using moose.Memory;
using moose.Misc;
using static Native;

namespace moose.Networking
{
    public unsafe class RTL8139 : NIC
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static ushort IOBase = 0;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static uint RX = 0;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static byte IRQ = 0;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static byte[] MAC = null;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static byte[] TSAD = null;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static byte[] TSD = null;
#pragma warning restore CA2211 // Non-constant fields should not be visible
        private static ushort CurrentPointer = 0;
        private static int TXIndex = 0;

        public static void Initialize()
        {
            TSAD = new byte[] { 0x20, 0x24, 0x28, 0x2C };
            TSD = new byte[] { 0x10, 0x14, 0x18, 0x1C };

            PCIDevice device = PCI.GetDevice(0x10EC, 0x8139);
            if (device == null)
            {
                Console.WriteLine("[RTL8139] RTL8139 Not Found");
                return;
            }
            Console.WriteLine("[RTL8139] RTL8139 Found");
            IOBase = (ushort)(device.Bar0 & ~0x3);
            device.WriteRegister16(0x04, 0x04 | 0x02 | 0x01);

            Out8((ushort)(IOBase + 0x52), 0x00);
            Out8((ushort)(IOBase + 0x37), 0x10);
            while ((In8((ushort)(IOBase + 0x37)) & 0x10) != 0)
            {
                ;
            }

            Console.WriteLine("[RTL8139] Soft-Reset Done");

            uint p = (uint)Allocator.Allocate(8192 + 16 + 1500 + 0xF);
            p &= ~0xFU;
            Console.Write("[RTL8139] RX is at:");
            Console.WriteLine(((ulong)p).ToStringHex());
            RX = p;

            Out32((ushort)(IOBase + 0x30), p);

            Out16((ushort)(IOBase + 0x3C), 0x5);
            Out32((ushort)(IOBase + 0x44), 0xF | (1 << 7));
            Out8((ushort)(IOBase + 0x37), 0xC);
            Console.WriteLine("[RTL8139] RTL8139 Configuration Done");
            Console.Write("[RTL8139] IRQ:");
            Console.WriteLine(((ulong)device.IRQ).ToStringHex());

            MAC = new byte[6];
            MAC[0] = In8((ushort)(IOBase + 0));
            MAC[1] = In8((ushort)(IOBase + 1));
            MAC[2] = In8((ushort)(IOBase + 2));
            MAC[3] = In8((ushort)(IOBase + 3));
            MAC[4] = In8((ushort)(IOBase + 4));
            MAC[5] = In8((ushort)(IOBase + 5));
            Console.Write("[RTL8139] MAC:");
            Console.Write(((ulong)MAC[0]).ToStringHex());
            Console.Write(":");
            Console.Write(((ulong)MAC[1]).ToStringHex());
            Console.Write(":");
            Console.Write(((ulong)MAC[2]).ToStringHex());
            Console.Write(":");
            Console.Write(((ulong)MAC[3]).ToStringHex());
            Console.Write(":");
            Console.Write(((ulong)MAC[4]).ToStringHex());
            Console.Write(":");
            Console.WriteLine(((ulong)MAC[5]).ToStringHex());

            Network.MAC = MAC;
            IRQ = device.IRQ;
            Interrupts.EnableInterrupt(device.IRQ);

            Network.Controller = new RTL8139();
        }

        public static void OnInterrupt()
        {
            ushort Status = In16((ushort)(IOBase + 0x3E));
            if ((Status & (1 << 2)) != 0)
            {
                //Debug.WriteLine("Transmit OK");
            }
            if ((Status & (1 << 0)) != 0)
            {
                //Debug.WriteLine("Receive OK");

                ushort* t = (ushort*)(RX + CurrentPointer);
                ushort length = *(t + 1);
                t += 2;

                Ethernet.HandlePacket((byte*)t, length);

                CurrentPointer = (ushort)((CurrentPointer + length + 4 + 3) & ~3);
                Out16((ushort)(IOBase + 0x38), (ushort)(CurrentPointer - 0x10));
            }
            Out16((ushort)(IOBase + 0x3E), 0x05);
        }

        public override void Send(byte* Data, int Length)
        {
            Out32((ushort)(IOBase + TSAD[TXIndex]), (uint)Data);
            Out32((ushort)(IOBase + TSD[TXIndex++]), (uint)Length);
            if (TXIndex > 3)
            {
                TXIndex = 0;
            }
        }
    }
}
