//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
//#define Network

#if Network
using System.Net;
using moose.NET;
#endif
using System.Runtime;
using moose.Drivers;
using moose.Graphics;

namespace Kernel
{
    public unsafe class Program
    {
        private static void Main() { }

        /*
         * Minimum system requirement:
         * 1024MiB of RAM
         * Memory Map:
         * 256 MiB - 512MiB   -> System
         * 512 MiB - ∞     -> Free to use
         */
        [RuntimeExport("KMain")]
#pragma warning disable IDE0051 // Remove unused private members
        private static void KMain()
#pragma warning restore IDE0051 // Remove unused private members
        {
            #region Network
            ///
            /// How to use network
            /// 1. Install OpenVPN's Windows tap driver
            /// 2. Open "Control Panel\Network and Internet\Network Connections" in the Windows Control Panel.
            /// 3. Rename the newly created network adapter to "tap" in the Windows Control Panel.
            /// 4. Ctrl-Click your network adapter and the "tap" network adapter, then right-click on the "tap" network adapter and click "Bridge Connections"
            /// 5. The network has been successfully linked.
            /// To Un-Bridge the connections, Click on "Network Bridge" and then click at the top of the window "Delete this connection". ( Do this when you are not using the network as it slows down your internet connection )
#if Network
            Network.Initialize(IPAddress.Parse(192, 168, 137, 188), IPAddress.Parse(192, 168, 137, 1), IPAddress.Parse(255, 255, 255, 0));
            //Make sure this IP is pointing your gateway
            TcpClient client = TcpClient.Connect(IPAddress.Parse(192, 168, 137, 1), 80);
            client.OnData += Client_OnData;
            client.Send(ToASCII("GET / HTTP/1.1\r\nHost: 192.168.1.1\r\nUser-Agent: Mozilla/4.0 (compatible; moose framework)\r\n\r\n"));
            for (; ; )
            {
                Native.Hlt();
            }
#endif
            #endregion
            Framebuffer.TripleBuffered = true;

            if (PCI.GetDevice(0x15AD, 0x0405) != null)
            {
                Framebuffer.Graphics = new VMWareSVGAIIGraphics();
            }
            ASC16.DrawChar('W', 10, 10, 0xFF0000);
            ASC16.DrawChar('E', 18, 26, 0xFFA500);
            ASC16.DrawChar('L', 26, 42, 0xFFFF00);
            ASC16.DrawChar('C', 34, 58, 0x00FF00);
            ASC16.DrawChar('O', 42, 74, 0x0000FF);
            ASC16.DrawChar('M', 50, 90, 0x800080);
            ASC16.DrawString("E To moose.", 58, 106, 0xFFFFFF);
            Framebuffer.Graphics.Update();

            for (; ; )
            {

            }
        }

#if Network
        private static void Client_OnData(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write((char)data[i]);
            }
            Console.WriteLine();
        }

        public static byte[] ToASCII(string s)
        {
            byte[] buffer = new byte[s.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)s[i];
            }

            return buffer;
        }
#endif
    }
}