namespace Our.Umbraco.Nexu.Core.Services
{
    using System.Linq;

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
            var contentNeedsToBeParsed = this.CheckIfContentNeedsToBeParsed(
                content,
                parseEditedContentOnly,
                parseAllCultures);

            if (contentNeedsToBeParsed == false)
            {
                return;
            }

            foreach (var prop in content.Properties)
            {
                var parser = this.propertyValueParserCollection.FirstOrDefault(x => x.IsParserFor(prop.PropertyType.PropertyEditorAlias));

                if (parser != null)
                {
                    foreach (var propValue in prop.Values)
                    {
                        parser.GetRelatedEntities(propValue.EditedValue.ToString());
                    }                  
                }
            }
        }

        public bool CheckIfContentNeedsToBeParsed(IContent content, bool parseEditedContentOnly, bool parseAllCultures)
        {
            if (content.Blueprint)
            {
                return false;
            }

            if (content.Edited == false && parseEditedContentOnly)
            {
                return false;
            }

            return true;
        }
    }
}
