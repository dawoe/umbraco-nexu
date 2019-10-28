namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.ObjectResolution;

    /// <summary>
    /// The nested content parser.
    /// </summary>
    public class NestedContentParser : IPropertyParser
    {
        /// <summary>
        /// The content type service.
        /// </summary>
        private readonly IContentTypeService contentTypeService;

        /// <summary>
        /// The data type service.
        /// </summary>
        private readonly IDataTypeService dataTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NestedContentParser"/> class.
        /// </summary>
        public NestedContentParser()
        {
            this.contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NestedContentParser"/> class.
        /// </summary>
        /// <param name="contentTypeService">
        /// The content type service.
        /// </param>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public NestedContentParser(IContentTypeService contentTypeService, IDataTypeService dataTypeService)
        {
            this.contentTypeService = contentTypeService;
            this.dataTypeService = dataTypeService;
        }

        /// <summary>
        /// Check if it's a parser for a data type definition
        /// </summary>
        /// <param name="dataTypeDefinition">
        /// The data type definition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return dataTypeDefinition.PropertyEditorAlias.Equals("Umbraco.NestedContent");
        }

        /// <summary>
        /// Gets the linked entites from the property value
        /// </summary>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            if (propertyValue == null || propertyValue.ToString().IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            var entities = new List<ILinkedEntity>();

            var contentTypes = new Dictionary<string, IContentType>();
            var dataTypes = new Dictionary<int, IDataTypeDefinition>();

            try
            {
                var rawValue = JsonConvert.DeserializeObject<List<object>>(propertyValue.ToString());

                for (var i = 0; i < rawValue.Count; i++)
                {
                    var item = (JObject)rawValue[i];

                    var doctypeAlias = this.GetContentTypeAliasFromItem(item);

                    if (!string.IsNullOrEmpty(doctypeAlias))
                    {
                        if (!contentTypes.ContainsKey(doctypeAlias))
                        {
                            // retreive content type and stash it in dictionary
                            contentTypes.Add(
                                doctypeAlias,
                                this.contentTypeService.GetContentType(doctypeAlias));
                        }

                        var contentType = contentTypes[doctypeAlias];

                        // get all datatypes for this content type
                        foreach (var propertyType in contentType.CompositionPropertyTypes)
                        {
                            if (!dataTypes.ContainsKey(propertyType.DataTypeDefinitionId))
                            {
                                // retreive datatype and stash it in dictionary
                                dataTypes.Add(propertyType.DataTypeDefinitionId, this.dataTypeService.GetDataTypeDefinitionById(propertyType.DataTypeDefinitionId));
                            }

                            var dtd = dataTypes[propertyType.DataTypeDefinitionId];

                            // get the parser for the current property
                            var parser = PropertyParserResolver.Current.Parsers.FirstOrDefault(x => x.IsParserFor(dtd));

                            if (parser != null)
                            {
                                var propValue = this.GetPropertyValueFromItem(item, propertyType.Alias);

                                if (propValue != null)
                                {
                                    entities.AddRange(parser.GetLinkedEntities(propValue));
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception exception)
            {                
                ApplicationContext.Current.ProfilingLogger.Logger.Error<NestedContentParser>("Error parsing nested content", exception);
            }

            return entities;
        }

        /// <summary>
        /// Get's content type alias from item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetContentTypeAliasFromItem(JObject item)
        {
            var contentTypeAliasProperty = item["ncContentTypeAlias"];

            return contentTypeAliasProperty?.ToObject<string>();
        }

        /// <summary>
        /// Gets the property value from item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="propertyAlias">
        /// The property alias.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetPropertyValueFromItem(JObject item, string propertyAlias)
        {
            var property = item[propertyAlias];

            return property?.ToString();
        }
    }
}
