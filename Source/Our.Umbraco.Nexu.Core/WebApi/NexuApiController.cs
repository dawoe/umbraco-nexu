namespace Our.Umbraco.Nexu.Core.WebApi
{
    using System.Net;
    using System.Net.Http;

    using global::Umbraco.Web;
    using global::Umbraco.Web.Editors;
    using global::Umbraco.Web.WebApi;

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
        /// Initializes a new instance of the <see cref="NexuApiController"/> class.
        /// </summary>
        public NexuApiController()
        {
            this.nexuService = NexuService.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuApiController"/> class.
        /// </summary>
        /// <param name="nexuService">
        /// The nexu service.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco Context.
        /// </param>
        internal NexuApiController(INexuService nexuService, UmbracoContext umbracoContext) : base(umbracoContext)
        {
            this.nexuService = nexuService;
        }

        public HttpResponseMessage GetIncomingLinks(int contentId)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }        
    }
}
