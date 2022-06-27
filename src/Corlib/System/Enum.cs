//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Runtime.CompilerServices;

namespace System
{
    public abstract class Enum : ValueType
    {
        [Intrinsic]
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter
        public bool HasFlag(Enum flag)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
        {
            return false;
        }
    }
}