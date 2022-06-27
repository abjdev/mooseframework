//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace moose.Graphics
{
    public static unsafe class VBE
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static VBEInfo* Info;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static void Initialize(VBEInfo* info)
        {
            Info = info;
            if (info->PhysBase != 0)
            {
                Framebuffer.VideoMemory = (uint*)info->PhysBase;
                Framebuffer.SetVideoMode(info->ScreenWidth, info->ScreenHeight);
                Framebuffer.Graphics.Clear(0x0);
            }
        }
    }
}
