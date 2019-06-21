namespace Our.Umbraco.Nexu.Web.Composing.Composers
{
    using global::Umbraco.Core;
    using global::Umbraco.Core.Composing;
    using global::Umbraco.Web;

    using Our.Umbraco.Nexu.Core.Composing.Composers;
    using Our.Umbraco.Nexu.Web.Composing.Components;

    /// <summary>
    /// Represents the nexu web composer.
    /// </summary>
    [ComposeAfter(typeof(NexuComposer))]
    internal class NexuWebComposer : IUserComposer
    {
        /// <inheritdoc />
        public void Compose(Composition composition)
        {
            composition.Components().Append<ServerVariablesComponent>();
            composition.ContentApps().Append<RelatedLinksContentAppFactory>();
        }
    }
}
