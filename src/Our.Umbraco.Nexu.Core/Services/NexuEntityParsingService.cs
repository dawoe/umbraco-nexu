namespace Our.Umbraco.Nexu.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
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
        public void ParseContent(IContent content)
        {
            if (content.Blueprint)
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

        /// <summary>
        /// Gets the parser for a property editor
        /// </summary>
        /// <param name="propertyEditorAlias">
        /// The property editor alias.
        /// </param>
        /// <returns>
        /// The <see cref="IPropertyValueParser"/>.
        /// </returns>
        public virtual IPropertyValueParser GetParserForPropertyEditor(string propertyEditorAlias)
        {
            return this.propertyValueParserCollection.FirstOrDefault(x => x.IsParserFor(propertyEditorAlias));
        }

        /// <summary>
        /// Gets the related entities from property editor value.
        /// </summary>
        /// <param name="propertyEditorAlias">
        /// The property editor alias.
        /// </param>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IRelatedEntity}"/>.
        /// </returns>
        public virtual IEnumerable<IRelatedEntity> GetRelatedEntitiesFromPropertyEditorValue(string propertyEditorAlias, object propertyValue)
        {
            if (!string.IsNullOrWhiteSpace(propertyValue?.ToString()))
            {
                var parser = this.GetParserForPropertyEditor(propertyEditorAlias);

                if (parser != null)
                {
                    return parser.GetRelatedEntities(propertyValue.ToString()).DistinctBy(x => x.RelatedEntityUdi.ToString());
                }
            }           

            return Enumerable.Empty<IRelatedEntity>();
        }

        /// <summary>
        /// Gets related entities from a content property.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="IDictionary{T,U}"/>.
        /// </returns>
        public virtual IDictionary<string, IEnumerable<IRelatedEntity>> GetRelatedEntitiesFromProperty(Property property)
        {
            var relationsByCulture = new Dictionary<string, IEnumerable<IRelatedEntity>>();


            var editorAlias = property.PropertyType.PropertyEditorAlias;

            foreach (var cultureValue in property.Values)
            {
                var entities = this.GetRelatedEntitiesFromPropertyEditorValue(
                    editorAlias,
                    cultureValue.EditedValue);

                relationsByCulture.Add(cultureValue.Culture, entities);
            }

            return relationsByCulture;
        }
    }
}
