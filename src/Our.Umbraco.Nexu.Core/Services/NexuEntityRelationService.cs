namespace Our.Umbraco.Nexu.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
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
        public NexuEntityRelationService(IRelationRepository relationRepository, ILocalizationService localizationService, IContentService contentService)
        {
            this.relationRepository = relationRepository;
            this.localizationService = localizationService;
            this.contentService = contentService;
        }

        /// <inheritdoc />
        public IList<NexuRelationDisplayModel> GetRelationsForItem(Udi udi)
        {
            var nexuRelationDisplayModels = new List<NexuRelationDisplayModel>();

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
                        var model = nexuRelationDisplayModels.FirstOrDefault(x => x.Culture == culture && x.Id == content.Id);

                        if (model == null)
                        {
                            model = new NexuRelationDisplayModel
                                        {
                                            IsTrashed = content.Trashed,
                                            Key = content.Key,
                                            Id = content.Id,
                                            Culture = culture,
                                            IsPublished = content.IsCulturePublished(culture),
                                            Name = content.GetCultureName(culture)
                                        };

                            nexuRelationDisplayModels.Add(model);
                        }                                              
                    }

                }
            }

            return nexuRelationDisplayModels;
        }
    }
}
