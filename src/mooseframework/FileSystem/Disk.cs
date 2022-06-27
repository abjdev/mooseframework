//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace moose.FileSystem
{
    public abstract unsafe class Disk
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static Disk Instance;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public Disk()
        {
            Instance = this;
        }

        public abstract bool Read(ulong sector, uint count, byte* data);
        public abstract bool Write(ulong sector, uint count, byte* data);
    }
}
