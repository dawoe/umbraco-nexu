using System;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Nexu.Core.WebApi
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using AutoMapper;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Web;
    using global::Umbraco.Web.Editors;

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
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The media service.
        /// </summary>
        private readonly IMediaService mediaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuApiController"/> class.
        /// </summary>
        public NexuApiController()
        {
            this.mediaService = this.Services.MediaService;
            this.contentService = this.Services.ContentService;
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
        /// <param name="contentService">
        /// The content Service.
        /// </param>
        /// <param name="mediaService">
        /// The media Service.
        /// </param>
        internal NexuApiController(UmbracoContext umbracoContext, INexuService nexuService,  IMappingEngine mappingEngine, IContentService contentService, IMediaService mediaService) : base(umbracoContext)
        {
            this.nexuService = nexuService;
            this.mappingEngine = mappingEngine;
            this.contentService = contentService;
            this.mediaService = mediaService;
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
        /// Check descendants for incoming links.
        /// </summary>
        /// <param name="contentid">
        /// The contentid.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [HttpGet]
        public bool CheckContentDescendantsForIncomingLinks(int contentid)
        {
            var children = this.contentService.GetChildren(contentid).ToList();

            if (children.Count == 0)
            {
                return false;
            }

            return this.CheckDescendantsOfContentTreeForIncomingLinks(children);
        }

        /// <summary>
        /// Check media descendants for incoming links.
        /// </summary>
        /// <param name="mediaId">
        /// The media id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [HttpGet]
        public bool CheckMediaDescendantsForIncomingLinks(int mediaId)
        {
            var children = this.mediaService.GetChildren(mediaId).ToList();

            if (children.Count == 0)
            {
                return false;
            }

            return this.CheckDescendantsOfMediaTreeForIncomingLinks(children);
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

        /// <summary>
        /// Rebuilds relations
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public HttpResponseMessage Rebuild(int id = Constants.System.Root)
        {
            Thread backgroundRebuild = new Thread(new ParameterizedThreadStart(this.RebuildJob));
            backgroundRebuild.IsBackground = true;
            backgroundRebuild.Name = "NexuRebuildJob";
            backgroundRebuild.Start(id);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Rebuild job
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        internal void RebuildJob(object id)
        {
            try
            {
                var rootLevelItems = new List<IContent>();


                var attempInt = id.TryConvertTo<int>();

                if (!attempInt.Success)
                {
                    return;
                }

                // mark rebuild in progress
                NexuContext.Current.IsProcessing = true;

                if (attempInt.Result == Constants.System.Root)
                {
                    // get the root level content items
                    rootLevelItems = this.contentService.GetRootContent().ToList();
                }
                else
                {
                    // get the indicated start item
                    var startItem = this.contentService.GetById(attempInt.Result);

                    if (startItem != null)
                    {
                        rootLevelItems.Add(startItem);
                    }
                }

                // parse content tree
                foreach (var item in rootLevelItems)
                {
                    this.ParseContent(item);
                }

                // parser recyle bin
                if (attempInt.Result == Constants.System.Root)
                {
                    rootLevelItems = this.contentService.GetChildren(Constants.System.RecycleBinContent).ToList();

                    foreach (var item in rootLevelItems)
                    {
                        this.ParseContent(item);
                    }
                }

                // reset context variables after processing
                NexuContext.Current.IsProcessing = false;
                NexuContext.Current.ItemsProcessed = 0;
                NexuContext.Current.ItemInProgress = string.Empty;
            }
            catch (Exception e)
            {
                LogHelper.Error<NexuApiController>("An unhandled exception occurred while parsing content", e);
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
            this.nexuService.ParseContent(item);

            // update items processed counter
            NexuContext.Current.ItemsProcessed++;

            // get the children of current item
            var children = this.contentService.GetChildren(item.Id).ToList();

            foreach (var child in children)
            {
                // parse the content
                this.ParseContent(child);
            }
        }

        /// <summary>
        /// Check descendants of content tree for incoming links recursively
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckDescendantsOfContentTreeForIncomingLinks(IEnumerable<IContent> items)
        {
            var childIds = new List<int>();

            foreach (var item in items)
            {
                var relations = this.nexuService.GetNexuRelationsForContent(item.Id, false).ToList();

                if (relations.Any())
                {
                    return true;
                }

                childIds.Add(item.Id);
            }

            foreach (var id in childIds)
            {
                var children = this.contentService.GetChildren(id).ToList();

                if (children.Count != 0)
                {
                    var hasLinks = this.CheckDescendantsOfContentTreeForIncomingLinks(children);

                    if (hasLinks)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check descendants of media tree for incoming links recursively
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckDescendantsOfMediaTreeForIncomingLinks(IEnumerable<IMedia> items)
        {
            var childIds = new List<int>();

            foreach (var item in items)
            {
                var relations = this.nexuService.GetNexuRelationsForContent(item.Id, false).ToList();

                if (relations.Any())
                {
                    return true;
                }

                childIds.Add(item.Id);
            }

            foreach (var id in childIds)
            {
                var children = this.mediaService.GetChildren(id).ToList();

                if (children.Count != 0)
                {
                    var hasLinks = this.CheckDescendantsOfMediaTreeForIncomingLinks(children);

                    if (hasLinks)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
