//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace moose.Graphics
{
    internal unsafe class VMWareSVGAIIGraphics : Graphics
    {
        private readonly VMWareSVGAII svga;
        public VMWareSVGAIIGraphics() : base(Framebuffer.Width, Framebuffer.Height, Framebuffer.FirstBuffer)
        {
            svga = new VMWareSVGAII();
            svga.SetMode(Framebuffer.Width, Framebuffer.Height);
            Framebuffer.VideoMemory = svga.Video_Memory;
        }

        public override void Update()
        {
            svga.Update();
        }
    }
}
