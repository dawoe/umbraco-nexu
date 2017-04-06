namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;

    using global::Umbraco.Core;
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
        /// Initializes a new instance of the <see cref="ContentPickerParser"/> class.
        /// </summary>
        public ContentPickerParser()
        {
            this.contentService = ApplicationContext.Current.Services.ContentService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPickerParser"/> class.
        /// </summary>
        /// <param name="contentService">
        /// The content service.
        /// </param>
        public ContentPickerParser(IContentService contentService)
        {
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
                // parsing to int failed so could be new udi format in v7.6
                if (propertyValue.ToString().StartsWith("umb://document"))
                {
                    var key = propertyValue.ToString().TrimStart("umb://document/");

                    var attemptGuid = key.TryConvertTo<Guid>();

                    if (attemptGuid.Success)
                    {
                        var content = this.contentService.GetById(attemptGuid.Result);

                        if (content != null)
                        {
                            entities.Add(new LinkedDocumentEntity(content.Id));
                        }
                    }
                }
            }

            return entities;
        }
    }
}