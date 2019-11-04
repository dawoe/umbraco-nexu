﻿namespace Our.Umbraco.Nexu.Web.Composing.Components
{
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Models.ContentEditing;
    using global::Umbraco.Core.Models.Membership;

    using Our.Umbraco.Nexu.Common.Interfaces.Services;

    /// <summary>
    /// Represents the related links content app factory.
    /// </summary>
    public class RelatedLinksContentAppFactory : IContentAppFactory
    {
        private readonly IEntityRelationService entityRelationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedLinksContentAppFactory"/> class.
        /// </summary>
        public RelatedLinksContentAppFactory(IEntityRelationService entityRelationService)
        {
            this.entityRelationService = entityRelationService;
        }

        /// <inheritdoc />
        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            var contentBase = source as IContentBase;

            if (contentBase == null)
            {
                return null;
            }

            var nexuRelationDisplayModels = this.entityRelationService.GetRelationsForItem(contentBase.GetUdi());
            return new ContentApp
                       {
                           Alias = "nexuRelatedLinksApp",
                           Weight = 99,
                           Icon = "icon-link",
                           Name = "Related links",
                           View = "/App_Plugins/Nexu/views/related-links-app.html",
                           ViewModel = nexuRelationDisplayModels
                       };
        }
    }
}
