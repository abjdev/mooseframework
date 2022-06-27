//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System
{
    public struct Int16
    {
        public override string ToString()
        {
            return ((long)this).ToString();
        }
    }
}