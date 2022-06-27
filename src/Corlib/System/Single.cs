//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace System
{
    public struct Single
    {
        public override unsafe string ToString()
        {
            return ((double)this).ToString();
        }
    }
}