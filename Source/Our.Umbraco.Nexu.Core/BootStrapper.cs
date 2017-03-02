namespace Our.Umbraco.Nexu.Core
{
    using System;

    using AutoMapper;

    using ObjectResolution;
    using Resolvers;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Events;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Mapping.Profiles;

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
                // setup needed relation types
                NexuService.Current.SetupRelationTypes();

                // set up mappings
                Mapper.AddProfile<NexuMappingProfile>();

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
                PropertyParserResolver.Current =
                    new PropertyParserResolver(PluginManager.Current.ResolvePropertyParsers());
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
    }
}
