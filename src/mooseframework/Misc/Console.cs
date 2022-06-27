//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
#define ASCII

using System.Drawing;
using moose.Drivers;
using moose.Graphics;

namespace System
{
    public static unsafe class Console
    {
        private static int cursorX = 0;
        private static int cursorY = 0;
        private static uint foregroundColor = (uint)ConsoleColor.White;
        private static uint backgroundColor = (uint)ConsoleColor.Black;
        public static int CursorX { get => cursorX; set => cursorX = value; }
        public static int CursorY { get => cursorY; set => cursorY = value; }
        public static ConsoleColor ForegroundColor { get => (ConsoleColor)foregroundColor; set => foregroundColor = (uint)value; }
        public static ConsoleColor BackgroundColor { get => (ConsoleColor)backgroundColor; set => backgroundColor = (uint)value; }

        public delegate void OnWriteHandler(char chr);
        public static event OnWriteHandler OnWrite;

        private static uint[] ColorsFB;

        internal static void Initialize()
        {
            OnWrite = null;

            Clear();

            EnableCursor();
            SetCursorStyle(0b1110);

            ColorsFB = new uint[16]
            {
                Color.Black.ToArgb(),
                Color.Blue.ToArgb(),
                Color.Green.ToArgb(),
                Color.Cyan.ToArgb(),
                Color.Red.ToArgb(),
                Color.Purple.ToArgb(),
                Color.Brown.ToArgb(),
                Color.Gray.ToArgb(),
                Color.DarkGray.ToArgb(),
                Color.LightBlue.ToArgb(),
                Color.LightGreen.ToArgb(),
                Color.LightCyan.ToArgb(),
                Color.MediumVioletRed.ToArgb(),
                Color.MediumPurple.ToArgb(),
                Color.Yellow.ToArgb(),
                Color.White.ToArgb(),
            };

            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;
        }

        private static void SetCursorStyle(byte style)
        {
            Native.Out8(0x3D4, 0x0A);
            Native.Out8(0x3D5, style);
        }

        private static void EnableCursor()
        {
            Native.Out8(0x3D4, 0x0A);
            Native.Out8(0x3D5, (byte)((Native.In8(0x3D5) & 0xC0) | 0));

            Native.Out8(0x3D4, 0x0B);
            Native.Out8(0x3D5, (byte)((Native.In8(0x3D5) & 0xE0) | 15));
        }

        public static void Write(string s)
        {
            for (byte i = 0; i < s.Length; i++)
            {
                Write(s[i]);
            }
            s.Dispose();
        }

        public static void Write(object o)
        {
            string s = o.ToString();
            for (byte i = 0; i < s.Length; i++)
            {
                Write(s[i]);
            }
            s.Dispose();
        }

        public static void Back()
        {
            if (CursorX == 0)
            {
                if (CursorY == 0)
                {
                    return;
                } else
                {
                    CursorY -= 16;
                    CursorX = Framebuffer.Width;
                }
            }

            Framebuffer.Graphics.FillRectangle(CursorX - 8, CursorY, 16, 16, ColorsFB[(int)BackgroundColor]);
            CursorX -= 8;
            UpdateCursor();
        }


        public static void WriteStrAt(string s, byte line)
        {
            for (byte i = 0; i < s.Length; i++)
            {
                WriteAt(s[i], i, line);
            }
        }

        public static void Write(char c)
        {
            if (c == '\n')
            {
                WriteLine();
                return;
            }
#if ASCII
            if (c >= 0x20 && c <= 0x7E)
#else
            unsafe
#endif
            {
                OnWrite?.Invoke(c);
                WriteFramebuffer(c);

                CursorX += 8;
                if (CursorX == Framebuffer.Width)
                {
                    CursorX = 0;
                    CursorY += 16;
                }
                MoveUp();
                UpdateCursor();
            }
        }
        private static void WriteFramebuffer(char chr)
        {
            if (Framebuffer.VideoMemory != null && !Framebuffer.TripleBuffered)
            {
                Framebuffer.Graphics.FillRectangle(CursorX, CursorY, 8, 16, ColorsFB[(int)BackgroundColor]);
                ASC16.DrawChar(chr, CursorX, CursorY, ColorsFB[(int)ForegroundColor]);
            }
        }

        public static ConsoleKeyInfo ReadKey()
        {
            PS2Keyboard.CleanKeyInfo(true);
            while (PS2Keyboard.KeyInfo.KeyChar == '\0')
            {
                Native.Hlt();
            }
            return PS2Keyboard.KeyInfo;
        }

        public static string ReadLine()
        {
            string s = string.Empty;
            ConsoleKeyInfo key;
            while ((key = ReadKey()).Key != ConsoleKey.Enter)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Delete:
                    case ConsoleKey.Backspace:
                        if (s.Length.ToString() == "0")
                        {
                            continue;
                        }
                        Back();
                        s.Length -= 1;
                        break;
                    default:
                        Write((PS2Keyboard.KeyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? key.KeyChar.ToUpper() : key.KeyChar).ToString());
                        s += (PS2Keyboard.KeyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? key.KeyChar.ToUpper() : key.KeyChar).ToString();
                        break;

                }
                //Native.Hlt();
            }
            WriteLine();
            return s;
        }

        private static void MoveUp()
        {
            if (CursorY >= Framebuffer.Height - 16)
            {
                Native.Movsb((void*)0xb8000, (void*)0xB80A0, 0xF00);
                if (Framebuffer.VideoMemory != null && !Framebuffer.TripleBuffered)
                {
                    Framebuffer.Graphics.FillRectangle(0, Framebuffer.Height, Framebuffer.Width, 16, 0x0);
                    Framebuffer.Graphics.Copy(0, -16, 0, 0, Framebuffer.Width, Framebuffer.Height);
                }
                UpdateCursor();
                CursorY -= 16;
            }
        }

        private static void UpdateCursor()
        {
            int pos = (CursorY / 16 * Framebuffer.Width / 8) + (CursorX / 8);
            Native.Out8(0x3D4, 0x0F);
            Native.Out8(0x3D5, (byte)(pos & 0xFF));
            Native.Out8(0x3D4, 0x0E);
            Native.Out8(0x3D5, (byte)((pos >> 8) & 0xFF));
            UpdateCursorFramebuffer();
        }

        private static void UpdateCursorFramebuffer()
        {
            if (Framebuffer.VideoMemory != null && !Framebuffer.TripleBuffered)
            {
                ASC16.DrawChar('_',
                            CursorX,
                            CursorY,
                            0xFFFFFFFF
                            );
            }
        }

        public static void WriteLine(string s)
        {
            Write(s);
            WriteLine();
            s.Dispose();
        }

        public static void WriteLine(object o)
        {
            string s = o.ToString();
            Write(s);
            WriteLine();
            s.Dispose();
        }

        public static void WriteLine()
        {
            WriteFramebuffer(' ');
            OnWrite?.Invoke('\n');
            CursorX = 0;
            CursorY += 16;
            MoveUp();
            UpdateCursor();
        }

        public static void WriteAt(char chr, int x, int y)
        {
            CursorX = x * 8;
            CursorY = y * 16;
            Write(chr);
        }

        public static void Clear(uint background)
        {
            CursorX = 0;
            CursorY = 0;
            Framebuffer.Graphics.Clear(background);
            Framebuffer.Update();
        }
        public static void Clear()
        {
            Clear(0x0);
        }
    }
}
