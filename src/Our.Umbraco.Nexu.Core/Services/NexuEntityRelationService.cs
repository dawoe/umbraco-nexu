namespace Our.Umbraco.Nexu.Core.Services
{
    using System.Collections.Generic;

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
            var relations = this.relationRepository.GetIncomingRelationsForItem(udi);

            return new List<NexuRelationDisplayModel>();
        }
    }
}
