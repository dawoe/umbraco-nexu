namespace Our.Umbraco.Nexu.Core.WebApi
{
    using System.Net;
    using System.Net.Http;

    using AutoMapper;

    using global::Umbraco.Web;
    using global::Umbraco.Web.Editors;

    using Our.Umbraco.Nexu.Core.Interfaces;

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

        public HttpResponseMessage GetIncomingLinks(int contentId)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);            
        }        
    }
}
