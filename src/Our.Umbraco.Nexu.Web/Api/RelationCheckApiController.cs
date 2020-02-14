namespace Our.Umbraco.Nexu.Web.Api
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using global::Umbraco.Core;
    using global::Umbraco.Web.Editors;

    using Our.Umbraco.Nexu.Common.Interfaces.Services;

    /// <summary>
    /// Represents the relation check api controller
    /// </summary>
    public class RelationCheckApiController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// The entity relation service.
        /// </summary>
        private readonly IEntityRelationService entityRelationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationCheckApiController"/> class.
        /// </summary>
        /// <param name="entityRelationService">
        /// The entity relation service.
        /// </param>
        public RelationCheckApiController(IEntityRelationService entityRelationService)
        {
            this.entityRelationService = entityRelationService;
        }

        /// <summary>
        /// Gets the incoming links
        /// </summary>
        /// <param name="udi">
        /// The udi.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public HttpResponseMessage GetIncomingLinks(string udi)
        {
            var relations = this.entityRelationService.GetRelationsForItem(new GuidUdi(new Uri(udi)));

            return this.Request.CreateResponse(HttpStatusCode.OK, relations);
        }

        /// <summary>
        /// Checks a list of udi's for linked items
        /// </summary>
        /// <param name="udis">
        /// The udis.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage CheckLinkedItems([FromBody] string[] udis)
        {
            var listUdis = new List<Udi>();

            foreach (var stringUdi in udis)
            {
                GuidUdi guidUdi;

                if (GuidUdi.TryParse(stringUdi, out guidUdi))
                {
                    listUdis.Add(guidUdi);
                }
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, this.entityRelationService.GetUsedItemsFromList(listUdis));
        }

        [HttpGet]
        public HttpResponseMessage CheckDescendants(string udi)
        {
            var result = false;
            GuidUdi guidUdi;

            if (GuidUdi.TryParse(udi, out guidUdi))
            {
               result = this.entityRelationService.CheckLinksInDescendants(guidUdi);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
