//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace moose.Timing
{
    public static class Timer
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static ulong Ticks = 0;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        internal static void OnInterrupt()
        {
            Ticks++;
        }

        public static void Wait(ulong millisecond)
        {
            ulong T = Ticks;
            while (Ticks < (T + millisecond))
            {
                Native.Hlt();
            }
        }
    }
}
