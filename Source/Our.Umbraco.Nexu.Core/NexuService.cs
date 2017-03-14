namespace Our.Umbraco.Nexu.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Constants;
    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;
    using Our.Umbraco.Nexu.Resolvers;

    /// <summary>
    /// Nexu service
    /// </summary>
    internal class NexuService : INexuService
    {
        /// <summary>
        /// Internal service instance
        /// </summary>
        private static NexuService service;

        /// <summary>
        /// The profiler.
        /// </summary>
        private ProfilingLogger profiler;

        /// <summary>
        /// The property parser resolver.
        /// </summary>
        private PropertyParserResolver propertyParserResolver;

        /// <summary>
        /// The relation service.
        /// </summary>
        private IRelationService relationService;

        /// <summary>
        /// The data type service.
        /// </summary>
        private IDataTypeService dataTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuService"/> class. 
        /// </summary>
        /// <param name="profiler">
        /// The profiler.
        /// </param>
        /// <param name="relationService">
        /// The relation Service.
        /// </param>
        /// <param name="propertyParserResolver">
        /// The property Parser Resolver.
        /// </param>
        /// <param name="dataTypeService">
        /// The data Type Service.
        /// </param>
        public NexuService(
            ProfilingLogger profiler,
            IRelationService relationService,
            PropertyParserResolver propertyParserResolver,
            IDataTypeService dataTypeService)
        {
            this.profiler = profiler;
            this.relationService = relationService;
            this.propertyParserResolver = propertyParserResolver;
            this.dataTypeService = dataTypeService;
            service = this;
        }

        /// <summary>
        /// The current nexu service instance
        /// </summary>
        public static NexuService Current
            =>
            service
            ?? new NexuService(
                   global::Umbraco.Core.ApplicationContext.Current.ProfilingLogger,
                   global::Umbraco.Core.ApplicationContext.Current.Services.RelationService,
                   PropertyParserResolver.Current,
                   global::Umbraco.Core.ApplicationContext.Current.Services.DataTypeService);

        /// <summary>
        /// Gets all property parsrs
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public virtual IEnumerable<IPropertyParser> GetAllPropertyParsers()
        {
            return this.propertyParserResolver.Parsers;
        }

        /// <summary>
        /// Get the linked entities for a content item
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary{S,T}"/>.
        /// </returns>
        public virtual Dictionary<string, IEnumerable<ILinkedEntity>> GetLinkedEntitesForContent(IContent content)
        {
            using (
                this.profiler.DebugDuration<NexuService>(
                    $"Started getting linked entities from content item \"{content.Name}\" with id {content.Id}",
                    $"Completed getting linked entities from content item \"{content.Name}\" with id {content.Id}"))
            {
               var linkedEntities = new Dictionary<string, IEnumerable<ILinkedEntity>>();

                // get all parsers for this content item
                var parsableProperties = this.GetParsablePropertiesForContent(content).ToList();

                if (!parsableProperties.Any())
                {
                    // if no parsers found, exit
                    return linkedEntities;
                }

                // get linked entities fro the properties that we found a parsers for
                parsableProperties.ForEach(
                    pp =>
                        {
                            linkedEntities.Add(pp.Property.PropertyType.Name, pp.Parser.GetLinkedEntities(pp.Property.Value).ToList());                            
                        });

                return linkedEntities;
            }
        }

        /// <summary>
        /// Get all properties of content item we have a parser for
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public virtual IEnumerable<PropertyWithParser> GetParsablePropertiesForContent(IContent content)
        {
            var properties = new List<PropertyWithParser>();

            if (content == null)
            {
                return properties;
            }

            var parsers = this.GetAllPropertyParsers().ToList();

            if (parsers.Any())
            {
                content.Properties.ForEach(
                    p =>
                        {
                            var dtd = this.dataTypeService.GetDataTypeDefinitionById(
                                p.PropertyType.DataTypeDefinitionId);
                            var parser = parsers.FirstOrDefault(x => x.IsParserFor(dtd));

                            if (parser != null)
                            {
                                properties.Add(new PropertyWithParser(p, parser));
                            }
                        });
            }

            return properties;
        }

        /// <summary>
        /// Delete all relations for content.
        /// </summary>
        /// <param name="contentid">
        /// The contentid.
        /// </param>
        public virtual void DeleteRelationsForContent(int contentid)
        {
            var relations = this.GetNexuRelationsForContent(contentid).ToList();

            foreach (var relation in relations)
            {
                this.relationService.Delete(relation);
            }
        }

        /// <summary>
        /// Gets all nexu relations for content.
        /// </summary>
        /// <param name="contentId">
        /// The content id.
        /// </param>
        /// <param name="isParent">
        /// The is Parent.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public virtual IEnumerable<IRelation> GetNexuRelationsForContent(int contentId, bool isParent = true)
        {
            var relations = Enumerable.Empty<IRelation>().ToList();

            relations = isParent ? this.relationService.GetByParentId(contentId).ToList() : this.relationService.GetByChildId(contentId).ToList();                
            
            if (!relations.Any())
            {
                return relations;
            }

            return
                relations.Where(
                    x =>
                        x.RelationType.Alias == RelationTypes.DocumentToDocumentAlias
                        || x.RelationType.Alias == RelationTypes.DocumentToMediaAlias);
        }

        /// <summary>
        /// Saves linked entities as relations
        /// </summary>
        /// <param name="contentid">
        /// The contentid.
        /// </param>
        /// <param name="linkedEntities">
        /// The linked entities.
        /// </param>
        public virtual void SaveLinkedEntitiesAsRelations(int contentid, Dictionary<string, IEnumerable<ILinkedEntity>> linkedEntities)
        {
            var docToDocRelationType = this.relationService.GetRelationTypeByAlias(
                RelationTypes.DocumentToDocumentAlias);

            var docToMediaRelationType = this.relationService.GetRelationTypeByAlias(RelationTypes.DocumentToMediaAlias);

            var entitiesToSave = new Dictionary<int, Relation>();


            // loop trough entities, we only need to create one relation per linked item, but we store propery names in comments
            foreach (var item in linkedEntities)
            {
                var property = item.Key;

                foreach (var entity in item.Value)
                {
                    if (!entitiesToSave.ContainsKey(entity.Id))
                    {
                        entitiesToSave.Add(
                            entity.Id,
                            new Relation(
                                contentid,
                                entity.Id,
                                entity.LinkedEntityType == LinkedEntityType.Document
                                    ? docToDocRelationType
                                    : docToMediaRelationType) { Comment = property });
                    }
                    else
                    {
                        entitiesToSave[entity.Id].Comment += $" ,{property}";
                    }
                }
            }

            foreach (var item in entitiesToSave)
            {
                try
                {
                    this.relationService.Save(item.Value);
                }
                catch
                {
                    // ignore errors, can occure when try to create relation for non existing content
                }
            }
        }

        /// <summary>
        /// Parses content and saves linked entitites
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public void ParseContent(IContent content)
        {
            if (content == null)
            {
                return;
            }

            using (
              this.profiler.DebugDuration<NexuService>(
                  $"Started parse content for content item \"{content.Name}\" with id {content.Id}",
                  $"Completed parse content for  content item \"{content.Name}\" with id {content.Id}"))
            {
                // get all linked entities
                var linkedEntities = this.GetLinkedEntitesForContent(content);

                // delete all existing relations
                this.DeleteRelationsForContent(content.Id);

                // save all linked entities
                this.SaveLinkedEntitiesAsRelations(content.Id, linkedEntities);
            }
        }

        /// <summary>
        /// Sets up the needed the relation types
        /// </summary>
        public void SetupRelationTypes()
        {
            using (this.profiler.DebugDuration<NexuService>("Begin SetupRelationTypes", "End SetupRelationTypes"))
            {
                this.SetupDocumentToDocumentRelationType();
                this.SetupDocumentToMediaRelationType();
            }
        }

        /// <summary>
        /// Creates a relation type.
        /// </summary>
        /// <param name="alias">
        /// The alias of the relation type
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="childObjectType">
        /// The child object type.
        /// </param>
        private void CreateRelationType(string alias, string name, Guid childObjectType)
        {
            var relationType = new RelationType(
                                   childObjectType,
                                   new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document),
                                   alias,
                                   name);

            this.relationService.Save(relationType);
        }

        /// <summary>
        /// Sets up document to document relation type.
        /// </summary>
        private void SetupDocumentToDocumentRelationType()
        {
            if (this.relationService.GetRelationTypeByAlias(RelationTypes.DocumentToDocumentAlias) != null)
            {
                return;
            }

            this.CreateRelationType(
                RelationTypes.DocumentToDocumentAlias,
                RelationTypes.DocumentToDocumentName,
                new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document));
        }

        /// <summary>
        /// Sets up the document to media relation type.
        /// </summary>
        private void SetupDocumentToMediaRelationType()
        {
            if (this.relationService.GetRelationTypeByAlias(RelationTypes.DocumentToMediaAlias) != null)
            {
                return;
            }

            this.CreateRelationType(
                RelationTypes.DocumentToMediaAlias,
                RelationTypes.DocumentToMediaName,
                new Guid(global::Umbraco.Core.Constants.ObjectTypes.Media));
        }
    }
}