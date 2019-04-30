namespace Our.Umbraco.Nexu.Common.Interfaces.Services
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    /// <summary>
    /// Represents the entity parsing service
    /// </summary>
    internal interface IEntityParsingService
    {
        /// <summary>
        /// Parses a content item for related entities
        /// </summary>
        /// <param name="content">
        /// The content item that needs to be parsed
        /// </param>       
        void ParseContent(IContent content);

        /// <summary>
        /// Gets the parser for a property editor
        /// </summary>
        /// <param name="propertyEditorAlias">
        /// The property editor alias.
        /// </param>
        /// <returns>
        /// The <see cref="IPropertyValueParser"/>.
        /// </returns>
        IPropertyValueParser GetParserForPropertyEditor(string propertyEditorAlias);

        /// <summary>
        /// Gets the related entities from property editor value.
        /// </summary>
        /// <param name="propertyEditorAlias">
        /// The property editor alias.
        /// </param>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IRelatedEntity}"/>.
        /// </returns>
        IEnumerable<IRelatedEntity> GetRelatedEntitiesFromPropertyEditorValue(string propertyEditorAlias, object propertyValue);

        /// <summary>
        /// Gets related entities from a content property.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="IDictionary{T,U}"/>.
        /// </returns>
        IDictionary<string, IEnumerable<IRelatedEntity>> GetRelatedEntitiesFromProperty(Property property);
    }
}
