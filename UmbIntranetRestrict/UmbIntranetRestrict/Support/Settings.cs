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
        /// Access IpAddress specified for IntranetRestrict.
        /// </summary>
        public static IPAddress IpAddress
        {
            get
            {
                try
                {
                    return IPAddress.Parse(WebConfigurationManager.AppSettings[AppKey_IpAddress]);
                }
                catch
                {
                    throw new ConfigurationErrorsException("Value for " + AppKey_IpAddress + " not correctly specified.");
                    
                }
            }
        }

        /// <summary>
        /// Access subnet mask specified for IntranetRestrict.
        /// </summary>
        public static IPAddress SubnetMask
        {
            get
            {
                try
                {
                    return IPAddress.Parse(WebConfigurationManager.AppSettings[AppKey_SubnetMask]);
                }
                catch
                {
                    throw new ConfigurationErrorsException("Value for " + AppKey_SubnetMask + " not correctly specified.");
                }
            }
        }

        /// <summary>
        /// Access unauthorized page ID redirect specified for IntranetRestrict.
        /// </summary>
        public static int UnauthorizedPageId
        {
            get
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
}
