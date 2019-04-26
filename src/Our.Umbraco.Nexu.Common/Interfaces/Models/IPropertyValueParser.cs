namespace Our.Umbraco.Nexu.Common.Interfaces.Models
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Composing;

    /// <summary>
    /// Represents the PropertyValueParser interface.
    /// </summary>
    public interface IPropertyValueParser : IDiscoverable
    {
        /// <summary>
        /// Checks if this is the parser for the current property editor
        /// </summary>
        /// <param name="propertyEditorAlias">
        /// The property Editor Alias.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsParserFor(string propertyEditorAlias);

        /// <summary>
        /// Gets the related entities from the property value
        /// </summary>
        /// <param name="value">
        /// The property value
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IRelatedEntity}"/>.
        /// </returns>
        IEnumerable<IRelatedEntity> GetRelatedEntities(string value);
    }
}
