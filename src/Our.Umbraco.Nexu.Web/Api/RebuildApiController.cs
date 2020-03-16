namespace Our.Umbraco.Nexu.Web.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
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
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RebuildApiController"/> class.
        /// </summary>
        /// <param name="entityParsingService">
        /// The entity parsing service.
        /// </param>
        /// <param name="contentService">
        /// The content Service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public RebuildApiController(IEntityParsingService entityParsingService, IContentService contentService, ILogger logger)
        {
            this.entityParsingService = entityParsingService;
            this.contentService = contentService;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the rebuild status.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
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

        /// <summary>
        /// Starts a full rebuild of the related entities database
        /// </summary>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public HttpResponseMessage Rebuild()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.RebuildJob));

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Starts the rebuild job
        /// </summary>
        public void RebuildJob(object state)
        {
            try
            {
                // mark rebuild in progress
                NexuContext.Current.IsProcessing = true;

                var rootLevelItems = this.contentService.GetRootContent().ToList();

                // parse content tree
                foreach (var item in rootLevelItems)
                {
                    this.ParseContent(item);
                }

                // parser recyle bin
                long totalRecords = 0;
                rootLevelItems = this.contentService.GetPagedChildren(
                    Constants.System.RecycleBinContent,
                    0,
                    int.MaxValue,
                    out totalRecords).ToList();

                foreach (var item in rootLevelItems)
                {
                    this.ParseContent(item);
                }
            }
            catch (Exception e)
            {
                this.logger.Error<RebuildApiController>("An unhandled exception occurred while parsing content", e);
            }
            finally
            {
                // reset context variables after processing
                NexuContext.Current.IsProcessing = false;
                NexuContext.Current.ItemsProcessed = 0;
                NexuContext.Current.ItemInProgress = string.Empty;
            }
        }

        /// <summary>
        /// Parses content item recursively
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void ParseContent(IContent item)
        {
            // set the name of the current processed item
            NexuContext.Current.ItemInProgress = item.Name;

            // parse for links
            this.entityParsingService.ParseContent(item);

            // update items processed counter
            NexuContext.Current.ItemsProcessed++;

            long totalRecords = 0;

            // get the children of current item
            var children = this.contentService.GetPagedChildren(item.Id, 0, int.MaxValue, out totalRecords).ToList();

            foreach (var child in children)
            {
                // parse the content
                this.ParseContent(child);
            }
        }
    }
}
