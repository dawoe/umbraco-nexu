namespace Our.Umbraco.Nexu.Web.Composing.Components
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Models.ContentEditing;
    using global::Umbraco.Core.Models.Membership;

    using Our.Umbraco.Nexu.Common.Interfaces.Factories;
    using Our.Umbraco.Nexu.Common.Interfaces.Repositories;

    /// <summary>
    /// Represents the related links content app factory.
    /// </summary>
    public class RelatedLinksContentAppFactory : IContentAppFactory
    {
        private readonly IRelationRepository relationRepository;

        private readonly IDisplayModelFactory displayModelFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedLinksContentAppFactory"/> class.
        /// </summary>
        /// <param name="relationRepository">
        /// The relation repository.
        /// </param>
        /// <param name="displayModelFactory">
        /// The display model factory.
        /// </param>
        public RelatedLinksContentAppFactory(IRelationRepository relationRepository, IDisplayModelFactory displayModelFactory)
        {
            this.relationRepository = relationRepository;
            this.displayModelFactory = displayModelFactory;
        }

        /// <inheritdoc />
        public ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)
        {
            //var repo = Current.Factory.GetInstance(typeof(Common.Interfaces.Repositories.IRelationRepository)) as Common.Interfaces.Repositories.IRelationRepository;
            //var factory = Current.Factory.GetInstance(typeof(IDisplayModelFactory)) as IDisplayModelFactory;

            var contentBase = source as IContentBase;

            if (contentBase == null)
            {
                return null;
            }

            var relations = this.relationRepository.GetIncomingRelationsForItem(contentBase.GetUdi()).ToList();

            if (relations.Any() == false)
            {
                return null;
            }

            var models = this.displayModelFactory.ConvertRelationsToDisplayModels(relations);

            return new ContentApp
                       {
                           Alias = "nexuRelatedLinksApp",
                           Icon = "icon-link",
                           Name = "Related links",
                           View = "/App_Plugins/Nexu/views/related-links-app.html",
                           ViewModel = models
                       };
        }
    }
}
