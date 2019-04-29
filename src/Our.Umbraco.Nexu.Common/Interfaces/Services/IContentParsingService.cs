namespace Our.Umbraco.Nexu.Common.Interfaces.Services
{
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
    }
}
