namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Parsers.Helpers;

    /// <summary>
    /// Represents the parser for the grid editor
    /// </summary>
    public class GridParser : IPropertyValueParser
    {
        /// <inheritdoc />
        public bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.Grid);
        }

        /// <inheritdoc />
        public IEnumerable<IRelatedEntity> GetRelatedEntities(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Enumerable.Empty<IRelatedEntity>();
            }

            var relatedEntities = new List<IRelatedEntity>();

            foreach (var documentUdi in ParserUtilities.GetDocumentUdiFromText(value).ToList())
            {
                relatedEntities.Add(new RelatedDocumentEntity
                                        {
                                            RelatedEntityUdi = documentUdi
                                        });
            }

            foreach (var mediaUdi in ParserUtilities.GetMediaUdiFromText(value).ToList())
            {
                relatedEntities.Add(new RelatedMediaEntity()
                                        {
                                            RelatedEntityUdi = mediaUdi
                                        });
            }

            return relatedEntities;
        }
    }
}
