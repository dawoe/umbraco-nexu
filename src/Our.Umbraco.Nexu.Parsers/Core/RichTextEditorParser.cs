namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Parsers.Helpers;

    /// <summary>
    /// Represents the rich text editor parser
    /// </summary>
    public class RichTextEditorParser : IPropertyValueParser
    {
        /// <inheritdoc />
        public bool IsParserFor(Property property)
        {
            return property.PropertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.TinyMce);
        }

        /// <inheritdoc />
        public IEnumerable<IRelatedEntity> GetRelatedEntities(Property property)
        {
            if (property.Values != null && property.Values.Any())
            {
                var relatedEntities = new List<IRelatedEntity>();

                var utilities = new ParserUtilities();

                foreach (var value in property.Values)
                {
                    var documentUdis = utilities.GetDocumentUdiFromText(value.EditedValue.ToString());

                    foreach (var documentUdi in documentUdis.DistinctBy(x => x.ToString()))
                    {
                        relatedEntities.Add(new RelatedDocumentEntity
                                                {
                                                    Culture = value.Culture,
                                                    RelatedEntityUdi = documentUdi
                                                });
                    }

                    var mediaUdis = utilities.GetMediaUdiFromText(value.EditedValue.ToString());

                    foreach (var mediaUdi in mediaUdis.DistinctBy(x => x.ToString()))
                    {
                        relatedEntities.Add(new RelatedMediaEntity()
                                                {
                                                    Culture = value.Culture,
                                                    RelatedEntityUdi = mediaUdi
                                                });
                    }

                }

                return relatedEntities;
            }

            return Enumerable.Empty<IRelatedEntity>();
        }
    }
}
