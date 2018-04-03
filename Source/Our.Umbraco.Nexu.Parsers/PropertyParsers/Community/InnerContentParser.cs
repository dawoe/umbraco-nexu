namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Community
{
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The inner content parser.
    /// </summary>
    public class InnerContentParser : IPropertyParser
    {
        /// <summary>
        /// The content type service.
        /// </summary>
        private readonly IContentTypeService contentTypeService;

        /// <summary>
        /// The data type service.
        /// </summary>
        private readonly IDataTypeService dataTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InnerContentParser"/> class.
        /// </summary>
        public InnerContentParser()
        {
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            this.contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InnerContentParser"/> class.
        /// </summary>
        /// <param name="contentTypeService">
        /// The content type service.
        /// </param>
        /// <param name="dataTypeService">
        /// The data Type Service.
        /// </param>
        public InnerContentParser(IContentTypeService contentTypeService, IDataTypeService dataTypeService)
        {
            this.contentTypeService = contentTypeService;
            this.dataTypeService = dataTypeService;
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
    }
}
