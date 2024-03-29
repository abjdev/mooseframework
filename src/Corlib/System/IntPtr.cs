//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

using System.Runtime.CompilerServices;

namespace System
{
    public unsafe struct IntPtr
    {
        private void* _value;
        public unsafe void* ToPointer()
        {
            return _value;
        }
        public IntPtr(void* value) { _value = value; }
        public IntPtr(int value) { _value = (void*)value; }
        public IntPtr(uint value) { _value = (void*)value; }
        public IntPtr(long value) { _value = (void*)value; }
        public IntPtr(ulong value) { _value = (void*)value; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        [Intrinsic]
        public static readonly IntPtr Zero;

        public bool Equals(IntPtr ptr)
        {
            return _value == ptr._value;
        }

        public static explicit operator IntPtr(int value)
        {
            return new IntPtr(value);
        }

        public static explicit operator IntPtr(uint value)
        {
            return new IntPtr(value);
        }

        public static explicit operator IntPtr(long value)
        {
            return new IntPtr(value);
        }

        public static explicit operator IntPtr(ulong value)
        {
            return new IntPtr(value);
        }

        public static explicit operator IntPtr(void* value)
        {
            return new IntPtr(value);
        }

        public static explicit operator void*(IntPtr value)
        {
            return value._value;
        }

        public static explicit operator int(IntPtr value)
        {
            long l = (long)value._value;

            return checked((int)l);
        }

        public static explicit operator long(IntPtr value)
        {
            return (long)value._value;
        }

        public static explicit operator ulong(IntPtr value)
        {
            return (ulong)value._value;
        }

        public static explicit operator IntPtr(UIntPtr ptr)
        {
            return new IntPtr() { _value = (void*)ptr };
        }

        public static IntPtr operator +(IntPtr a, uint b)
        {
            return new IntPtr((byte*)a._value + b);
        }

        public static IntPtr operator +(IntPtr a, ulong b)
        {
            return new IntPtr((byte*)a._value + b);
        }

        public static bool operator ==(IntPtr a, IntPtr b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(IntPtr a, IntPtr b)
        {
            return !(a._value == b._value);
        }

        public override string ToString()
        {
            return ((UIntPtr)this).ToString();
        }
    }
}