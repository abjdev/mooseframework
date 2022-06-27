//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Runtime.InteropServices;

namespace moose.FileSystem
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VHDFooter
    {
        public fixed byte cookie[8];
        public uint features;
        public uint ffversion;
        public ulong dataoffset;
        public uint timestamp;
        public uint creatorapp;
        public uint creatorver;
        public uint creatorhos;
        public ulong origsize;
        public ulong currsize;
        public uint diskgeom;
        public uint disktype;
        public uint checksum;
        public fixed byte uniqueid[16];
        public byte savedst;
        public fixed byte reserved[427];
    }
}
