//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
//https://wiki.osdev.org/HPET#Interrupt_routing
using System;
using moose.Power;
using static moose.Misc.MMIO;
namespace moose.Drivers
{
    public static unsafe class HPET
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static ulong Clock;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static void Initialize()
        {
            if (ACPI.HPET == null)
            {
                Console.WriteLine("[HPET] HPET not found");
                return;
            }

            //1 Femtosecond= 1e-15 sec
            Clock = In(0) >> 32;
            Out(0x10, 0);
            Out(0xF0, 0);
            Out(0x10, 1);
            Console.WriteLine("[HPET] HPET Initialized");
        }

        public static void Out(ulong reg, ulong value)
        {
            Out64((ulong*)(ACPI.HPET->Addresses.Address + reg), value);
        }

        public static ulong In(ulong reg)
        {
            return In64((ulong*)(ACPI.HPET->Addresses.Address + reg));
        }

        public static ulong GetTickCount()
        {
            return In(0xF0);
        }

        public static void Wait(ulong Millionseconds)
        {
            WaitMicrosecond(Millionseconds * 10000);
        }

        public static void WaitMicrosecond(ulong Microsecond)
        {
            ulong Until = GetTickCount() + (Microsecond * 1000000000 / Clock);
            while (GetTickCount() < Until)
            {
                Native.Nop();
            }
        }
    }
}
