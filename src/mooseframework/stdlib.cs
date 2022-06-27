//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Runtime;
using moose.Memory;

namespace moose
{
    public static unsafe class stdlib
    {
        [RuntimeExport("malloc")]
        public static void* malloc(ulong size)
        {
            return (void*)Allocator.Allocate(size);
        }

        [RuntimeExport("free")]
        public static void free(void* ptr)
        {
            Allocator.Free((System.IntPtr)ptr);
        }

        [RuntimeExport("realloc")]
        public static void* realloc(void* ptr, ulong size)
        {
            return (void*)Allocator.Reallocate((IntPtr)ptr, size);
        }
        [RuntimeExport("memmove")]
        public static void* memmove(void* destination, void* source, UIntPtr s)
        {
            Native.Movsb(destination, source, (ulong)s);
            return destination;
        }
        [RuntimeExport("abs")]
        public static int abs(int number)
        {
            return Math.Abs(number);
        }

        [RuntimeExport("calloc")]
        public static void* calloc(ulong num, ulong size)
        {
            void* ptr = (void*)Allocator.Allocate(num * size);
            Native.Stosb(ptr, 0, num * size);
            return ptr;
        }

        [RuntimeExport("kmalloc")]
        public static void* kmalloc(ulong size)
        {
            return (void*)Allocator.Allocate(size);
        }

        [RuntimeExport("kfree")]
        public static void kfree(void* ptr)
        {
            Allocator.Free((System.IntPtr)ptr);
        }

        [RuntimeExport("krealloc")]
        public static void* krealloc(void* ptr, ulong size)
        {
            return (void*)Allocator.Reallocate((System.IntPtr)ptr, size);
        }


        [RuntimeExport("kcalloc")]
        public static void* kcalloc(ulong num, ulong size)
        {
            void* ptr = (void*)Allocator.Allocate(num * size);
            Native.Stosb(ptr, 0, num * size);
            return ptr;
        }
    }
}
