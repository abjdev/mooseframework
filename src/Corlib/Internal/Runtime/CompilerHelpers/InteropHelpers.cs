//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Runtime.InteropServices;
using moose;

namespace Internal.Runtime.CompilerHelpers
{
    public static class InteropHelpers
    {
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct MethodFixupCell
        {
            public IntPtr Target;
            public IntPtr MethodName;
            public ModuleFixupCell* Module;
            public CharSet CharSetMangling;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct ModuleFixupCell
        {
            public IntPtr Handle;
            public IntPtr ModuleName;
            public EETypePtr CallingAssemblyType;
            public uint DllImportSearchPathAndCookie;
        }

        public static unsafe IntPtr ResolvePInvoke(MethodFixupCell* pCell)
        {
            Console.Write("Method Name: ");
            Console.WriteLine(string.FromASCII(pCell->Module->ModuleName, strings.strlen((byte*)pCell->Module->ModuleName)));
            //Return the pointer of method
            return (IntPtr)(delegate*<void>)&pCell->Module->Handle;
        }

        public static unsafe byte* StringToAnsiString(string str, bool bestFit, bool throwOnUnmappableChar)
        {
            //String will become char* if we use DllImport
            //No Ansi support, Return unicode
            fixed (char* ptr = str)
            {
                return (byte*)ptr;
            }
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static unsafe void CoTaskMemFree(void* p)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            //TO-DO
        }
    }
}
