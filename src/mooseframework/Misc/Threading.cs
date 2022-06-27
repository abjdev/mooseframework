//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Internal.Runtime.CompilerServices;
using moose.CPU;
using moose.Memory;
using moose.Misc;
using moose.Power;

namespace System.Threading
{
    public unsafe class Thread
    {
        public bool Terminated;
        public IDT.IDTStackGeneric* Stack;
        public ulong SleepingTime;
        public int RunOnWhichCPU;

        public Thread(delegate*<void> method, ulong stack_size = 16384)
        {
            Stack = (IDT.IDTStackGeneric*)Allocator.Allocate((ulong)sizeof(IDT.IDTStackGeneric));

            Stack->irs.cs = 0x08;
            Stack->irs.ss = 0x10;
            Stack->irs.rsp = ((ulong)Allocator.Allocate(stack_size)) + stack_size;

            Stack->irs.rsp -= 8;
            *(ulong*)Stack->irs.rsp = (ulong)(delegate*<void>)&ThreadPool.Terminate;

            Stack->irs.rflags = 0x202;

            Stack->irs.rip = (ulong)method;

            Terminated = false;

            SleepingTime = 0;
        }

        public static void Sleep(ulong ms)
        {
            ThreadPool.Threads[ThreadPool.Index].SleepingTime = ms;
        }

        public static void Sleep(int Index, ulong ms)
        {
            ThreadPool.Threads[Index].SleepingTime = ms;
        }
        public Thread Start()
        {
            //Bootstrap CPU
            RunOnWhichCPU = 0;
            ThreadPool.Threads.Add(this);
            return this;
        }

        public Thread Start(int cpu)
        {
            RunOnWhichCPU = cpu;
            ThreadPool.Threads.Add(this);
            return this;
        }
    }
    internal static unsafe class ThreadPool
    {
        public static List<Thread> Threads;
        public static bool Initialized = false;
        public static bool Locked = false;
        public static long Locker = 0;
        internal static int Index
        {
            get => ThreadIndexes[SMP.ThisCPU];
            set => ThreadIndexes[SMP.ThisCPU] = value;
        }

        public static void Initialize()
        {
            Native.Cli();
            //Bootstrap CPU
            if (SMP.ThisCPU == 0)
            {
                byte size = 0;
                for (int i = 0; i < ACPI.LocalAPIC_CPUIDs.Count; i++)
                {
                    if (ACPI.LocalAPIC_CPUIDs[i] > size)
                    {
                        size = ACPI.LocalAPIC_CPUIDs[i];
                    }
                }

                ThreadIndexes = new int[size + 1];
                Locked = false;
                Initialized = false;
                Threads = new();
                new Thread(&IdleThread).Start();
                Initialized = true;
            }
            //Application CPU
            else
            {
                new Thread(&IdleThread).Start((int)SMP.ThisCPU);
            }
            Native.Sti();
            _int20h();
        }
        public static void Terminate()
        {
            Threads[Index].Terminated = true;
            _int20h();
            Panic.Error("[Threading] Termination Failed");
        }

        [DllImport("*")]
        private static extern void _int20h();

        private static void IdleThread()
        {
            for (; ; )
            {
                Native.Hlt();
            }
        }

        private static int[] ThreadIndexes;
        public static bool CanLock => Unsafe.As<bool, ulong>(ref Initialized);

        public static void Lock()
        {
            Locker = SMP.ThisCPU;
            Locked = true;
            LocalAPIC.SendAllInterruptIncludingSelf(0x20);
        }

        public static void UnLock()
        {
            Locked = false;
        }
        public static void Schedule(IDT.IDTStackGeneric* stack)
        {
            if (!Initialized)
            {
                return;
            }

            if (Locked && Locker != SMP.ThisCPU)
            {
                while (Locked)
                {
                    Native.Nop();
                }

                return;
            }

            if (Locked && Locker == SMP.ThisCPU)
            {
                return;
            }

            if (SMP.ThisCPU == 0)
            {
                for (int i = 0; i < Threads.Count; i++)
                {
                    if (Threads[i].SleepingTime > 0)
                    {
                        Threads[i].SleepingTime--;
                    }
                }
            }

            for (; ; )
            {
                if (
                   !Threads[Index].Terminated &&
                    Threads[Index].RunOnWhichCPU == SMP.ThisCPU
                    )
                {
                    Native.Movsb(Threads[Index].Stack, stack, (ulong)sizeof(IDT.IDTStackGeneric));
                    break;
                }
                Index = (Index + 1) % Threads.Count;
            }

            do
            {
                Index = (Index + 1) % Threads.Count;
            } while
            (
                Threads[Index].Terminated ||
                (Threads[Index].SleepingTime > 0) ||
                Threads[Index].RunOnWhichCPU != SMP.ThisCPU
            );

            Native.Movsb(stack, Threads[Index].Stack, (ulong)sizeof(IDT.IDTStackGeneric));
        }
    }
}
