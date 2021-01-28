using Our.Umbraco.Nexu.Common;

namespace Our.Umbraco.Nexu.Web.Composing.Components
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using global::Umbraco.Core.Composing;
    using global::Umbraco.Web;
    using global::Umbraco.Web.JavaScript;

    using Our.Umbraco.Nexu.Web.Api;

    /// <summary>
    /// Represents the component for registering server variables
    /// </summary>
    internal class ServerVariablesComponent : IComponent
    {
        /// <inheritdoc />
        public void Initialize()
        {
            ServerVariablesParser.Parsing += this.ServerVariablesParserParsing;
        }

        /// <inheritdoc />
        public void Terminate()
        {
            ServerVariablesParser.Parsing -= this.ServerVariablesParserParsing;
        }

        /// <summary>
        /// Event handler for the ServerVariablesParsing parsing event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ServerVariablesParserParsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            var urlDictionairy = new Dictionary<string, object>();

            urlDictionairy.Add("RebuildApi", urlHelper.GetUmbracoApiServiceBaseUrl<RebuildApiController>(c => c.GetRebuildStatus()));
            urlDictionairy.Add("RelationCheckApi", urlHelper.GetUmbracoApiServiceBaseUrl<RelationCheckApiController>(c => c.GetIncomingLinks(null)));
            urlDictionairy.Add("PreventDelete", NexuContext.Current.PreventDelete);
            urlDictionairy.Add("PreventUnPublish", NexuContext.Current.PreventUnPublish);

            if (!e.Keys.Contains("Nexu"))
            {
                e.Add("Nexu", urlDictionairy);
            }
        }
    }
}
