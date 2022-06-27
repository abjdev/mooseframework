//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
#if HasGC
using System;

namespace moose.Memory
{
    public static unsafe class GC
    {
        [Flags]
        public enum Flags : byte
        {
            Fixed = 0b1000_0000,
            NeedsToBeCollected = 0b0100_0000,
        }

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static bool AllowCollect;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static void Collect()
        {
            lock (null)
            {
                ulong memSaved = 0;
                ulong counter = 0;
                for (ulong i = 0; i < Allocator.NumPages; i++)
                {
                    if (Allocator._Info.Pages[i] == 0)
                    {
                        continue;
                    }

                    if (Allocator._Info.Pages[i] == Allocator.PageSignature)
                    {
                        continue;
                    }

                    if (((Flags)Allocator._Info.GCInfos[i]).HasFlag(Flags.Fixed))
                    {
                        continue;
                    }

                    ulong addr = (ulong)(Allocator._Info.Start + (i * Allocator.PageSize));
                    ulong* page = PageTable.GetPage(addr);
                    if (BitHelpers.IsBitSet(*page, 5)) //Accessed bit
                    {
                        *page &= ~(1UL << 5);
                        Allocator._Info.GCInfos[i] = 0;
                    } else
                    {
                        Allocator._Info.GCInfos[i]++;
                    }

                    ulong pages = Allocator._Info.Pages[i];

                    if (((Flags)Allocator._Info.GCInfos[i]).HasFlag(Flags.NeedsToBeCollected))
                    {
                        counter++;
                        memSaved += Allocator.Free((IntPtr)addr);
                    }

                    i += pages;
                }
                if (memSaved != 0)
                {
                    Console.Write("GC Collected: ");
                    Console.Write(counter.ToString());
                    Console.Write(" Unused Handle(s) ");
                    Console.Write((memSaved / 1048576).ToString());
                    Console.WriteLine("MiB");
                }
            }
        }
    }
}
#endif