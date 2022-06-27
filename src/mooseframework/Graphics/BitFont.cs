//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
#define AlphaBitfont
using System;
using System.Collections.Generic;
using moose.Misc;

namespace moose.Graphics
{
    public class BitFontDescriptor
    {
        public string Charset;
        public byte[] Raw;
        public int Size;
        public string Name;

        public BitFontDescriptor(string aName, string aCharset, byte[] aRaw, int aSize)
        {
            Charset = aCharset;
            Raw = aRaw;
            Size = aSize;
            Name = aName;
        }
    }

    public static class BitFont
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static List<BitFontDescriptor> RegisteredBitFont;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static void Initialize()
        {
            RegisteredBitFont = new List<BitFontDescriptor>();
        }

        public static void RegisterBitFont(BitFontDescriptor bitFontDescriptor)
        {
            RegisteredBitFont.Add(bitFontDescriptor);
        }

        private static bool AtEdge = false;

        private static int DrawChar(byte[] raw, int size, int size8, uint color, int index, int X, int Y, bool AntiAliasing = true)
        {
            if (index < 0)
            {
                return size / 2;
            }

            int MaxX = 0;
            int SizePerFont = size * size8 * index;
            AtEdge = false;

            for (int h = 0; h < size; h++)
            {
                for (int aw = 0; aw < size8; aw++)
                {
                    for (int ww = 0; ww < 8; ww++)
                    {
                        if ((raw[SizePerFont + (h * size8) + aw] & (0x80 >> ww)) != 0)
                        {
                            int max = (aw * 8) + ww;

                            int x = X + max;
                            int y = Y + h;

                            Framebuffer.Graphics.DrawPoint(x, y, color);

                            if (AntiAliasing && AtEdge)
                            {
                                int threshhold = 2;
                                int maxalpha = 150;

                                for (int ax = -threshhold; ax <= threshhold; ax++)
                                {
                                    if (ax == 0)
                                    {
                                        continue;
                                    }

                                    int alpha = Math.Abs(((-maxalpha) * ax * ax / (threshhold * threshhold)) + maxalpha);
                                    Framebuffer.Graphics.DrawPoint(x + ax, y, (color & ~0xFF000000) | ((uint)alpha << 24));
                                }
                            }

                            AtEdge = false;

                            if (max > MaxX)
                            {
                                MaxX = max;
                            }
                        } else
                        {
                            AtEdge = true;
                        }
                    }
                }
            }

            return MaxX;
        }

        private static int CalculateChar(byte[] raw, int size, int size8, uint color, int index)
        {
            if (index < 0)
            {
                return size / 2;
            }

            int MaxX = 0;
            int SizePerFont = size * size8 * index;
            AtEdge = false;

            for (int h = 0; h < size; h++)
            {
                for (int aw = 0; aw < size8; aw++)
                {
                    for (int ww = 0; ww < 8; ww++)
                    {
                        if ((raw[SizePerFont + (h * size8) + aw] & (0x80 >> ww)) != 0)
                        {
                            int max = (aw * 8) + ww;

                            if (max > MaxX)
                            {
                                MaxX = max;
                            }
                        } else
                        {
                            AtEdge = true;
                        }
                    }
                }
            }

            return MaxX;
        }

        private static BitFontDescriptor GetBitFontDescriptor(string FontName)
        {
            for (int i = 0; i < RegisteredBitFont.Count; i++)
            {
                if (RegisteredBitFont[i].Name == FontName)
                {
                    return RegisteredBitFont[i];
                }
            }

            Panic.Error("BitFont Descriptor Not Found");
            return null;
        }
        public static int MeasureChar(string FontName, char c)
        {
            BitFontDescriptor bitFontDescriptor = GetBitFontDescriptor(FontName);

            int Size = bitFontDescriptor.Size;
            int Size8 = Size / 8;

            return CalculateChar(bitFontDescriptor.Raw, Size, Size8, 0, bitFontDescriptor.Charset.IndexOf(c));
        }
        public static int MeasureString(string FontName, string Text, int Divide = 0)
        {
            BitFontDescriptor bitFontDescriptor = GetBitFontDescriptor(FontName);

            int Size = bitFontDescriptor.Size;
            int Size8 = Size / 8;

            int UsedX = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];
                UsedX += CalculateChar(bitFontDescriptor.Raw, Size, Size8, 0, bitFontDescriptor.Charset.IndexOf(c)) + 2 + Divide;
            }

            return UsedX - (2 + Divide);
        }

        public static int DrawString(string FontName, uint color, string Text, int X, int Y, int LineWidth = -1, bool AntiAlising = true, int Divide = 0)
        {
            BitFontDescriptor bitFontDescriptor = GetBitFontDescriptor(FontName);

            int Size = bitFontDescriptor.Size;
            int Size8 = Size / 8;

            int Line = 0;
            int UsedX = 0;
            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];
                if (c == '\n' || (LineWidth != -1 && UsedX + bitFontDescriptor.Size > LineWidth))
                {
                    Line++;
                    UsedX = 0;
                    continue;
                }
                UsedX += DrawChar(bitFontDescriptor.Raw, Size, Size8, color, bitFontDescriptor.Charset.IndexOf(c), UsedX + X, Y + (bitFontDescriptor.Size * Line), AntiAlising) + 2 + Divide;
            }

            return UsedX - (2 + Divide);
        }
    }
}
