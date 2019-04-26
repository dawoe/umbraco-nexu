namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Parsers.Helpers;

    /// <summary>
    /// Represents the rich text editor parser
    /// </summary>
    public class RichTextEditorParser : IPropertyValueParser
    {
        /// <inheritdoc />
        public bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.TinyMce);
        }

        /// <inheritdoc />
        public IEnumerable<IRelatedEntity> GetRelatedEntities(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Enumerable.Empty<IRelatedEntity>();
            }

            var relatedEntities = new List<IRelatedEntity>();
            var utilities = new ParserUtilities();
                      

            foreach (var documentUdi in utilities.GetDocumentUdiFromText(value))
            {
                relatedEntities.Add(new RelatedDocumentEntity
                                        {
                                            RelatedEntityUdi = documentUdi
                                        });
            }

            foreach (var mediaUdi in utilities.GetMediaUdiFromText(value))
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
