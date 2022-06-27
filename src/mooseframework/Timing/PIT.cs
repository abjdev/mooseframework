//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using moose.Misc;

namespace moose.Timing
{
    public class PIT
    {
        public static void Initialize()
        {
            ushort timerCount = 1193182 / 1000;

            Native.Out8(0x43, 0x36);
            Native.Out8(0x40, (byte)(timerCount & 0xFF));
            Native.Out8(0x40, (byte)((timerCount & 0xFF00) >> 8));

            Interrupts.EnableInterrupt(0x20);
        }
    }
}
