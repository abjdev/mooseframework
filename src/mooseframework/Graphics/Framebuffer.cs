//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Windows.Forms;
using moose.Memory;

namespace moose.Graphics
{
    public static unsafe class Framebuffer
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static ushort Width;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static ushort Height;
#pragma warning restore CA2211 // Non-constant fields should not be visible

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static uint* VideoMemory;
#pragma warning restore CA2211 // Non-constant fields should not be visible

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static uint* FirstBuffer;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static uint* SecondBuffer;
#pragma warning restore CA2211 // Non-constant fields should not be visible

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static Graphics Graphics;
#pragma warning restore CA2211 // Non-constant fields should not be visible
        private static bool _TripleBuffered = false;

        /// <summary>
        /// Since you enabled TripleBuffered you have to call Framebuffer.Graphics.Update() in order to make it display
        /// </summary>
        public static bool TripleBuffered
        {
            get => _TripleBuffered;
            set
            {
                if (Graphics == null)
                {
                    return;
                }

                if (_TripleBuffered == value)
                {
                    return;
                }

                Graphics.Clear(0x0);
                _TripleBuffered = value;
                if (!_TripleBuffered)
                {
                    Console.Clear();
                    Graphics.Clear(0x0);
                }
                Graphics.Update();
            }
        }

        public static void Update()
        {
            if (TripleBuffered)
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    if (FirstBuffer[i] != SecondBuffer[i])
                    {
                        VideoMemory[i] = FirstBuffer[i];
                    }
                }
                Native.Movsd(SecondBuffer, FirstBuffer, (ulong)(Width * Height));
            }
            if (Graphics != null && Graphics is VMWareSVGAIIGraphics)
            {
                Graphics.Update();
            }
        }

        public static void SetVideoMode(ushort XRes, ushort YRes)
        {
            Width = XRes;
            Height = YRes;
            FirstBuffer = (uint*)Allocator.Allocate((ulong)(XRes * YRes * 4));
            SecondBuffer = (uint*)Allocator.Allocate((ulong)(XRes * YRes * 4));
            Native.Stosd(FirstBuffer, 0, (ulong)(XRes * YRes));
            Native.Stosd(SecondBuffer, 0, (ulong)(XRes * YRes));
            Control.MousePosition.X = XRes / 2;
            Control.MousePosition.Y = YRes / 2;
            Graphics = new Graphics(Width, Height, VideoMemory);
        }
    }
}
