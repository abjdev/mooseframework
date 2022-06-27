//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using moose.Misc;

namespace moose.CPU
{
    public static partial class LocalAPIC
    {
        public static ulong Ticks => In(0x390);

        public static void StartTimer(ulong freq, uint irq)
        {
            Out(0x320, 0x00020000 | irq);
            Out(0x3e0, 0x3);
            Out(0x380, (uint)freq);
            Interrupts.EnableInterrupt(0x20);
        }

        public static void StopTimer()
        {
            Out(0x320, 0x00020000 | 0x10000);
        }
    }
}