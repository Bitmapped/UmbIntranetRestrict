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
        private const string AppKey_UnauthorizedPageId = "IntranetRestrict:UnauthorizedPageId";

        /// <summary>
        /// Load settings from configuration file.
        /// </summary>
        public Settings()
        {
            // Load values from config files.
            this.IpAddresses = this.ConfigLoadIpAddresses(AppKey_IpAddress);
            this.SubnetMasks = this.ConfigLoadIpAddresses(AppKey_SubnetMask);
            this.UnauthorizedPageId = this.ConfigLoadUnauthorizedPageId();

            // Check to ensure the same number of Ip addresses and subnet masks are specified.
            if (this.IpAddresses.Count != this.SubnetMasks.Count)
            {
                throw new ConfigurationErrorsException("The same number of IP addresses and subnet masks must be specified.");
            }
        }

        /// <summary>
        /// Allowed IP addresses.
        /// </summary>
        public List<IPAddress> IpAddresses { get; private set; }

        /// <summary>
        /// Allowed subnet masks.
        /// </summary>
        public List<IPAddress> SubnetMasks { get; private set; }

        /// <summary>
        /// PageId for redirecting unauthorized users.
        /// </summary>
        public int UnauthorizedPageId { get; private set; }

        /// <summary>
        /// Access IpAddress specified for IntranetRestrict.
        /// </summary>
        private List<IPAddress> ConfigLoadIpAddresses(string key)
        {
            try
            {
                // Split IP address into multiple parts.
                var strIpAddresses = WebConfigurationManager.AppSettings[key].Split(',').Select(x => x.Trim());

                // Process each string.
                var ipAddresses = new List<IPAddress>();
                foreach (var strIpAddress in strIpAddresses)
                {
                    ipAddresses.Add(IPAddress.Parse(strIpAddress));
                }

                // Throw exception if no Ip addresses exist in set.
                if (!ipAddresses.Any())
                {
                    throw new ConfigurationErrorsException("No Ip addresses were specified.");
                }

                return ipAddresses;
            }
            catch
            {
                throw new ConfigurationErrorsException("Value for " + key + " not correctly specified.");

            }
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
