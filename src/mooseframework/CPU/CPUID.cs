//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Runtime.InteropServices;

namespace moose.CPU
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CPUID
    {
        public uint EAX;
        public uint EBX;
        public uint ECX;
        public uint EDX;
    }
}
