//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
using System.Net;

namespace moose.Networking
{
    public static class Network
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static byte[] MAC;
        public static byte[] IP;
        public static byte[] Mask;
        public static byte[] Boardcast;
        public static byte[] Gateway;
        public static NIC Controller;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public delegate void OnDataHandler(byte[] buffer);

        public static void Initialize(IPAddress IPAddress, IPAddress GatewayAddress, IPAddress SubnetMask)
        {
            Boardcast = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            Gateway = GatewayAddress.Address;
            Mask = SubnetMask.Address;
            IP = IPAddress.Address;
            UDP.Clients = new();
            ARP.Initialize();
            TCP.Clients = new();
        }
    }
}
