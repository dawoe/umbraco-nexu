namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Interfaces;

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
            return false;
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
            return Enumerable.Empty<ILinkedEntity>();
        }
    }
}
