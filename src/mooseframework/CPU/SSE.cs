//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Runtime.InteropServices;

namespace moose.CPU
{
    internal static class SSE
    {
        [DllImport("*")]
        public static extern void enable_sse();
    }
}
