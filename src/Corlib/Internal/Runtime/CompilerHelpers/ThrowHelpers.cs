//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using Internal.TypeSystem;

namespace Internal.Runtime.CompilerHelpers
{
    public static class ThrowHelpers
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static void ThrowInvalidProgramException(ExceptionStringID id) { }
        public static void ThrowInvalidProgramExceptionWithArgument(ExceptionStringID id, string methodName) { }
        public static void ThrowOverflowException() { }
        public static void ThrowIndexOutOfRangeException() { }
        public static void ThrowTypeLoadException(ExceptionStringID id, string className, string typeName) { }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}