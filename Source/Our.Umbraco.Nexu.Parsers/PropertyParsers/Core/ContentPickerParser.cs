namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The content picker parser.
    /// </summary>
    public class ContentPickerParser : IPropertyParser
    {
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
                 global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias) || dataTypeDefinition.PropertyEditorAlias.Equals("Umbraco.ContentPicker2");
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

            return entities;
        }
    }
}