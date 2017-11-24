namespace Our.Umbraco.Nexu.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using AutoMapper;

    using ObjectResolution;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Events;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Web;
    using global::Umbraco.Web.UI.JavaScript;

    using Our.Umbraco.Nexu.Core.Mapping.Profiles;
    using Our.Umbraco.Nexu.Core.WebApi;

    /// <summary>
    /// Bootstrapper to handle umbraco startup events
    /// </summary>
    internal class BootStrapper : ApplicationEventHandler
    {
        /// <inheritdoc />
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            using (ApplicationContext.Current.ProfilingLogger.TraceDuration<BootStrapper>("Begin ApplicationStarted", "End ApplicationStarted"))
            {
                // set up mappings
                Mapper.AddProfile<NexuMappingProfile>();

                // setup server variables
                ServerVariablesParser.Parsing += this.ServerVariablesParserParsing;

                // setup content service events
                ContentService.Saved += this.ContentServiceOnSaved;
            }
        }

        /// <inheritdoc />
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            using (
                ApplicationContext.Current.ProfilingLogger.TraceDuration<BootStrapper>(
                    "Begin ApplicationInitialized",
                    "End ApplicationInitialized"))
            {
                // resolve property parsers
                PropertyParserResolver.Current =
                    new PropertyParserResolver(PluginManager.Current.ResolvePropertyParsers());

                // resolve grid editor parsers
                GridEditorParserResolver.Current = new GridEditorParserResolver(PluginManager.Current.ResolveGridEditorParsers());
            }
        }

        /// <summary>
        /// Content service saved event handler
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private void ContentServiceOnSaved(IContentService sender, SaveEventArgs<IContent> saveEventArgs)
        {
            foreach (var content in saveEventArgs.SavedEntities)
            {
                NexuService.Current.ParseContent(content);
            }
        }

        /// <summary>
        /// ServerVariablesParser parsing event handler
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

            urlDictionairy.Add("BaseApi", urlHelper.GetUmbracoApiServiceBaseUrl<NexuApiController>(c => c.GetIncomingLinks(-1)));
            urlDictionairy.Add("GetIncomingLinks", urlHelper.GetUmbracoApiService<NexuApiController>("GetIncomingLinks", null));
            urlDictionairy.Add("GetRebuildStatus", urlHelper.GetUmbracoApiService<NexuApiController>("GetRebuildStatus"));
            urlDictionairy.Add("Rebuild", urlHelper.GetUmbracoApiService<NexuApiController>("Rebuild", null));

            if (!e.Keys.Contains("Nexu"))
            {
                e.Add("Nexu", urlDictionairy);
            }
        }
    }
}
