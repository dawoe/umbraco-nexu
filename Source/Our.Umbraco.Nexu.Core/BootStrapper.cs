namespace Our.Umbraco.Nexu.Core
{
    using ObjectResolution;
    using Resolvers;

    using global::Umbraco.Core;

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
    }
}
