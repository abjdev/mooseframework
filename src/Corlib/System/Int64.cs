//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System
{
    public unsafe struct Int64
    {
        public const long MaxValue = 0x7fffffffffffffffL;
        public const long MinValue = unchecked((long)0x8000000000000000L);
        public override string ToString()
        {
            long val = this;
            bool isNeg = BitHelpers.IsBitSet(val, 63);
            char* x = stackalloc char[22];
            int i = 20;

            x[21] = '\0';

            if (isNeg)
            {
                ulong _val = (ulong)val;
                _val = 0xFFFFFFFFFFFFFFFF - _val;
                _val += 1;
                val = (long)_val;
            }

            do
            {
                long d = val % 10;
                val /= 10;

                d += 0x30;
                x[i--] = (char)d;
            } while (val > 0);

            if (isNeg)
            {
                x[i] = '-';
            } else
            {
                i++;
            }

            return new string(x + i, 0, 21 - i);
        }
    }
}