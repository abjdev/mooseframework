//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System
{
    public struct Int32
    {
        public const int MaxValue = 2147483647;
        public const int MinValue = -2147483648;

        public override string ToString()
        {
            return ((long)this).ToString();
        }
    }
}