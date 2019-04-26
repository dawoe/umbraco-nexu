namespace Our.Umbraco.Nexu.Core.Services
{
    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Common.Interfaces.Services;
    using Our.Umbraco.Nexu.Core.Composing.Collections;

    /// <summary>
    /// Represents the nexu entity parsing service
    /// </summary>
    internal class NexuEntityParsingService : IEntityParsingService
    {
        /// <summary>
        /// The property value parser collection.
        /// </summary>
        private readonly PropertyValueParserCollection propertyValueParserCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuEntityParsingService"/> class.
        /// </summary>
        /// <param name="propertyValueParserCollection">
        /// The property value parser collection.
        /// </param>
        public NexuEntityParsingService(PropertyValueParserCollection propertyValueParserCollection)
        {
            this.propertyValueParserCollection = propertyValueParserCollection;
        }

        /// <inheritdoc />
        public void ParseContent(IContent content, bool parseEditedContentOnly = true, bool parseAllCultures = false)
        {
            if (content.Blueprint)
            {
                return;
            }
        }
    }
}
