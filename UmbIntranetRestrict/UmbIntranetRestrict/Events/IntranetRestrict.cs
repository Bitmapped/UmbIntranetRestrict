using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.template;
using UmbIntranetRestrict.Support;
using System.Net;
using umbraco.NodeFactory;

namespace UmbIntranetRestrict.Events
{
    public class IntranetRestrict : IApplicationEventHandler
    {
        // Ensure event handler is only registered once.
        private static object registerLock = new object();
        private static bool registerRan = false;

        public void OnApplicationStarted(UmbracoApplicationBase httpApplicationBase, ApplicationContext applicationContext)
        {
            // Handle locking.
            if (!registerRan)
            {
                lock (registerLock)
                {
                    if (!registerRan)
                    {
                        // Register event.
                        UmbracoDefault.AfterRequestInit += new UmbracoDefault.RequestInitEventHandler(this.RestrictIntranet);

                        // Record that registration happened.
                        registerRan = true;
                    }
                }
            }
        }

        #region Unused interface methods
        public void OnApplicationStarting(UmbracoApplicationBase httpApplicationBase, ApplicationContext applicationContext) { }
        public void OnApplicationInitialized(UmbracoApplicationBase httpApplicationBase, ApplicationContext applicationContext) { }
        #endregion

        /// <summary>
        /// Redirect user to restricted access page if not from valid IP.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event properties</param>
        private void RestrictIntranet(object sender, RequestInitEventArgs e)
        {
            // Ensure there is a page to load.
            if (e.Page == null)
            {
                // No valid page.  Do nothing.
                return;
            }

            // Load document corresponding with current page.
            var node = new Node(e.Page.PageID);

            // Determine if page has Intranet restrictions set.
            if (node.HasProperty("umbIntranetRestrict"))
            {
                // Determine if access should be restricted.
                bool intranetRestrict = node.GetProperty("umbIntranetRestrict").Value.ToString() == "1" ? true : false;

                //throw new Exception("Output was " + restrictProperty.ToString());

                // Determine if we are to restrict access.
                if (intranetRestrict)
                {
                    // Get Ip addresses of current request.
                    var requestIp = IPAddress.Parse(e.Context.Request.UserHostAddress);

                    // Determine if request is in allowed subnet.
                    bool allowedRequest = requestIp.IsInSameSubnet(Settings.IpAddress, Settings.SubnetMask);

                    if (!allowedRequest)
                    {
                        // Rewrite URL to display unauthorized page.
                        string rewriteUrl = umbraco.library.NiceUrl(Settings.UnauthorizedPageId);
                        e.Context.RewritePath(rewriteUrl);
                    }
                }
            }
        }
    }
}
