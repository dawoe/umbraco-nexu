namespace Our.Umbraco.Nexu.Core
{
    using global::Umbraco.Core;

    /// <summary>
    /// Bootstrapper to handle umbraco startup events
    /// </summary>
    internal class BootStrapper : ApplicationEventHandler
    {
        /// <summary>
        /// Overridable method to execute when Bootup is completed, this allows you to perform any other bootup logic required for the application.
        /// Resolution is frozen so now they can be used to resolve instances.
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {            
            using (ApplicationContext.Current.ProfilingLogger.TraceDuration<BootStrapper>("Begin ApplicationStarted", "End ApplicationStarted"))
            {
                // setup needed relation types
                NexuService.Current.SetupRelationTypes();
            }
        }
    }
}
