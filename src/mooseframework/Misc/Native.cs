//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Runtime.InteropServices;
using moose.CPU;
using moose.Misc;

internal static unsafe class Native
{

    [DllImport("*")]
    public static extern CPUID* CPUID(int index);

    [DllImport("*")]
    public static extern ulong Rdtsc();

    [DllImport("*")]
    public static extern void Insb(ushort port, byte* data, ulong count);

    [DllImport("*")]
    public static extern void Insw(ushort port, ushort* data, ulong count);

    [DllImport("*")]
    public static extern void Insd(ushort port, uint* data, ulong count);

    [DllImport("*")]
    public static extern void Outsb(ushort port, byte* data, ulong count);

    [DllImport("*")]
    public static extern void Outsw(ushort port, ushort* data, ulong count);

    [DllImport("*")]
    public static extern void Outsd(ushort port, uint* data, ulong count);

    [DllImport("*")]
    public static extern ulong ReadCR2();

    [DllImport("*")]
    public static extern void Out8(uint port, byte value);

    [DllImport("*")]
    public static extern void Out16(uint port, ushort value);

    [DllImport("*")]
    public static extern void Out32(uint port, uint value);

    [DllImport("*")]
    public static extern byte In8(uint port);

    [DllImport("*")]
    public static extern ushort In16(uint port);

    [DllImport("*")]
    public static extern uint In32(uint port);

    [DllImport("*")]
    public static extern void Stosd(void* p, uint value, ulong count);

    [DllImport("*")]
    public static extern unsafe void Stosb(void* p, byte value, ulong count);

    [DllImport("*")]
    public static extern void Movsb(void* dest, void* source, ulong count);

    [DllImport("*")]
    public static extern void Movsd(uint* dest, uint* source, ulong count);

    [DllImport("*")]
    public static extern void Load_GDT(ref GDT.GDTDescriptor gdtr);

    [DllImport("*")]
    public static extern void Load_IDT(ref IDT.IDTDescriptor idtr);

    [DllImport("*")]
    public static extern unsafe void WriteCR3(ulong value);

    [DllImport("*")]
    public static extern void Hlt();

    [DllImport("*")]
    public static extern void Cli();

    [DllImport("*")]
    public static extern void Sti();

    [DllImport("*")]
    public static extern void Invlpg(ulong physicalAddress);

    [DllImport("*")]
    public static extern void Nop();

    [DllImport("*")]
    public static extern void Fxsave64(void* ptr);

    [DllImport("*")]
    public static extern void Fxrstor64(void* ptr);

    [DllImport("*")]
    public static extern ulong Rdmsr(ulong index);

    [DllImport("*")]
    public static extern void Wrmsr(ulong index, ulong value);
}