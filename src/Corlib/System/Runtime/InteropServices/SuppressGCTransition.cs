//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class SuppressGCTransitionAttribute : Attribute
    {
        public SuppressGCTransitionAttribute()
        {
        }
    }
}