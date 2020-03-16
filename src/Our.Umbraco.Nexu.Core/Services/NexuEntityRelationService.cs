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

        private readonly IMediaService mediaService;

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
        /// <param name="mediaService">The media service</param>
        public NexuEntityRelationService(IRelationRepository relationRepository, ILocalizationService localizationService, IContentService contentService, IContentTypeService contentTypeService, IMediaService mediaService)
        {
            this.relationRepository = relationRepository;
            this.localizationService = localizationService;
            this.contentService = contentService;
            this.contentTypeService = contentTypeService;
            this.mediaService = mediaService;
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
          
            foreach (var relation in relations.Distinct())
            {
                var guidUdi = GuidUdi.Parse(relation.Key);

                if (guidUdi.EntityType == Constants.UdiEntityType.Document)
                {
                    var content = this.contentService.GetById(guidUdi.Guid);

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
                else if (guidUdi.EntityType == Constants.UdiEntityType.Media)
                {
                    var media = this.mediaService.GetById(guidUdi.Guid);

                    if (media != null)
                    {
                        if (nexuRelationDisplayModels.Any(x => x.Key == media.Key) == false)
                        {
                            nexuRelationDisplayModels.Add(new NexuRelationDisplayModel
                                                              {
                                                                  Culture = string.Empty,
                                                                  Id = media.Id,
                                                                  IsPublished = false,
                                                                  IsTrashed = media.Trashed,
                                                                  Key = media.Key,
                                                                  Name = media.Name
                                                              });
                        }
                        
                    }
                }                
            }

            return nexuRelationDisplayModels;
        }

        public bool CheckLinksInDescendants(GuidUdi rootId)
        {
            if(rootId.EntityType == "media")
            {
                return CheckMediaDescendants(rootId);
            }

            return CheckContentDescendants(rootId);
        }

        private bool CheckContentDescendants(GuidUdi rootId)
        {
            var content = this.contentService.GetById(rootId.Guid);

            if (content != null)
            {
                var descendants = this.contentService.GetPagedDescendants(content.Id, 0, int.MaxValue, out long totalRecords).ToList();

                if (descendants.Any())
                {
                    var udis = descendants.Select(x => (Udi)x.GetUdi()).ToList();

                    var relations = this.relationRepository.GetUsedItemsFromList(udis);

                    if (relations.Any())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckMediaDescendants(GuidUdi rootId)
        {
            var media = this.mediaService.GetById(rootId.Guid);

            if (media != null)
            {
                var descendants = this.mediaService.GetPagedDescendants(media.Id, 0, int.MaxValue, out long totalRecords).ToList();

                if (descendants.Any())
                {
                    var udis = descendants.Select(x => (Udi)x.GetUdi()).ToList();

                    var relations = this.relationRepository.GetUsedItemsFromList(udis);

                    if (relations.Any())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
