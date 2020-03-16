namespace Our.Umbraco.Nexu.Parsers
{
    using System.Collections.Generic;
    using System.Linq;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Parsers.Helpers;

    /// <summary>
    /// Represents the base text parser class
    /// </summary>
    public abstract class BaseTextParser : IPropertyValueParser
    {
        /// <inheritdoc />
        public abstract bool IsParserFor(string propertyEditorAlias);

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
