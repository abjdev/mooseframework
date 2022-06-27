//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Runtime.InteropServices;

namespace moose.CPU
{
    public static unsafe class AVX
    {
        public static void Initialize()
        {
            CPUID* cpuid = Native.CPUID(1);
            if (!BitHelpers.IsBitSet(cpuid->ECX, 28))
            {
                Console.WriteLine("[AVX] Warning: Current CPU doesn't support AVX");
                return;
            }
            enable_avx();
        }

        [DllImport("*")]
        public static extern void enable_avx();

        [DllImport("*")]
        public static extern void avx_memcpy(void* pvDest, void* pvSrc, ulong nBytes);
    }
}
