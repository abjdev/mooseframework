//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Windows.Forms;
using moose.Misc;
using static Native;

namespace moose.Drivers
{
    public static class PS2Mouse
    {
        private const byte Data = 0x60;
        private const byte Command = 0x64;

        private const byte SetDefaults = 0xF6;
        private const byte EnableDataReporting = 0xF4;

        private static int Phase = 0;
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static byte[] MData;
#pragma warning restore CA2211 // Non-constant fields should not be visible
        private static int aX;
        private static int aY;

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static int ScreenWidth = 0;
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static int ScreenHeight = 0;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static void Initialize()
        {
            MData = new byte[3];
            Interrupts.EnableInterrupt(0x2c);

            byte _status;

            Hlt();
            Out8(Command, 0xA8);

            Hlt();
            Out8(Command, 0x20);
            Hlt();
            _status = (byte)(In8(0x60) | 3);
            Hlt();
            Out8(Command, 0x60);
            Hlt();
            Out8(Data, _status);

            WriteRegister(SetDefaults);
            WriteRegister(EnableDataReporting);

            WriteRegister(0xF2);

            WriteRegister(0xF3);
            WriteRegister(200);

            WriteRegister(0xF2);

            Control.MouseButtons = MouseButtons.None;
        }

        public static void WriteRegister(byte value)
        {
            Hlt();
            Out8(Command, 0xD4);
            Hlt();
            Out8(Data, value);

            ReadRegister();
        }

        public static byte ReadRegister()
        {
            Hlt();
            return In8(Data);
        }

        public static void OnInterrupt()
        {
            byte D = In8(Data);

            if (Phase == 0)
            {
                if (D == 0xfa)
                {
                    Phase = 1;
                }
            } else if (Phase == 1)
            {
                if ((D & 8) == 8)
                {
                    MData[0] = D;
                    Phase = 2;
                }
            } else if (Phase == 2)
            {
                MData[1] = D;
                Phase = 3;
            } else if (Phase == 3)
            {
                MData[2] = D;
                Phase = 1;

                MData[0] &= 0x07;

                Control.MouseButtons = MouseButtons.None;
                if (MData[0] == 0x01)
                {
                    Control.MouseButtons = MouseButtons.Left;
                }
                if (MData[0] == 0x02)
                {
                    Control.MouseButtons = MouseButtons.Right;
                }

                aX = MData[1] > 127 ? -(255 - MData[1]) : MData[1];

                aY = MData[2] > 127 ? -(255 - MData[2]) : MData[2];

                Control.MousePosition.X += aX;
                Control.MousePosition.Y -= aY;
            }
        }
    }
}