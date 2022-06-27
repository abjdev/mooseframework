//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace System.Runtime
{
    // Custom attribute that the compiler understands that instructs it
    // to export the method under the given symbolic name.
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class RuntimeExportAttribute : Attribute
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public RuntimeExportAttribute(string entry) { }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}