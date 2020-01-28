namespace Our.Umbraco.Nexu.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Common.Interfaces.Repositories;
    using Our.Umbraco.Nexu.Common.Interfaces.Services;
    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents nexu entity relation service.
    /// </summary>
    public class NexuEntityRelationService : IEntityRelationService
    {
        /// <summary>
        /// The relation repository.
        /// </summary>
        private readonly IRelationRepository relationRepository;

        /// <summary>
        /// The localization service.
        /// </summary>
        private readonly ILocalizationService localizationService;

        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The content type service.
        /// </summary>
        private readonly IContentTypeService contentTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuEntityRelationService"/> class.
        /// </summary>
        /// <param name="relationRepository">
        /// The relation repository.
        /// </param>
        /// <param name="localizationService">
        /// The localization Service.
        /// </param>
        /// <param name="contentService">
        /// The content Service.
        /// </param>
        /// <param name="contentTypeService">
        /// The content Type Service.
        /// </param>
        public NexuEntityRelationService(IRelationRepository relationRepository, ILocalizationService localizationService, IContentService contentService, IContentTypeService contentTypeService)
        {
            this.relationRepository = relationRepository;
            this.localizationService = localizationService;
            this.contentService = contentService;
            this.contentTypeService = contentTypeService;
        }

        /// <inheritdoc />
        public IList<NexuRelationDisplayModel> GetRelationsForItem(Udi udi)
        {
            var nexuRelationDisplayModels = new List<NexuRelationDisplayModel>();

            var contentTypes = new Dictionary<int, IContentType>();

            var relations = this.relationRepository.GetIncomingRelationsForItem(udi).ToList();

            if (relations.Any())
            {
                var defaultLanguage = this.localizationService.GetDefaultLanguageIsoCode().ToLowerInvariant();

                var contentItems = this.contentService
                    .GetByIds(relations.Select(x => new GuidUdi(new Uri(x.ParentUdi))).Distinct()).ToList();

                foreach (var relation in relations)
                {
                    var culture = !string.IsNullOrWhiteSpace(relation.Culture) ? relation.Culture : defaultLanguage;
                    var content = contentItems.Find(x => x.GetUdi().ToString() == relation.ParentUdi);

                    if (content != null)
                    {
                        var contentName = content.Name;
                        var published = content.Published;

                        if (content.AvailableCultures.Any())
                        {
                            contentName = content.GetCultureName(culture);
                            published = content.IsCulturePublished(culture);
                        }

                        var model = nexuRelationDisplayModels.FirstOrDefault(x => x.Culture == culture && x.Id == content.Id);

                        if (model == null)
                        {
                            model = new NexuRelationDisplayModel
                                        {
                                            IsTrashed = content.Trashed,
                                            Key = content.Key,
                                            Id = content.Id,
                                            Culture = culture,
                                            IsPublished = published,
                                            Name = contentName
                                        };

                            nexuRelationDisplayModels.Add(model);
                        }

                        var prop = content.Properties.FirstOrDefault(x => x.Alias == relation.PropertyAlias);                        

                        if (prop != null)
                        {
                            if (contentTypes.ContainsKey(content.ContentTypeId) == false)
                            {
                                contentTypes.Add(content.ContentTypeId, this.contentTypeService.Get(content.ContentTypeId));
                            }

                            var currentContentType = contentTypes[content.ContentTypeId];

                            var tabName = currentContentType.CompositionPropertyGroups.FirstOrDefault(
                                cpg => cpg.PropertyTypes.Any(pt => pt.Alias == relation.PropertyAlias))?.Name;

                            model.Properties.Add(new NexuRelationPropertyDisplay
                                                     {
                                                         PropertyName = prop.PropertyType.Name,
                                                         TabName = tabName
                                                     });
                        }
                    }

                }
            }

            return nexuRelationDisplayModels;
        }

        /// <inheritdoc />
        public IList<NexuRelationDisplayModel> GetUsedItemsFromList(IList<Udi> udis)
        {
            var nexuRelationDisplayModels = new List<NexuRelationDisplayModel>();

            if (udis?.Any() != true)
            {
               return nexuRelationDisplayModels;
            }

            var relations = this.relationRepository.GetUsedItemsFromList(udis);

            if (!relations.Any())
            {
                return nexuRelationDisplayModels;
            }
          
            foreach (var relation in relations)
            {
                var content = this.contentService.GetById(GuidUdi.Parse(relation.Key).Guid);

                if (content != null)
                {
                    var contentName = content.Name;
                    var published = content.Published;

                    if (content.AvailableCultures.Any())
                    {
                        contentName = content.GetCultureName(relation.Value);
                        published = content.IsCulturePublished(relation.Value);
                    }

                    nexuRelationDisplayModels.Add(new NexuRelationDisplayModel
                                                      {
                                                          Culture = relation.Value,
                                                          Id = content.Id,
                                                          IsPublished = published,
                                                          IsTrashed = content.Trashed,
                                                          Key = content.Key,
                                                          Name = contentName                                                          
                                                      });
                }
            }

            return nexuRelationDisplayModels;
        }
    }
}
