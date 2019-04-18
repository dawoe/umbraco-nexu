namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Parsers.Helpers;

    /// <summary>
    /// Represents content picker parser.
    /// </summary>
    public class ContentPickerParser : IPropertyValueParser
    {
        /// <inheritdoc />
        public bool IsParserFor(Property property)
        {
            return property.PropertyType.PropertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.ContentPicker);
        }

        /// <inheritdoc />
        public IEnumerable<IRelatedEntity> GetRelatedEntities(Property property)
        {
            if (property.Values != null && property.Values.Any())
            {
                var relatedEntities = new List<IRelatedEntity>();
                                
                foreach (var value in property.Values.WhereNotNull())
                {
                    if (!string.IsNullOrWhiteSpace(value.EditedValue?.ToString()))
                    {
                        relatedEntities.Add(new RelatedDocumentEntity
                                                {
                                                    Culture = value.Culture,
                                                    RelatedEntityUdi = new StringUdi(new Uri(value.EditedValue.ToString()))
                                                });
                    }

                }

                return relatedEntities;
            }

            return Enumerable.Empty<IRelatedEntity>();
        }
    }
}
