using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UmbIntranetRestrict.Support
{
    public static class IPAddressExtensions
    {
        /// <summary>
        /// Tests if IP address is in allowed network.
        /// </summary>
        /// <param name="testAddress">IP address to test.</param>
        /// <param name="allowedNetworks">Allowed networks.</param>
        /// <returns>True if IP address is in an allowed network.</returns>
        public static bool IsInAllowedNetwork(this IPAddress testAddress, IEnumerable<IPNetwork> allowedNetworks)
        {
            return allowedNetworks.Any(x => IPNetwork.Contains(x, testAddress));
        }
    }
}
