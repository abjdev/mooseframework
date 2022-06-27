//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
    // A class responsible for running static constructors. The compiler will call into this
    // code to ensure static constructors run and that they only run once.
    [McgIntrinsics]
    internal static class ClassConstructorRunner
    {
#pragma warning disable IDE0051 // Remove unused private members
        private static unsafe IntPtr CheckStaticClassConstructionReturnNonGCStaticBase(ref StaticClassConstructionContext context, IntPtr nonGcStaticBase)
        {
            CheckStaticClassConstruction(ref context);
            return nonGcStaticBase;
        }

        private static unsafe object CheckStaticClassConstructionReturnGCStaticBase(ref StaticClassConstructionContext context, object gcStaticBase)
        {
            CheckStaticClassConstruction(ref context);
            return gcStaticBase;
        }
#pragma warning restore IDE0051 // Remove unused private members

        private static unsafe void CheckStaticClassConstruction(ref StaticClassConstructionContext context)
        {
            // Very simplified class constructor runner. In real world, the class constructor runner
            // would need to be able to deal with potentially multiple threads racing to initialize
            // a single class, and would need to be able to deal with potential deadlocks
            // between class constructors.

            if (context.initialized == 1)
            {
                return;
            }

            context.initialized = 1;

            // Run the class constructor.
            ((delegate*<void>)context.cctorMethodAddress)();
        }
    }
}
