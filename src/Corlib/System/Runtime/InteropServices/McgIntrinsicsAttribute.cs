//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Runtime.InteropServices
{
    // Custom attribute that marks a class as having special "Call" intrinsics.
    [AttributeUsage(AttributeTargets.All)]
    internal class McgIntrinsicsAttribute : Attribute { }
}