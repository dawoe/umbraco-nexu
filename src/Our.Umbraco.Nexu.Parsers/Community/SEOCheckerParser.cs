using Our.Umbraco.Nexu.Parsers.Helpers;

namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents content picker parser.
    /// </summary>
    public class SEOCheckerParser : IPropertyValueParser
    {
        /// <inheritdoc />
        public bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals("SEOChecker.SEOCheckerSocialPropertyEditor");
        }

        /// <inheritdoc />
        public IEnumerable<IRelatedEntity> GetRelatedEntities(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var entities = new List<IRelatedEntity>();
                var model = XDocument.Parse(value).Root;
                if (model != null)
                {
                    try
                    {
                        var socialImage = model.Descendants("socialImage").FirstOrDefault();
                        if (socialImage == null)
                        {
                            return Enumerable.Empty<IRelatedEntity>();
                        }

                        if (string.IsNullOrEmpty(socialImage.Value))
                        {
                            return Enumerable.Empty<IRelatedEntity>();
                        }

                        foreach (var documentUdi in ParserUtilities.GetMediaUdiFromText(socialImage.Value).ToList())
                        {
                            entities.Add(new RelatedMediaEntity
                            {
                                RelatedEntityUdi = documentUdi,
                            });
                        }

                        return entities;
                    }
                    catch { }
                }
            }

            return Enumerable.Empty<IRelatedEntity>();
        }
    }
}
