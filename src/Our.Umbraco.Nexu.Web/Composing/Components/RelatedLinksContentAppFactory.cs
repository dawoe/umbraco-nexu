namespace Our.Umbraco.Nexu.Web.Composing.Components
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Models.ContentEditing;
    using global::Umbraco.Core.Models.Membership;

    /// <summary>
    /// Represents the related links content app factory.
    /// </summary>
    public class RelatedLinksContentAppFactory : IContentAppFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedLinksContentAppFactory"/> class.
        /// </summary>
        public RelatedLinksContentAppFactory()
        {
        }

        /// <inheritdoc />
        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            var contentBase = source as IContentBase;

            if (contentBase == null)
            {
                return null;
            }

            return new ContentApp
                       {
                           Alias = "nexuRelatedLinksApp",
                           Icon = "icon-link",
                           Name = "Related links",
                           View = "/App_Plugins/Nexu/views/related-links-app.html"
                       };
        }
    }
}
