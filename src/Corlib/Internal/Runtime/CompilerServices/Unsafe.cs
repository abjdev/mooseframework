//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Runtime.CompilerServices;

namespace Internal.Runtime.CompilerServices
{
    public static unsafe class Unsafe
    {
        [Intrinsic]
        public static extern ref T Add<T>(ref T source, int elementOffset);

        [Intrinsic]
        public static extern ref TTo As<TFrom, TTo>(ref TFrom source);

        [Intrinsic]
        public static extern T As<T>(object value) where T : class;

        [Intrinsic]
        public static extern void* AsPointer<T>(ref T value);

        [Intrinsic]
        public static extern ref T AsRef<T>(void* pointer);

        public static ref T AsRef<T>(IntPtr pointer)
        {
            return ref AsRef<T>((void*)pointer);
        }

        [Intrinsic]
        public static extern int SizeOf<T>();

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060 // Remove unused parameter
        public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            for (; ; )
            {
                ;
            }
        }

        [Intrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T AddByteOffset<T>(ref T source, nuint byteOffset)
        {
            return ref AddByteOffset(ref source, (IntPtr)(void*)byteOffset);
        }
    }
}