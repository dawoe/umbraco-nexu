namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The content picker parser.
    /// </summary>
    public class ContentPickerParser : IPropertyParser
    {
        /// <summary>
        /// Get the linked entities from the property editor data
        /// </summary>
        /// <param name="property">The property to parse</param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(Property property)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Check if it's a parser for this property
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(PropertyType property)
        {
            return property.PropertyEditorAlias.Equals(
                global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias);
        }
    }
}