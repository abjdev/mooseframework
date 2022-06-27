//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace System
{
    public struct RuntimeTypeHandle
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IntPtr Value;
#pragma warning restore IDE0052 // Remove unread private members

        public RuntimeTypeHandle(EETypePtr ptr)
        {
            Value = ptr.RawValue;
        }
    }
}