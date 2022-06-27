//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System;

namespace Internal.Runtime.CompilerHelpers
{
    public static class SynchronizedMethodHelpers
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private static void MonitorEnter(object obj, ref bool lockTaken) { lockTaken = true; }

        private static void MonitorExit(object obj, ref bool lockTaken) { lockTaken = false; }

        private static void MonitorEnterStatic(IntPtr pEEType, ref bool lockTaken) { lockTaken = true; }

        private static void MonitorExitStatic(IntPtr pEEType, ref bool lockTaken) { lockTaken = false; }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0051 // Remove unused private members
    }
}
