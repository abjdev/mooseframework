//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using moose.CPU;
using moose.Graphics;

namespace moose.Misc
{
    public static class Panic
    {
        public static void Error(string msg, bool skippable = false)
        {
            LocalAPIC.SendAllInterrupt(0xFD);

            IDT.Disable();
            /*Framebuffer.TripleBuffered = false;
            Console.Clear(0x0000FF);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write("PANIC: ");
            Console.WriteLine(msg);
            Console.WriteLine("All CPU(s) are halted Now!");*/
            if (!skippable)
            {
                Framebuffer.Update();
                for (; ; )
                {
                    ;
                }
            }
        }
    }
}
