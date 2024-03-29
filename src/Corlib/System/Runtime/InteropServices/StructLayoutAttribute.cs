//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class StructLayoutAttribute : Attribute
    {
        public StructLayoutAttribute(LayoutKind layoutKind)
        {
            Value = layoutKind;
        }

        public LayoutKind Value { get; }

        public int Pack;
        public int Size;
        public CharSet CharSet;
    }

    public enum LayoutKind
    {
        Sequential = 0,
        Explicit = 2,
        Auto = 3,
    }
}