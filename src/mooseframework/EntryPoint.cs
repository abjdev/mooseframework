//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
//#define NETWORK
using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using Internal.Runtime.CompilerHelpers;
using moose;
using moose.Audio;
using moose.CPU;
using moose.Drivers;
using moose.FileSystem;
using moose.Graphics;
using moose.Memory;
using moose.Misc;
using moose.Power;
using moose.Timing;

internal static unsafe class EntryPoint
{
    [RuntimeExport("Entry")]
    public static void Entry(MultibootInfo* Info, IntPtr Modules, IntPtr Trampoline)
    {
        Allocator.Initialize((IntPtr)0x20000000);
        StartupCodeHelpers.InitializeModules(Modules);
#if HasGC
        GC.AllowCollect = false;
#endif
        PageTable.Initialize();
        ASC16.Initialize();
        VBE.Initialize((VBEInfo*)Info->VBEInfo);
        Console.Initialize();
        IDT.Disable();
        Interrupts.Initialize();
        GDT.Initialize();
        IDT.Initialize();
        IDT.Enable();
        SSE.enable_sse();
        //AVX.init_avx();
        ACPI.Initialize();
#if UseAPIC
        PIC.Disable();
        LocalAPIC.Initialize();
        IOAPIC.Initialize();
        HPET.Initialize();
#else
        PIC.Enable();
#endif
        PS2Keyboard.Initialize();
        Interrupts.EnableInterrupt(0x21);
        Serial.Initialize();
        PIT.Initialize();
        PS2Mouse.Initialize();
        SMBIOS.Initialize();
        PCI.Initialize();
        IDE.Initialize();
        SATA.Initialize();
        ThreadPool.Initialize();
        Console.WriteLine("[SMP] Trampoline: 0x" + ((ulong)Trampoline).ToString("x2"));
        Native.Movsb((byte*)SMP.Trampoline, (byte*)Trampoline, 512);
        SMP.Initialize((uint)SMP.Trampoline);

#if HasGC
        GC.AllowCollect = true;
#endif
        _ = new Ramdisk((IntPtr)Info->Mods[0]);
        _ = new FATFS();
        Audio.Initialize();
        AC97.Initialize();

        KMain();
    }
    [DllImport("*")]
    public static extern void KMain();
}