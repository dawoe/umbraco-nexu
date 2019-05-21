namespace Our.Umbraco.Nexu.Core.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Composing;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Common.Interfaces.Factories;
    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents the nexu display model factory
    /// </summary>
    internal class NexuDisplayModelFactory : IDisplayModelFactory
    {
        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The content type service.
        /// </summary>
        private readonly IContentTypeService contentTypeService;

        /// <summary>
        /// The localization service.
        /// </summary>
        private readonly ILocalizationService localizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuDisplayModelFactory"/> class.
        /// </summary>
        /// <param name="contentService">
        /// The content service.
        /// </param>
        /// <param name="contentTypeService">
        /// The content Type Service.
        /// </param>
        /// <param name="localizationService">
        /// The localization Service.
        /// </param>
        public NexuDisplayModelFactory(IContentService contentService, IContentTypeService contentTypeService, ILocalizationService localizationService)
        {
            this.contentService = contentService;
            this.contentTypeService = contentTypeService;
            this.localizationService = localizationService;
        }

        /// <inheritdoc />
        public IEnumerable<NexuRelationDisplay> ConvertRelationsToDisplayModels(IEnumerable<NexuRelation> relations)
        {
            var displayModels = new List<NexuRelationDisplay>();

            var nexuRelations = relations.ToList();

            if (nexuRelations.Any())
            {
                var defaultLanguage = this.localizationService.GetDefaultLanguageIsoCode().ToLower();                

                var udis = nexuRelations.Select(x => x.ParentUdi).Distinct().Select(x => new GuidUdi(new Uri(x)));

                var contentItems = this.contentService.GetByIds(udis).ToDictionary(x => x.GetUdi().ToString());

                if (contentItems.Any())
                {
                    foreach (var relation in nexuRelations)
                    {
                        if (string.IsNullOrWhiteSpace(relation.Culture))
                        {
                            relation.Culture = defaultLanguage;
                        }

                        var contentItem = contentItems.Keys.Contains(relation.ParentUdi) ? contentItems[relation.ParentUdi] : null;

                        if (contentItem != null)
                        {
                            var model = displayModels.FirstOrDefault(
                                x => x.Id == contentItem.Id && x.Culture == relation.Culture);

                            if (model == null)
                            {
                                model = new NexuRelationDisplay();

                                model.Id = contentItem.Id;
                                model.Name = contentItem.GetCultureName(relation.Culture);
                                model.Culture = relation.Culture;
                                displayModels.Add(model);
                            }

                            var prop = contentItem.Properties.FirstOrDefault(x => x.Alias == relation.PropertyAlias);
                           

                            if (prop != null)
                            {
                                var contentType = this.contentTypeService.Get(contentItem.ContentTypeId);

                                var tabName = contentType.CompositionPropertyGroups
                                    .FirstOrDefault(x => x.PropertyTypes.Contains(prop.PropertyType)).Name;

                                model.Properties.Add(new NexuRelationPropertyDisplay
                                                         {
                                                             PropertyName = prop.PropertyType.Name,
                                                             TabName = tabName
                                                         });
                            }
                        }
                    }
                }
                
            }

            return displayModels;
        }


    }
}
