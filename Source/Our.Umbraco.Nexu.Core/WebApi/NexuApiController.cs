namespace Our.Umbraco.Nexu.Core.WebApi
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using AutoMapper;

    using global::Umbraco.Web;
    using global::Umbraco.Web.Editors;
    using global::Umbraco.Web.WebApi;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The nexu api controller.
    /// </summary>
    public class NexuApiController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// The nexu service.
        /// </summary>
        private readonly INexuService nexuService;

        /// <summary>
        /// The mapping engine.
        /// </summary>
        private readonly IMappingEngine mappingEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuApiController"/> class.
        /// </summary>
        public NexuApiController()
        {
            this.mappingEngine = AutoMapper.Mapper.Engine;
            this.nexuService = NexuService.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuApiController"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        /// <param name="nexuService">
        /// The nexu service.
        /// </param>
        /// <param name="mappingEngine">
        /// The mapping Engine.
        /// </param>
        internal NexuApiController(UmbracoContext umbracoContext, INexuService nexuService,  IMappingEngine mappingEngine) : base(umbracoContext)
        {
            this.nexuService = nexuService;
            this.mappingEngine = mappingEngine;
        }

        /// <summary>
        /// Gets incoming links for a document
        /// </summary>
        /// <param name="contentId">
        /// The content id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        public HttpResponseMessage GetIncomingLinks(int contentId)
        {
            var relations = this.nexuService.GetNexuRelationsForContent(contentId, false);

            var relatedDocs = this.mappingEngine.Map<IEnumerable<RelatedDocument>>(relations.ToList());           

            return this.Request.CreateResponse(HttpStatusCode.OK, relatedDocs);
        }

        /// <summary>
        /// Gets the rebuild status.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        public HttpResponseMessage GetRebuildStatus()
        {
            var model = new RebuildStatus
                            {
                                IsProcessing = NexuContext.Current.IsProcessing,
                                ItemsProcessed = NexuContext.Current.ItemsProcessed,
                                ItemName = NexuContext.Current.ItemInProgress
                            };

            return this.Request.CreateResponse(HttpStatusCode.OK, model);
        }
    }
}
