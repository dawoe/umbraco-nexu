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
    /// The vorto parser.
    /// </summary>
    public class VortoParser : IPropertyParser
    {
        /// <summary>
        /// The data type service.
        /// </summary>
        private readonly IDataTypeService dataTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VortoParser"/> class.
        /// </summary>
        public VortoParser()
        {
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VortoParser"/> class.
        /// </summary>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public VortoParser(IDataTypeService dataTypeService)
        {
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
            return dataTypeDefinition.PropertyEditorAlias.Equals("Our.Umbraco.Vorto");
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
            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }
            
            var entities = new List<ILinkedEntity>();

            try
            {
                var item = JsonConvert.DeserializeObject<JObject>(propertyValue.ToString());

                var vortoDataTypeGuid = this.GetDatatypeGuidFromItem(item);

                var vortoDataType = this.dataTypeService.GetDataTypeDefinitionById(new Guid(vortoDataTypeGuid));

                var preValues =
                    this.dataTypeService.GetPreValuesCollectionByDataTypeId(vortoDataType.Id).FormatAsDictionary();

                var editorDataTypeGuid = JsonConvert.DeserializeObject<JObject>(preValues["dataType"].Value).Value<string>("guid");

                if (!string.IsNullOrEmpty(editorDataTypeGuid))
                {
                    var editorDataype = this.dataTypeService.GetDataTypeDefinitionById(new Guid(editorDataTypeGuid));

                    if (editorDataype != null)
                    {
                        var parser = PropertyParserResolver.Current.Parsers.FirstOrDefault(x => x.IsParserFor(editorDataype));

                        if (parser != null)
                        {
                            var values = this.GetValuesForLanguages(item);

                            foreach (var value in values)
                            {
                                entities.AddRange(parser.GetLinkedEntities(value));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<VortoParser>("Error parsing vorto", exception);     
            }
           

            return entities;
        }

        /// <summary>
        /// Gets the datatype guid from the item
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDatatypeGuidFromItem(JObject item)
        {
            var dtdGuid = item["dtdGuid"];

            return dtdGuid?.ToObject<string>();
        }

        /// <summary>
        /// Gets property values for all languages
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<object> GetValuesForLanguages(JObject item)
        {
            var propertyValues = new List<object>();

            var values = item["values"];

            if (values != null)
            {
                foreach (var lang in values)
                {
                    var langValue = lang.Values().First();

                    if (langValue != null)
                    {
                        propertyValues.Add(langValue.ToString());
                    }
                }
            }


            return propertyValues;
        }
    }
}
