//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace System.Drawing
{
    public class Image
    {
        public uint[] RawData;
        public int Bpp;
        public uint Width;
        public uint Height;

        public Image(int width, int height)
        {
            Width = (uint)width;
            Height = (uint)height;
            Bpp = 4;
            RawData = new uint[width * height];
        }
        public Image(uint width, uint height)
        {
            Width = width;
            Height = height;
            Bpp = 4;
            RawData = new uint[width * height];
        }
        public Image()
        {

        }

        public uint GetPixel(int X, int Y)
        {
            return RawData[(Y * Width) + X];
        }

        public Image ResizeImage(uint NewWidth, uint NewHeight)
        {
            if (NewWidth == 0 || NewHeight == 0)
            {
                return new Image();
            }

            uint w1 = Width, h1 = Height;
            uint[] temp = new uint[NewWidth * NewHeight];

            uint x_ratio = ((w1 << 16) / NewWidth) + 1, y_ratio = ((h1 << 16) / NewHeight) + 1;
            uint x2, y2;

            for (uint i = 0; i < NewHeight; i++)
            {
                for (uint j = 0; j < NewWidth; j++)
                {
                    x2 = (j * x_ratio) >> 16;
                    y2 = (i * y_ratio) >> 16;
                    temp[(i * NewWidth) + j] = RawData[(y2 * w1) + x2];
                }
            }

            Image image = new()
            {
                Width = NewWidth,
                Height = NewHeight,
                Bpp = Bpp,
                RawData = temp
            };

            return image;
        }
        public Image ResizeImage(int NewWidth, int NewHeight)
        {
            return ResizeImage((uint)NewWidth, (uint)NewHeight);
        }
        public override void Dispose()
        {
            RawData.Dispose();
            base.Dispose();
        }
    }
}