//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class UnmanagedFunctionPointerAttribute : Attribute
    {
        public UnmanagedFunctionPointerAttribute()
        {
            CallingConvention = CallingConvention.Winapi;
        }

        public UnmanagedFunctionPointerAttribute(CallingConvention callingConvention)
        {
            CallingConvention = callingConvention;
        }

        public CallingConvention CallingConvention { get; }

        public bool BestFitMapping;
        public bool SetLastError;
        public bool ThrowOnUnmappableChar;
        public CharSet CharSet;
    }
}