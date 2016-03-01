using System;
using System.Net;
using System.Web;
using UmbIntranetRestrict.Support;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace UmbIntranetRestrict.Events
{
    public class IntranetRestrictEventHandler : ApplicationEventHandler
    {
        #region Fields

        /// <summary>
        /// Configuration settings.
        /// </summary>
        private readonly Settings settings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor to load settings.
        /// </summary>
        public IntranetRestrictEventHandler()
        {
            // Load settings from config file.
            this.settings = new Settings();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Register event handler on start.
        /// </summary>
        /// <param name="httpApplicationBase">Umbraco application.</param>
        /// <param name="applicationContext">Application context.</param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            PublishedContentRequest.Prepared += PublishedContentRequest_Prepared;
        }

        /// <summary>
        /// Event handler to redirect traffic from unauthorized IPs.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
        private void PublishedContentRequest_Prepared(object sender, EventArgs e)
        {
            // Get request.
            var request = sender as PublishedContentRequest;
            var context = HttpContext.Current;

            // If the response is invalid, the page doesn't exist, or will be changed already, don't do anything more.
            if ((request == null) || (!request.HasPublishedContent) || (request.Is404) || (request.IsRedirect) || (request.ResponseStatusCode > 0))
            {
                // Log for debugging.
                LogHelper.Debug<IntranetRestrictEventHandler>("Stopping IntranetRestrict for requested URL {0} because request was null ({1}), there was no published content ({2}), was 404 ({3}), was a redirect ({4}), or status code ({5}) was already set.",
                    () => context.Request.Url.AbsolutePath,
                    () => (request == null),
                    () => (!request.HasPublishedContent),
                    () => (request.Is404),
                    () => (request.IsRedirect),
                    () => request.ResponseStatusCode);

                return;
            }

            // Determine if page has Intranet restrictions set.
            if (request.PublishedContent.GetPropertyValue<bool>("umbIntranetRestrict", false))
            {
                // Get Ip addresses of current request.
                var requestIp = IPAddress.Parse(context.Request.UserHostAddress);

                // Determine if request is in allowed subnet.
                if (!requestIp.IsInAllowedNetwork(settings.AllowedIpNetworks))
                {
                    // Get page to display.
                    var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                    var unauthorizedContent = umbracoHelper.TypedContent(settings.UnauthorizedPageId);

                    // Get template for page to display.
                    var fileService = ApplicationContext.Current.Services.FileService;
                    var unauthorizedTemplate = fileService.GetTemplate(unauthorizedContent.TemplateId);

                    // Change published content to unauthorized content page.  Set template.
                    request.SetInternalRedirectPublishedContent(unauthorizedContent);
                    request.SetTemplate(unauthorizedTemplate);

                    // Set HTTP 403 Unauthorized status code for Umbraco. Umbraco doesn't handle substatus codes, so use generic 403.
                    request.SetResponseStatus(403, "Unauthorized");

                    // Include in try-catch block in case there are problems setting depending on IIS version.
                    try
                    {
                        // Set status code and substatus code directly for IIS. While Umbraco sets status code itself, if we don't set status directly ourselves here substatus gets ignored.
                        context.Response.StatusCode = 403;
                        context.Response.SubStatusCode = 6;

                        // Try skipping custom IIS errors if desired.
                        context.Response.TrySkipIisCustomErrors = UmbracoConfig.For.UmbracoSettings().WebRouting.TrySkipIisCustomErrors;
                    }
                    catch (Exception ex)
                    {
                        // Error trying to set values.
                        LogHelper.Debug<IntranetRestrictEventHandler>(ex.ToString());
                    }

                    // Log for debugging.
                    LogHelper.Debug<IntranetRestrictEventHandler>("Attempt to access {0} from {1} was unauthorized.",
                        () => context.Request.Url.AbsolutePath,
                        () => requestIp);
                }
            }
        }

        #endregion Methods
    }
}