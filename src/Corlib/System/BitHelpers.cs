//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System
{
    internal static class BitHelpers
    {
        public static bool IsBitSet(sbyte val, int pos)
        {
            return (((byte)val) & (1U << pos)) != 0;
        }

        public static bool IsBitSet(byte val, int pos)
        {
            return (val & (1U << pos)) != 0;
        }

        public static bool IsBitSet(short val, int pos)
        {
            return (((ushort)val) & (1U << pos)) != 0;
        }

        public static bool IsBitSet(ushort val, int pos)
        {
            return (val & (1U << pos)) != 0;
        }

        public static bool IsBitSet(int val, int pos)
        {
            return (((uint)val) & (1U << pos)) != 0;
        }

        public static bool IsBitSet(uint val, int pos)
        {
            return (val & (1U << pos)) != 0;
        }

        public static bool IsBitSet(long val, int pos)
        {
            return (((ulong)val) & (1UL << pos)) != 0;
        }

        public static bool IsBitSet(ulong val, int pos)
        {
            return (val & (1UL << pos)) != 0;
        }
    }
}