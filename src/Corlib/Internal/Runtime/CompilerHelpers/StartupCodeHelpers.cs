//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;
using System.Runtime;
using Internal.Runtime.CompilerServices;
using moose.Memory;

namespace Internal.Runtime.CompilerHelpers
{
    public unsafe class StartupCodeHelpers
    {
        [RuntimeExport("__imp_GetCurrentThreadId")]
        public static int Imp_GetCurrentThreadId()
        {
            return 0;
        }

        [RuntimeExport("__CheckForDebuggerJustMyCode")]
        public static int CheckForDebuggerJustMyCode()
        {
            return 0;
        }

        [RuntimeExport("__fail_fast")]
        private static void FailFast()
        {
            while (true)
            {
                ;
            }
        }

        [RuntimeExport("memset")]
        private static unsafe void MemSet(byte* ptr, int c, int count)
        {
            for (byte* p = ptr; p < ptr + count; p++)
            {
                *p = (byte)c;
            }
        }

        [RuntimeExport("memcpy")]
        private static unsafe void MemCpy(byte* dest, byte* src, ulong count)
        {
            for (ulong i = 0; i < count; i++)
            {
                dest[i] = src[i];
            }
        }

        [RuntimeExport("RhpFallbackFailFast")]
        private static void RhpFallbackFailFast()
        {
            while (true)
            {
                ;
            }
        }

        [RuntimeExport("RhpReversePInvoke2")]
#pragma warning disable IDE0060 // Remove unused parameter
        private static void RhpReversePInvoke2(IntPtr frame) { }

        [RuntimeExport("RhpReversePInvokeReturn2")]
        private static void RhpReversePInvokeReturn2(IntPtr frame) { }

        [RuntimeExport("RhpReversePInvoke")]
        private static void RhpReversePInvoke(IntPtr frame) { }

        [RuntimeExport("RhpReversePInvokeReturn")]
        private static void RhpReversePInvokeReturn(IntPtr frame) { }

        [RuntimeExport("RhpPInvoke")]
        private static void RhpPinvoke(IntPtr frame) { }

        [RuntimeExport("RhpPInvokeReturn")]
        private static void RhpPinvokeReturn(IntPtr frame) { }
#pragma warning restore IDE0060 // Remove unused parameter

        [RuntimeExport("RhpNewFast")]
        private static unsafe object RhpNewFast(EEType* pEEType)
        {
            uint size = pEEType->BaseSize;

            // Round to next power of 8
            if (size % 8 > 0)
            {
                size = ((size / 8) + 1) * 8;
            }

            IntPtr data = Allocator.Allocate(size);
            object obj = Unsafe.As<IntPtr, object>(ref data);
            Allocator.ZeroFill(data, size);
            *(IntPtr*)data = (IntPtr)pEEType;

            return obj;
        }



        [RuntimeExport("RhpNewArray")]
        public static unsafe object RhpNewArray(EEType* pEEType, int length)
        {
            ulong size = pEEType->BaseSize + ((ulong)length * pEEType->ComponentSize);

            // Round to next power of 8
            if (size % 8 > 0)
            {
                size = ((size / 8) + 1) * 8;
            }

            IntPtr data = Allocator.Allocate(size);
            object obj = Unsafe.As<IntPtr, object>(ref data);
            Allocator.ZeroFill(data, size);
            *(IntPtr*)data = (IntPtr)pEEType;

            byte* b = (byte*)data;
            b += sizeof(IntPtr);
            Allocator.MemoryCopy((IntPtr)b, (IntPtr)(&length), sizeof(int));

            return obj;
        }


        [RuntimeExport("RhpAssignRef")]
        private static unsafe void RhpAssignRef(void** address, void* obj)
        {
            *address = obj;
        }

        [RuntimeExport("RhpByRefAssignRef")]
        private static unsafe void RhpByRefAssignRef(void** address, void* obj)
        {
            *address = obj;
        }

        [RuntimeExport("RhpCheckedAssignRef")]
        private static unsafe void RhpCheckedAssignRef(void** address, void* obj)
        {
            *address = obj;
        }

        [RuntimeExport("RhpStelemRef")]
        private static unsafe void RhpStelemRef(Array array, int index, object obj)
        {
            fixed (int* n = &array._numComponents)
            {
                byte* ptr = (byte*)n;
                ptr += sizeof(void*);   // Array length is padded to 8 bytes on 64-bit
                ptr += index * array.m_pEEType->ComponentSize;  // Component size should always be 8, seeing as it's a pointer...
                IntPtr* pp = (IntPtr*)ptr;
                *pp = Unsafe.As<object, IntPtr>(ref obj);
            }
        }

        [RuntimeExport("RhTypeCast_IsInstanceOfClass")]
        public static unsafe object RhTypeCast_IsInstanceOfClass(EEType* pTargetType, object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (pTargetType == obj.m_pEEType)
            {
                return obj;
            }

            EEType* bt = obj.m_pEEType->RawBaseType;

            while (true)
            {
                if (bt == null)
                {
                    return null;
                }

                if (pTargetType == bt)
                {
                    return obj;
                }

                bt = bt->RawBaseType;
            }
        }


        public static void InitializeModules(IntPtr Modules)
        {
            for (int i = 0; ; i++)
            {
                if (((IntPtr*)Modules)[i].Equals(IntPtr.Zero))
                {
                    break;
                }

                ReadyToRunHeader* header = (ReadyToRunHeader*)((IntPtr*)Modules)[i];
                ModuleInfoRow* sections = (ModuleInfoRow*)(header + 1);

                if (header->Signature != ReadyToRunHeaderConstants.Signature)
                {
                    FailFast();
                }

                for (int k = 0; k < header->NumberOfSections; k++)
                {
                    if (sections[k].SectionId == ReadyToRunSectionType.GCStaticRegion)
                    {
                        InitializeStatics(sections[k].Start, sections[k].End);
                    }
                }
            }
            DateTime.DaysToMonth365 = new int[]{
                0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
            DateTime.DaysToMonth366 = new int[]{
                0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
        }

        private static unsafe void InitializeStatics(IntPtr rgnStart, IntPtr rgnEnd)
        {
            for (IntPtr* block = (IntPtr*)rgnStart; block < (IntPtr*)rgnEnd; block++)
            {
                IntPtr* pBlock = (IntPtr*)*block;
                long blockAddr = (long)*pBlock;

                if ((blockAddr & GCStaticRegionConstants.Uninitialized) == GCStaticRegionConstants.Uninitialized)
                {
                    object obj = RhpNewFast((EEType*)(blockAddr & ~GCStaticRegionConstants.Mask));

                    if ((blockAddr & GCStaticRegionConstants.HasPreInitializedData) == GCStaticRegionConstants.HasPreInitializedData)
                    {
                        IntPtr pPreInitDataAddr = *(pBlock + 1);
                        fixed (byte* p = &obj.GetRawData())
                        {
                            MemCpy(p, (byte*)pPreInitDataAddr, obj.GetRawDataSize());
                        }
                    }

                    IntPtr handle = Allocator.Allocate((ulong)sizeof(IntPtr));
                    *(IntPtr*)handle = Unsafe.As<object, IntPtr>(ref obj);
                    *pBlock = handle;
                }
            }
        }
    }
}