namespace Our.Umbraco.Nexu.Core
{
    using global::Umbraco.Core;

    /// <summary>
    /// Bootstrapper to handle umbraco startup events
    /// </summary>
    internal class BootStrapper : ApplicationEventHandler
    {
        /// <summary>
        /// Overridable method to execute when the ApplicationContext is created and other static objects that require initialization have been setup
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // setup needed relation types
            NexuService.Current.SetupRelationTypes();
        }
    }
}
