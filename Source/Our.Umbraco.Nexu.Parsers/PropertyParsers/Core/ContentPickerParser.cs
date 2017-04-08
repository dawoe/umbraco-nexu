namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The content picker parser.
    /// </summary>
    public class ContentPickerParser : IPropertyParser
    {
        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly ICacheProvider cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPickerParser"/> class.
        /// </summary>
        public ContentPickerParser()
        {
            this.contentService = ApplicationContext.Current.Services.ContentService;
            this.cache = ApplicationContext.Current.ApplicationCache.StaticCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPickerParser"/> class.
        /// </summary>
        /// <param name="contentService">
        /// The content service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache Provider.
        /// </param>
        public ContentPickerParser(IContentService contentService, ICacheProvider cacheProvider)
        {
            this.contentService = contentService;
            this.cache = cacheProvider;
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
            return dataTypeDefinition.PropertyEditorAlias.Equals(
                 Constants.PropertyEditors.ContentPickerAlias) || dataTypeDefinition.PropertyEditorAlias.Equals("Umbraco.ContentPicker2");
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
            var entities = new List<LinkedDocumentEntity>();

            if (propertyValue == null)
            {
                return entities;
            }

            var attemptInt = propertyValue.TryConvertTo<int>();

            if (attemptInt.Success)
            {
                entities.Add(new LinkedDocumentEntity(attemptInt.Result));
            }
            else
            {
                if (ParserHelper.IsDocumentUdi(propertyValue.ToString()))
                {
                    var id = ParserHelper.MapDocumentUdiToId(this.contentService, this.cache, propertyValue.ToString());

                    if (id > -1)
                    {
                        entities.Add(new LinkedDocumentEntity(id));
                    }
                }                
            }

            return entities;
        }
    }
}