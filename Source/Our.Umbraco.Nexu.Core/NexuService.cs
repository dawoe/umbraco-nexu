namespace Our.Umbraco.Nexu.Core
{
    using System;
    using System.Collections.Generic;

    using Constants;    

    using Interfaces;

    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    /// <summary>
    /// Nexu service
    /// </summary>
    public class NexuService : INexuService
    {
        /// <summary>
        /// Internal service instance
        /// </summary>
        private static NexuService service;

        /// <summary>
        /// The profiler.
        /// </summary>
        private ProfilingLogger profiler;

        /// <summary>
        /// The relation service.
        /// </summary>
        private IRelationService relationService;           

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuService"/> class. 
        /// </summary>
        /// <param name="profiler">
        /// The profiler.
        /// </param>
        /// <param name="relationService">
        /// The relation Service.
        /// </param>
        public NexuService(ProfilingLogger profiler, IRelationService relationService)
        {
            this.profiler = profiler;
            this.relationService = relationService;           
            service = this;
        }

        /// <summary>
        /// The current nexu service instance
        /// </summary>
        public static NexuService Current => service ?? new NexuService(global::Umbraco.Core.ApplicationContext.Current.ProfilingLogger, global::Umbraco.Core.ApplicationContext.Current.Services.RelationService);           

        /// <summary>
        /// Sets up the needed the relation types
        /// </summary>
        public void SetupRelationTypes()
        {           
            using (this.profiler.DebugDuration<NexuService>("Begin SetupRelationTypes", "End SetupRelationTypes"))
            {
                this.SetupDocumentToDocumentRelationType();
                this.SetupDocumentToMediaRelationType();
            }
        }

        public IEnumerable<IPropertyParser> GetAllPropertyParsers()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets up document to document relation type.
        /// </summary>
        private void SetupDocumentToDocumentRelationType()
        {
            if (this.relationService.GetRelationTypeByAlias(RelationTypes.DocumentToDocumentAlias) != null)
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
            if (this.relationService.GetRelationTypeByAlias(RelationTypes.DocumentToMediaAlias) != null)
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

            this.relationService.Save(relationType);
        }
    }
}
