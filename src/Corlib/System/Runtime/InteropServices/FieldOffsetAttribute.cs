//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class FieldOffsetAttribute : Attribute
    {
        public FieldOffsetAttribute(int offset)
        {
            Value = offset;
        }

        public int Value { get; }
    }
}