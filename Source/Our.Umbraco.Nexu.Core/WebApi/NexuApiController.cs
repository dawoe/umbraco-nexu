namespace Our.Umbraco.Nexu.Core.WebApi
{
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
        internal NexuApiController(INexuService nexuService)
        {
            this.nexuService = nexuService;
        }        
    }
}
