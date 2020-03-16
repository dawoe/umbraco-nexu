namespace Our.Umbraco.Nexu.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Interfaces.Repositories;
    using Our.Umbraco.Nexu.Common.Interfaces.Services;
    using Our.Umbraco.Nexu.Common.Models;
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
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The relation repository.
        /// </summary>
        private readonly IRelationRepository relationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuEntityParsingService"/> class.
        /// </summary>
        /// <param name="propertyValueParserCollection">
        /// The property value parser collection.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="relationRepository">
        /// The relation Repository.
        /// </param>
        public NexuEntityParsingService(PropertyValueParserCollection propertyValueParserCollection, ILogger logger, IRelationRepository relationRepository)
        {
            this.propertyValueParserCollection = propertyValueParserCollection;
            this.logger = logger;
            this.relationRepository = relationRepository;
        }        
        
        /// <inheritdoc />
        public void ParseContent(IContent content)
        {
            if (content.Blueprint)
            {
                return;
            }

            var relations = new List<NexuRelation>();

            try
            {
                relations.AddRange(this.GetRelatedEntitiesFromContent(content));
            }
            catch (Exception ex)
            {
                this.logger.Error<NexuEntityParsingService>($"Something went wrong parsing content with id {content.Id.ToString()}", ex);
                return;
            }

            try
            {
                this.SaveRelationsForContentItem(content, relations);
            }
            catch (Exception ex)
            {
                this.logger.Error<NexuEntityParsingService>($"Something went wrong saving relations for content with id {content.Id.ToString()}", ex);
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
                    cultureValue.EditedValue).ToList();

                if (entities.Any())
                {
                    relationsByCulture.Add(cultureValue.Culture ?? "invariant", entities);
                }                
            }

            return relationsByCulture;
        }

        /// <summary>
        /// Gets the related entities from a content item
        /// </summary>
        /// <param name="content">
        /// The content item to get the relations from
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public virtual IEnumerable<NexuRelation> GetRelatedEntitiesFromContent(IContent content)
        {
            var entities = new List<NexuRelation>();

            foreach (var prop in content.Properties.ToList())
            {
                var relatedEntities = this.GetRelatedEntitiesFromProperty(prop);

                foreach (var language in relatedEntities.Keys.ToList())
                {
                    foreach (var entity in relatedEntities[language].ToList())
                    {
                        entities.Add(new NexuRelation
                                         {
                                             ParentUdi = content.GetUdi().ToString(),
                                             ChildUdi = entity.RelatedEntityUdi.ToString(),
                                             RelationType = entity.RelationType,
                                             Culture = language,
                                             PropertyAlias = prop.Alias
                                         });
                    }
                }
            }

            return entities;
        }

        /// <summary>
        /// Saves relations for content item.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="relations">
        /// The relations.
        /// </param>
        public virtual void SaveRelationsForContentItem(IContent content, IEnumerable<NexuRelation> relations)
        {
            var relationList = relations.ToList();

            foreach (var relation in relationList)
            {
                if (relation.Culture == "invariant")
                {
                    relation.Culture = null;
                }
            }

            this.relationRepository.PersistRelationsForContentItem(content.GetUdi(), relationList);
        }
    }
}
