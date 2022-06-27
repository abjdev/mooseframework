//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Collections.Generic;

namespace moose.FileSystem
{
    public class FileInfo
    {
        public string Name;
        public FileAttribute Attribute;

        public override void Dispose()
        {
            Name.Dispose();
            base.Dispose();
        }
    }

    public enum FileAttribute : byte
    {
        ReadOnly = 0x01,
        Hidden = 0x02,
        System = 0x04,
        Directory = 0x10,
        Archive = 0x20,
    }

    public abstract class File
    {
        /// <summary>
        /// This will be overwritten if you initialize file system
        /// </summary>
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static File Instance;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public File()
        {
            Instance = this;
        }

        public abstract List<FileInfo> GetFiles(string Directory);
        public abstract void Delete(string Name);
        public abstract byte[] ReadAllBytes(string Name);
        public abstract void WriteAllBytes(string Name, byte[] Content);
        public abstract void Format();
    }
}
