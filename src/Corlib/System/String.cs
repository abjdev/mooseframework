//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Runtime.CompilerServices;
using System.Text;
using Internal.Runtime.CompilerHelpers;
using Internal.Runtime.CompilerServices;
using moose.Memory;

namespace System
{
    public sealed unsafe class String
    {
        [Intrinsic]
        public static readonly string Empty = "";


        // The layout of the string type is a contract with the compiler.
        private int _length;
        internal char _firstChar;


        public int Length
        {
            [Intrinsic]
            get => _length;
            internal set => _length = value;
        }

        public unsafe char this[int index]
        {
            [Intrinsic]
            get => Unsafe.Add(ref _firstChar, index);

            set
            {
                fixed (char* p = &_firstChar)
                {
                    p[index] = value;
                }
            }
        }


#pragma warning disable CS0824 // Constructor is marked external
        public extern unsafe String(char* ptr);
        public extern String(IntPtr ptr);
        public extern String(char[] buf);
        public extern unsafe String(char* ptr, int index, int length);
        public extern unsafe String(char[] buf, int index, int length);
#pragma warning restore CS0824 // Constructor is marked external


        public static unsafe string FromASCII(IntPtr ptr, int length)
        {
            char[] buf = new char[length];
            byte* _ptr = (byte*)ptr;

            for (int i = 0; i < length; i++)
            {
                buf[i] = (char)_ptr[i];
            }

            string s = new(buf);
            buf.Dispose();

            return s;
        }

        public static unsafe string FromASCII(IntPtr ptr, int length, byte ignore)
        {
            char[] buf = new char[length];
            byte* _ptr = (byte*)ptr;

            int len = 0;

            for (int i = 0; i < length; i++)
            {
                if (_ptr[i] != ignore)
                {
                    buf[i] = (char)_ptr[i];
                    len++;
                }
            }

            string s = new(buf, 0, len);
            buf.Dispose();

            return s;
        }

        private static unsafe string Ctor(char* ptr)
        {
            int i = 0;

            while (ptr[i++] != '\0')
            { }

            return Ctor(ptr, 0, i - 1);
        }
        private static unsafe string Ctor(IntPtr ptr)
        {
            return Ctor((char*)ptr);
        }
        private static unsafe string Ctor(char[] buf)
        {
            fixed (char* _buf = buf)
            {
                return Ctor(_buf, 0, buf.Length);
            }
        }
        private static unsafe string Ctor(char* ptr, int index, int length)
        {
            EETypePtr et = EETypePtr.EETypePtrOf<string>();

            char* start = ptr + index;
            object data = StartupCodeHelpers.RhpNewArray(et.Value, length);
            string s = Unsafe.As<object, string>(ref data);

            fixed (char* c = &s._firstChar)
            {
                Allocator.MemoryCopy((IntPtr)c, (IntPtr)start, (ulong)length * sizeof(char));
                c[length] = '\0';
            }

            return s;
        }
        private static unsafe string Ctor(char[] ptr, int index, int length)
        {
            fixed (char* _ptr = ptr)
            {
                return Ctor(_ptr, index, length);
            }
        }

        public override string ToString()
        {
            return this;
        }

        public override bool Equals(object obj)
        {
#pragma warning disable IDE0038 // Use pattern matching
            return obj is string && Equals((string)obj);
#pragma warning restore IDE0038 // Use pattern matching
        }

        public bool Equals(string val)
        {
            if (Length != val.Length)
            {
                return false;
            }

            for (int i = 0; i < Length; i++)
            {
                if (this[i] != val[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator ==(string a, string b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(string a, string b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static string Concat(string a, string b)
        {
            int Length = a.Length + b.Length;
            char* ptr = stackalloc char[Length];
            int currentIndex = 0;
            for (int i = 0; i < a.Length; i++)
            {
                ptr[currentIndex] = a[i];
                currentIndex++;
            }
            for (int i = 0; i < b.Length; i++)
            {
                ptr[currentIndex] = b[i];
                currentIndex++;
            }
            return new string(ptr, 0, Length);
        }

        internal int IndexOf(char j)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this[i] == j)
                {
                    return i;
                }
            }

            return -1;
        }

        public static string Concat(string a, string b, string c)
        {
            string p1 = a + b;
            string p2 = p1 + c;
            p1.Dispose();
            return p2;
        }

        public static string Concat(string a, string b, string c, string d)
        {
            string p1 = a + b;
            string p2 = p1 + c;
            string p3 = p2 + d;
            p1.Dispose();
            p2.Dispose();
            return p3;
        }

        public static string Concat(params string[] vs)
        {
            string s = "";
            for (int i = 0; i < vs.Length; i++)
            {
                string tmp = s + vs[i];
                s.Dispose();
                s = tmp;
            }
            vs.Dispose();
            return s;
        }
        public static string Format(string format, params object[] args)
        {
            lock (format)
            {
                string res = Empty;
                for (int i = 0; i < format.Length; i++)
                {
                    string chr;
                    if ((i + 2) < format.Length && format[i] == '{' && format[i + 2] == '}')
                    {
                        chr = args[format[i + 1] - 0x30].ToString();
                        i += 2;
                    } else
                    {
                        chr = format[i].ToString();
                    }
                    string str = res + chr;
                    chr.Dispose();
                    res.Dispose();
                    res = str;
                }

                for (int i = 0; i < args.Length; i++)
                {
                    args[i].Dispose();
                }

                args.Dispose();
                return res;
            }
        }

        internal static string FastAllocateString(int length)
        {
            return new(Allocator.Allocate((ulong)(2 * length)));
        }

        internal static unsafe void wstrcpy(char* dmem, char* smem, int charCount)
        {
            Native.Movsb((byte*)dmem, (byte*)smem, (ulong)(charCount * 2));
        }

        public string Substring(int startIndex)
        {
            if ((Length == 0) && (startIndex == 0))
            {
                return Empty;
            }
            // Usually one uses the extension method with non-null values
            // so all we need to worry about is startIndex compared to value.Length.
            if ((startIndex < 0) || (startIndex >= Length))
            {
                //throw new ArgumentOutOfRangeException("startIndex");
            }

            StringBuilder sb = new();
            for (int i = startIndex; i < Length; i++)
            {
                sb.Append(this[i]);
            }
            return sb.ToString();
        }
        public string Substring(int startIndex, int endIndex)
        {
            if ((Length == 0) && (startIndex == 0))
            {
                return Empty;
            }
            // Usually one uses the extension method with non-null values
            // so all we need to worry about is startIndex compared to value.Length.
            if ((startIndex < 0) || (startIndex >= Length) || (endIndex >= Length) || (endIndex <= startIndex))
            {
                //throw new ArgumentOutOfRangeException("startIndex");
            }

            StringBuilder sb = new();
            for (int i = startIndex; i < endIndex; i++)
            {
                sb.Append(this[i]);
            }
            return sb.ToString();
        }
        /*public bool EndsWith(string str)
        {
            bool x = true;
            for (int i = str.Length; i >= 0; i--)
            {
                if (this[i] != str[i])
                {
                    x = false;
                    break;
                }
            }
            return x;
        }*/

    }
}