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
    /// Archetype property parser.
    /// </summary>
    public class ArchetypeParser : IPropertyParser
    {
        private readonly IDataTypeService _dataTypeService;
        private Dictionary<string, List<KeyValuePair<string, Guid>>> _aliasToIdMappings;
        private List<int> _processedDataTypeIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchetypeParser"/> class.
        /// </summary>
        public ArchetypeParser()
        {
            this._dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            this._aliasToIdMappings = new Dictionary<string, List<KeyValuePair<string, Guid>>>();
            this._processedDataTypeIds = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchetypeParser"/> class.
        /// </summary>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public ArchetypeParser(IDataTypeService dataTypeService)
        {
            this._dataTypeService = dataTypeService;
            this._aliasToIdMappings = new Dictionary<string, List<KeyValuePair<string, Guid>>>();
            this._processedDataTypeIds = new List<int>();
        }

        /// <inheritdoc />
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            var isArchetype = dataTypeDefinition.PropertyEditorAlias.Equals("Imulus.Archetype");
            if (isArchetype && !this._processedDataTypeIds.Contains(dataTypeDefinition.Id))
            {
                var preValues = this._dataTypeService.GetPreValuesByDataTypeId(dataTypeDefinition.Id).FirstOrDefault();
                if (preValues != null)
                {
                    var data = JsonConvert.DeserializeObject<JObject>(preValues);
                    foreach (var fieldset in data["fieldsets"])
                    {
                        var fieldsetAlias = fieldset["alias"].ToString();
                        if (!this._aliasToIdMappings.ContainsKey(fieldsetAlias))
                        {
                            this._aliasToIdMappings.Add(fieldsetAlias, new List<KeyValuePair<string, Guid>>());
                        }
                        foreach (var property in fieldset["properties"])
                        {
                            this._aliasToIdMappings[fieldsetAlias].Add(new KeyValuePair<string, Guid>(property["alias"].ToString(), new Guid(property["dataTypeGuid"].ToString())));
                        }
                    }
                }
                this._processedDataTypeIds.Add(dataTypeDefinition.Id);
            }
            return isArchetype;
        }

        /// <inheritdoc />
        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }
            var entities = new List<ILinkedEntity>();
            var model = JsonConvert.DeserializeObject<JObject>(propertyValue.ToString());
            if (model != null)
            {
                try
                {
                    var parsers = new Dictionary<KeyValuePair<string,string>, IPropertyParser>();
                    var dataTypes = new Dictionary<Guid, IDataTypeDefinition>();
                    var modelFieldSets = model["fieldsets"];
                    if (modelFieldSets.Any())
                    {
                        foreach (var fieldset in modelFieldSets)
                        {
                            var items = this._aliasToIdMappings[fieldset["alias"].ToString()];
                            foreach (var prop in items)
                            {
                                IDataTypeDefinition dataType = null;
                                if (dataTypes.ContainsKey(prop.Value))
                                {
                                    dataType = dataTypes[prop.Value];
                                }
                                else
                                {
                                    dataType = this._dataTypeService.GetDataTypeDefinitionById(prop.Value);
                                    dataTypes.Add(prop.Value, dataType);
                                }
                                if (dataType != null)
                                {
                                    var parser = PropertyParserResolver.Current.Parsers.FirstOrDefault(x => x.IsParserFor(dataType));
                                    if (parser != null)
                                    {                                       
                                       parsers.Add(new KeyValuePair<string, string>(prop.Key,fieldset["id"].ToString()), parser);                                            
                                    }
                                }
                            }

                            foreach (var keyValuePair in parsers.Keys)
                            {
                                var item = fieldset["properties"].FirstOrDefault(x=>x["alias"].ToString() == keyValuePair.Key);
                                if (item != null)
                                {
                                    entities.AddRange(parsers[keyValuePair].GetLinkedEntities(item["value"]));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this.GetType(), $"An error occured parsing Archetype property", ex);
                }
            }
            return entities;
        }
    }
}