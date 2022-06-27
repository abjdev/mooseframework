//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace moose.Misc
{
    public static class PCIClassID
    {
        public static string GetName(byte id)
        {
            return id switch
            {
                0x00 => "Unclassified device",
                0x01 => "Mass storage controller",
                0x02 => "Network controller",
                0x03 => "Display controller",
                0x04 => "Multimedia controller",
                0x05 => "Memory controller",
                0x06 => "Bridge",
                0x07 => "Communication controller",
                0x08 => "Generic system peripheral",
                0x09 => "Input device controller",
                0x0a => "Docking station",
                0x0b => "Processor",
                0x0c => "Serial bus controller",
                0x0d => "Wireless controller",
                0x0e => "Intelligent controller",
                0x0f => "Satellite communications controller",
                0x10 => "Encryption controller",
                0x11 => "Signal processing controller",
                0x12 => "Processing accelerators",
                0x13 => "Non-Essential Instrumentation",
                0x40 => "Coprocessor",
                0xff => "Unassigned class",
                _ => "Unknown class"
            };
        }
    }
}