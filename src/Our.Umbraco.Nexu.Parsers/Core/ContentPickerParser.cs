namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents content picker parser.
    /// </summary>
    public class ContentPickerParser : IPropertyValueParser
    {
        /// <inheritdoc />
        public bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.ContentPicker);
        }

        /// <inheritdoc />
        public IEnumerable<IRelatedEntity> GetRelatedEntities(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var relatedEntities = new List<IRelatedEntity>
                                          {
                                              new RelatedDocumentEntity
                                                  {
                                                      RelatedEntityUdi = new StringUdi(new Uri(value))
                                                  }
                                          };

                return relatedEntities;
            }

            return Enumerable.Empty<IRelatedEntity>();
        }
    }
}
