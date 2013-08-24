/**
 * Code taken from http://blogs.msdn.com/b/knom/archive/2008/12/31/ip-address-calculations-with-c-subnetmasks-networks.aspx
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace UmbIntranetRestrict.Support
{
    public static class IPAddressExtensions
    {
        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        /// <summary>
        /// Tests is a given Ip address is in the same subnet as a specified subnet/mask.
        /// </summary>
        /// <param name="testAddress">Ip address to test.</param>
        /// <param name="allowedAddress">Allowed IP to check against.</param>
        /// <param name="subnetMask">Subnet for Allowed IP to check against.</param>
        /// <returns></returns>
        public static bool IsInSameSubnet(this IPAddress testAddress, IPAddress allowedAddress, IPAddress subnetMask)
        {
            // Check to see if subnet is specified as 0.0.0.0 or 255.255.255.255, which we will use to refer to a single IP address.
            if ((subnetMask.ToString() == "0.0.0.0") || (subnetMask.ToString() == "255.255.255.255"))
            {
                // See if addresses match.
                return testAddress.Equals(allowedAddress);
            }

            IPAddress network1 = allowedAddress.GetNetworkAddress(subnetMask);
            IPAddress network2 = testAddress.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }

        /// <summary>
        /// Tests is a given Ip address is in the same subnet as a specified subnet/mask.
        /// </summary>
        /// <param name="testAddress">Ip address to test.</param>
        /// <param name="allowedAddresses">Allowed IPs to check against.</param>
        /// <param name="subnetMasks">Subnets for allowed IPs to check against.</param>
        /// <returns></returns>
        public static bool IsInSameSubnet(this IPAddress testAddress, List<IPAddress> allowedAddresses, List<IPAddress> subnetMasks)
        {
            // Loop to process each IP/subnet.
            for (int ipCount = 0; ipCount < allowedAddresses.Count; ipCount++)
            {
                // Test to see if this subnet matches.
                if (testAddress.IsInSameSubnet(allowedAddresses[ipCount], subnetMasks[ipCount]))
                {
                    // Address is allowed.
                    return true;
                }
            }

            // We've exhausted all IP addresses and nothing matches.
            return false;
        }
    }
}
