//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Runtime;
using System.Runtime.InteropServices;
using moose.FileSystem;
using moose.Memory;
using moose.Misc;

namespace moose
{
    internal unsafe class stdio
    {
        [RuntimeExport("_putchar")]
        public static void _putchar(byte chr)
        {
            if (chr == '\n')
            {
                Console.WriteLine();
            } else
            {
                Console.Write((char)chr);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FILE
        {
            public byte* DATA;
            public long OFFSET;
            public long LENGTH;
        }

        public enum SEEK
        {
            SET,
            CUR,
            END
        }

        [RuntimeExport("fopen")]
        public static FILE* fopen(byte* name, byte* mode)
        {
            string sname = string.FromASCII((IntPtr)name, strings.strlen(name));
            FILE file = new();
            byte[] buffer = File.Instance.ReadAllBytes(sname);
            if (buffer == null)
            {
                Panic.Error("fopen: file not found");
            }
            file.DATA = (byte*)Allocator.Allocate((ulong)buffer.Length);
            fixed (byte* p = buffer)
            {
                Native.Movsb(file.DATA, p, (ulong)buffer.Length);
            }

            file.LENGTH = buffer.Length;
            buffer.Dispose();
            sname.Dispose();
            return &file;
        }

        [RuntimeExport("fclose")]
        public static int fclose(FILE* stream)
        {
            stream->DATA->Dispose();
            stream->LENGTH.Dispose();
            stream->OFFSET.Dispose();
            stream->Dispose();
            return 0;
        }

        [RuntimeExport("fseek")]
        public static void fseek(FILE* handle, long offset, SEEK seek)
        {
            if (seek == SEEK.SET)
            {
                handle->OFFSET = offset;
            } else
            {
                Panic.Error("Non-implemented seek");
            }
        }
        [RuntimeExport("ftell")]
        public static ulong ftell(FILE* stream)
        {
            return (ulong)stream->OFFSET;
        }

        [RuntimeExport("fread")]
        public static void fread(byte* buffer, long elementSize, long elementCount, FILE* handle)
        {
            Native.Movsb(buffer, handle->DATA + handle->OFFSET, (ulong)elementSize);
        }
    }
}
