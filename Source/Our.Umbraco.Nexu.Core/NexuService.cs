namespace Our.Umbraco.Nexu.Core
{
    using System;

    using Constants;

    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    /// <summary>
    /// Nexu service
    /// </summary>
    public class NexuService
    {
        /// <summary>
        /// Internal service instance
        /// </summary>
        private static NexuService service;                     

        /// <summary>
        /// Prevents a default instance of the <see cref="NexuService"/> class from being created.
        /// </summary>
        private NexuService()
        {
            this.Profiler = global::Umbraco.Core.ApplicationContext.Current.ProfilingLogger;
            this.RelationService = global::Umbraco.Core.ApplicationContext.Current.Services.RelationService;
            service = this;
        }

        /// <summary>
        /// The current nexu service instance
        /// </summary>
        public static NexuService Current => service ?? new NexuService();

        /// <summary>
        /// Gets or sets the relation service.
        /// </summary>
        private IRelationService RelationService { get; set; }

        /// <summary>
        /// Gets or sets the profiler.
        /// </summary>
        private ProfilingLogger Profiler { get; set; }

        /// <summary>
        /// Sets up the needed the relation types
        /// </summary>
        internal void SetupRelationTypes()
        {
            using (this.Profiler.DebugDuration<NexuService>("Begin SetupRelationTypes", "End SetupRelationTypes"))
            {
                this.SetupDocumentToDocumentRelationType();
                this.SetupDocumentToMediaRelationType();
            }
        }

        /// <summary>
        /// Sets up document to document relation type.
        /// </summary>
        private void SetupDocumentToDocumentRelationType()
        {
            if (this.RelationService.GetRelationTypeByAlias(RelationTypes.DocumentToDocumentAlias) != null)
            {
                return;
            }

            this.CreateRelationType(RelationTypes.DocumentToDocumentAlias, RelationTypes.DocumentToDocumentName, new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document));            
        }

        /// <summary>
        /// Sets up the document to media relation type.
        /// </summary>
        private void SetupDocumentToMediaRelationType()
        {
            if (this.RelationService.GetRelationTypeByAlias(RelationTypes.DocumentToMediaAlias) != null)
            {
                return;
            }

            this.CreateRelationType(RelationTypes.DocumentToMediaAlias, RelationTypes.DocumentToMediaName, new Guid(global::Umbraco.Core.Constants.ObjectTypes.Media));
        }

        /// <summary>
        /// Creates a relation type.
        /// </summary>
        /// <param name="alias">
        /// The alias of the relation type
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="childObjectType">
        /// The child object type.
        /// </param>
        private void CreateRelationType(string alias, string name, Guid childObjectType)
        {
            var relationType = new RelationType(
                                   childObjectType,
                                   new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document),
                                   alias,
                                   name);

            this.RelationService.Save(relationType);
        }
    }
}
