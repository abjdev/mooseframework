//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace moose.Networking
{
    public abstract unsafe class NIC
    {
        public abstract void Send(byte* Data, int Length);
    }
}