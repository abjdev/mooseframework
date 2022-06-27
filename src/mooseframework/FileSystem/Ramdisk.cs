//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;

namespace moose.FileSystem
{
    public unsafe class Ramdisk : Disk
    {
        private readonly byte* ptr;

        public Ramdisk(IntPtr _ptr)
        {
            ptr = (byte*)_ptr;
        }

        public override bool Read(ulong sector, uint count, byte* p)
        {
            Native.Movsb(p, ptr + (sector * 512), 512 * count);
            return true;
        }

        public override bool Write(ulong sector, uint count, byte* p)
        {
            Native.Movsb(ptr + (sector * 512), p, 512 * count);
            return true;
        }
    }
}
