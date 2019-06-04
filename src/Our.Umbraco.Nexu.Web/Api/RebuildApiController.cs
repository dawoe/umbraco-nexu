namespace Our.Umbraco.Nexu.Web.Api
{
    using System.Net;
    using System.Net.Http;

    using global::Umbraco.Core.Services;
    using global::Umbraco.Web.Editors;

    using Our.Umbraco.Nexu.Common;
    using Our.Umbraco.Nexu.Common.Interfaces.Services;
    using Our.Umbraco.Nexu.Web.Models;

    /// <summary>
    /// Represents rebuild api controller.
    /// </summary>
    public class RebuildApiController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// The entity parsing service.
        /// </summary>
        private readonly IEntityParsingService entityParsingService;

        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RebuildApiController"/> class.
        /// </summary>
        /// <param name="entityParsingService">
        /// The entity parsing service.
        /// </param>
        /// <param name="contentService">
        /// The content Service.
        /// </param>
        public RebuildApiController(IEntityParsingService entityParsingService, IContentService contentService)
        {
            this.entityParsingService = entityParsingService;
            this.contentService = contentService;
        }

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
