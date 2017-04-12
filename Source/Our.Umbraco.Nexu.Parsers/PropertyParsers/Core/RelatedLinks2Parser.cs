namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The related links parser in Umbraco V7.6
    /// </summary>
    public class RelatedLinks2Parser : IPropertyParser
    {
        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The cache provider.
        /// </summary>
        private readonly ICacheProvider cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedLinks2Parser"/> class.
        /// </summary>
        public RelatedLinks2Parser()
        {
            this.contentService = ApplicationContext.Current.Services.ContentService;
            this.cacheProvider = ApplicationContext.Current.ApplicationCache.StaticCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedLinks2Parser"/> class.
        /// </summary>
        /// <param name="contentService">
        /// The content service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache provider.
        /// </param>
        public RelatedLinks2Parser(IContentService contentService, ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
            this.contentService = contentService;
        }

        /// <summary>
        /// Check if it's a parser for a data type definition
        /// </summary>
        /// <param name="dataTypeDefinition">
        /// The data type definition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return dataTypeDefinition.PropertyEditorAlias.Equals("Umbraco.RelatedLinks2");
        }

        /// <summary>
        /// Gets the linked entites from the property value
        /// </summary>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            var entities = new List<ILinkedEntity>();

            try
            {
                var jsonValue = JsonConvert.DeserializeObject<List<object>>(propertyValue.ToString());

                foreach (JObject item in jsonValue)
                {
                    if (this.IsInternalLink(item))
                    {
                        var udi = this.GetInternalId(item);

                        if (ParserHelper.IsDocumentUdi(udi))
                        {
                            var id = ParserHelper.MapDocumentUdiToId(this.contentService, this.cacheProvider, udi);

                            if (id > -1)
                            {
                                entities.Add(new LinkedDocumentEntity(id));
                            }
                        }                   
                    }
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<RelatedLinks2Parser>("Error parsing related links", exception);
            }

            return entities;
        }

        /// <summary>
        /// Check if it is a internal link.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsInternalLink(JObject item)
        {
            bool isInternal = false;

            var isInternalProperty = item["isInternal"];

            if (isInternalProperty != null)
            {
                isInternal = isInternalProperty.ToObject<bool>();
            }

            return isInternal;
        }

        /// <summary>
        /// Gets the internal id.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetInternalId(JObject item)
        {
            var internalIdProperty = item["internal"];

            return internalIdProperty?.ToObject<string>();
        }
    }
}
