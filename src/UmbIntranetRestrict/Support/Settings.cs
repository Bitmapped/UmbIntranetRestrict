using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Configuration;
using System.Configuration;

namespace UmbIntranetRestrict.Support
{
    public class Settings
    {
        // Define constants.
        private const string AppKey_IpAddress = "IntranetRestrict:IpAddress";
        private const string AppKey_SubnetMask = "IntranetRestrict:SubnetMask";
        private const string AppKey_IPNetwork = "IntranetRestrict:IpNetwork";
        private const string AppKey_UnauthorizedPageId = "IntranetRestrict:UnauthorizedPageId";

        /// <summary>
        /// Load settings from configuration file.
        /// </summary>
        public Settings()
        {
            // Load values from config files.
            this.AllowedIpNetworks = this.ConfigLoadIpNetworks();
            this.UnauthorizedPageId = this.ConfigLoadUnauthorizedPageId();
        }

        /// <summary>
        /// Allowed IP networks.
        /// </summary>
        public IEnumerable<IPNetwork> AllowedIpNetworks { get; private set; }
        
        /// <summary>
        /// PageId for redirecting unauthorized users.
        /// </summary>
        public int UnauthorizedPageId { get; private set; }

        /// <summary>
        /// Access IpAddress specified for IntranetRestrict.
        /// </summary>
        private List<IPAddress> ConfigLoadAddresses(string key)
        {
            // Return empty list if there are no values.
            if (WebConfigurationManager.AppSettings[key] == null)
            {
                return new List<IPAddress>();
            }

            try
            {
                // Split IP address into multiple parts.
                var ipAddresses = WebConfigurationManager.AppSettings[key]
                    .Split(',')
                    .Select(x => x.Trim())
                    .Select(x => IPAddress.Parse(x))
                    .ToList();

                return ipAddresses;
            }
            catch
            {
                throw new ConfigurationErrorsException("Value for " + key + " not correctly specified.");
            }
        }

        /// <summary>
        /// Loads IP networks based on provided IP addresses and IP networks.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IPNetwork> ConfigLoadIpNetworks()
        {
            // Store Ip networks.
            var ipNetworks = new List<IPNetwork>();

            // Load IP addresses and subnets.
            var ipAddresses = ConfigLoadAddresses(AppKey_IpAddress);
            var subnetMasks = ConfigLoadAddresses(AppKey_SubnetMask);

            // Check to ensure the same number of Ip addresses and subnet masks are specified.
            if (ipAddresses.Count() != subnetMasks.Count())
            {
                throw new ArgumentException("The same number of IP addresses and subnet masks must be specified.");
            }

            // Generate network for each IP/subnet pair.            
            for (var i = 0; i < ipAddresses.Count(); i++)
            {
                // For compatibility purposes, treat subnet masks of 0.0.0.0 as being 255.255.255.255;
                if (subnetMasks[i].ToString() == "0.0.0.0")
                {
                    subnetMasks[i] = IPAddress.Parse("255.255.255.255");
                }

                ipNetworks.Add(IPNetwork.Parse(ipAddresses[i], subnetMasks[i]));
            }

            // Load specified networks.
            if (WebConfigurationManager.AppSettings[AppKey_IPNetwork] != null)
            {
                try
                {
                    ipNetworks.AddRange(WebConfigurationManager.AppSettings[AppKey_IPNetwork]
                                        .Split(',')
                                        .Select(x => x.Trim())
                                        .Select(x => IPNetwork.Parse(x)));
                }
                catch
                {
                    throw new ConfigurationErrorsException("Value for " + AppKey_IPNetwork + " not correctly specified.");
                }
            }

            // Throw exception if no Ip addresses exist in set.
            if (!ipNetworks.Any())
            {
                throw new ConfigurationErrorsException("No addresses were specified for UmbIntranetRestrict.");
            }

            return ipNetworks;
        }

        /// <summary>
        /// Access unauthorized page ID redirect specified for IntranetRestrict.
        /// </summary>
        private int ConfigLoadUnauthorizedPageId()
        {
            try
            {
                return Int32.Parse(WebConfigurationManager.AppSettings[AppKey_UnauthorizedPageId]);
            }
            catch
            {
                throw new ConfigurationErrorsException("Value for " + AppKey_UnauthorizedPageId + " not correctly specified.");
            }
        }
    }
}
