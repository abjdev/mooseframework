//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class MethodImplAttribute : Attribute
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public MethodImplAttribute(MethodImplOptions methodImplOptions) { }
#pragma warning restore IDE0060 // Remove unused parameter
    }

    public enum MethodImplOptions
    {
        Unmanaged = 0x0004,
        NoInlining = 0x0008,
        NoOptimization = 0x0040,
        AggressiveInlining = 0x0100,
        AggressiveOptimization = 0x200,
        InternalCall = 0x1000,
    }
}