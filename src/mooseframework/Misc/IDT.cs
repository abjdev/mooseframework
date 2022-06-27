//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using Internal.Runtime.CompilerServices;
using moose.CPU;
using moose.Drivers;
using moose.Graphics;
using moose.Networking;
using moose.Timing;

namespace moose.Misc
{
    public static class IDT
    {
        [DllImport("*")]
        private static extern unsafe void set_idt_entries(void* idt);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IDTEntry
        {
            public ushort BaseLow;
            public ushort Selector;
            public byte Reserved0;
            public byte Type_Attributes;
            public ushort BaseMid;
            public uint BaseHigh;
            public uint Reserved1;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IDTDescriptor
        {
            public ushort Limit;
            public ulong Base;
        }

        private static IDTEntry[] idt;
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static IDTDescriptor idtr;
#pragma warning restore CA2211 // Non-constant fields should not be visible


        public static bool Initialized { get; private set; }


        public static unsafe bool Initialize()
        {
            idt = new IDTEntry[256];

            // TODO: Figure out a way to do this in C#
            set_idt_entries(Unsafe.AsPointer(ref idt[0]));

            fixed (IDTEntry* _idt = idt)
            {
                // Fill IDT descriptor
                idtr.Limit = (ushort)((sizeof(IDTEntry) * 256) - 1);
                idtr.Base = (ulong)_idt;
            }

            Native.Load_IDT(ref idtr);

            Initialized = true;
            return true;
        }

        public static void Enable()
        {
            Native.Sti();
        }

        public static void Disable()
        {
            Native.Cli();
        }

        //interrupts_asm.asm line 39
        [RuntimeExport("exception_handler")]
        public static unsafe void ExceptionHandler(int code, IDTStackGeneric* stack)
        {
            Panic.Error($"Kernel panic on CPU{SMP.ThisCPU}", true);
            InterruptReturnStack* irs = code switch
            {
                0x00 or 0x01 or 0x02 or 0x03 or 0x04 or 0x05 or 0x06 or 0x07 or 0x09 or 0x0F or 0x10 or 0x12 or 0x13 or 0x14 or 0x16 or 0x17 or 0x18 or 0x19 or 0x1A or 0x1B or 0x1C or 0x1F => (InterruptReturnStack*)((byte*)stack + sizeof(RegistersStack)),
                _ => (InterruptReturnStack*)((byte*)stack + sizeof(RegistersStack) + sizeof(ulong)),
            };
            Console.WriteLine($"RIP: 0x{stack->irs.rip.ToString("x2")}");
            Console.WriteLine($"Code Segment: 0x{stack->irs.cs.ToString("x2")}");
            Console.WriteLine($"RFlags: 0x{stack->irs.rflags.ToString("x2")}");
            Console.WriteLine($"RSP: 0x{stack->irs.rsp.ToString("x2")}");
            Console.WriteLine($"Stack Segment: 0x{stack->irs.ss.ToString("x2")}");
            string v = code switch
            {
                0 => "DIVIDE BY ZERO",
                1 => "SINGLE STEP",
                2 => "NMI",
                3 => "BREAKPOINT",
                4 => "OVERFLOW",
                5 => "BOUNDS CHECK",
                6 => "INVALID OPCODE",
                7 => "COPR UNAVAILABLE",
                8 => "DOUBLE FAULT",
                9 => "COPR SEGMENT OVERRUN",
                10 => "INVALID TSS",
                11 => "SEGMENT NOT FOUND",
                12 => "STACK EXCEPTION",
                13 => "GENERAL PROTECTION",
                14 => (Native.ReadCR2() >> 5) < 0x1000 ? "NULL POINTER" : "PAGE FAULT",
                16 => "COPR ERROR",
                _ => "UNKNOWN"
            };
            Console.WriteLine($"Description: {v}");
            if (SMP.ThisCPU == 0)
            {
                Framebuffer.Update();
            }
            //This method is unreturnable
        }

        public struct RegistersStack
        {
            public ulong rax;
            public ulong rcx;
            public ulong rdx;
            public ulong rbx;
            public ulong rsi;
            public ulong rdi;
            public ulong r8;
            public ulong r9;
            public ulong r10;
            public ulong r11;
            public ulong r12;
            public ulong r13;
            public ulong r14;
            public ulong r15;
        }

        //https://os.phil-opp.com/returning-from-exceptions/
        public struct InterruptReturnStack
        {
            public ulong rip;
            public ulong cs;
            public ulong rflags;
            public ulong rsp;
            public ulong ss;
        }

        public struct IDTStackGeneric
        {
            public RegistersStack rs;
            public ulong errorCode;
            public InterruptReturnStack irs;
        }

        [RuntimeExport("irq_handler")]
        public static unsafe void IRQHandler(int irq, IDTStackGeneric* stack)
        {
            if (irq == 0xFD)
            {
                Native.Cli();
                Native.Hlt();
                for (; ; )
                {
                    Native.Hlt();
                }
            }
            //For main processor
            if (SMP.ThisCPU == 0)
            {
                switch (irq)
                {
                    case 0x20:
                        Timer.OnInterrupt();
                        break;
                    case 0x21:
                        byte b = Native.In8(0x60);
                        PS2Keyboard.ProcessKey(b);
                        break;
                    case 0x2C:
                        PS2Mouse.OnInterrupt();
                        break;
                }
                if (irq == RTL8139.IRQ)
                {
                    RTL8139.OnInterrupt();
                }
                if (irq == Intel8254X.IRQ)
                {
                    Intel8254X.OnInterrupt();
                }
                if (irq == 0x20)
                {
                    ThreadPool.Schedule(stack);
                }
                Interrupts.HandleInterrupt(irq);
            }
            Interrupts.EndOfInterrupt((byte)irq);
        }
    }
}