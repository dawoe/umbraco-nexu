namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Community
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
    /// The stacked content parser.
    /// </summary>
    public class StackedContentParser : IPropertyParser
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
        /// Initializes a new instance of the <see cref="StackedContentParser"/> class.
        /// </summary>
        public StackedContentParser()
        {
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            this.contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackedContentParser"/> class.
        /// </summary>
        /// <param name="contentTypeService">
        /// The content type service.
        /// </param>
        /// <param name="dataTypeService">
        /// The data Type Service.
        /// </param>
        public StackedContentParser(IContentTypeService contentTypeService, IDataTypeService dataTypeService)
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
        public virtual bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return dataTypeDefinition.PropertyEditorAlias.Equals("Our.Umbraco.StackedContent");
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
            if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToString()))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            var entities = new List<ILinkedEntity>();

            var contentTypes = new Dictionary<string, IContentType>();
            var dataTypes = new Dictionary<int, IDataTypeDefinition>();

            try
            {
                var items = JsonConvert.DeserializeObject<List<object>>(propertyValue.ToString());

                for (var i = 0; i < items.Count; i++)
                {
                    var item = (JObject)items[i];

                    var docTypeGuid = this.GetDocTypeGuidFromItem(item);

                    if (!string.IsNullOrEmpty(docTypeGuid))
                    {
                        if (!contentTypes.ContainsKey(docTypeGuid))
                        {
                            contentTypes.Add(
                                docTypeGuid,
                                this.contentTypeService.GetContentType(new Guid(docTypeGuid)));
                        }

                        var doctype = contentTypes[docTypeGuid];

                        // get all datatypes for this content type
                        foreach (var propertyType in doctype.CompositionPropertyTypes)
                        {
                            if (!dataTypes.ContainsKey(propertyType.DataTypeDefinitionId))
                            {
                                // retreive datatype and stash it in dictionary
                                dataTypes.Add(
                                    propertyType.DataTypeDefinitionId,
                                    this.dataTypeService.GetDataTypeDefinitionById(propertyType.DataTypeDefinitionId));
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
                ApplicationContext.Current.ProfilingLogger.Logger.Error<StackedContentParser>(
                    "Error parsing stacked content",
                    exception);
            }

            return entities;
        }

        private string GetDocTypeGuidFromItem(JObject item)
        {
            var guidProperty = item["icContentTypeGuid"];

            return guidProperty?.ToObject<string>();
        }

        private string GetPropertyValueFromItem(JObject item, string propertyAlias)
        {
            var property = item[propertyAlias];

            return property?.ToString();
        }
    }
}
