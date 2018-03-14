using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.Nexu.Core.Interfaces;
using Our.Umbraco.Nexu.Core.Models;
using Our.Umbraco.Nexu.Core.ObjectResolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Impinj.Web.App_Plugins.NexuExtensions {
    public class ArchetypeParser : IPropertyParser {
        private readonly IDataTypeService _dataTypeService;
        private Dictionary<string, List<KeyValuePair<string, Guid>>> _aliasToIdMappings;
        private List<int> _processedDataTypeIds;

        public ArchetypeParser() {
            _dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            _aliasToIdMappings = new Dictionary<string, List<KeyValuePair<string, Guid>>>();
            _processedDataTypeIds = new List<int>();
        }

        public ArchetypeParser(IDataTypeService dataTypeService) {
            this._dataTypeService = dataTypeService;
            this._aliasToIdMappings = new Dictionary<string, List<KeyValuePair<string, Guid>>>();
            this._processedDataTypeIds = new List<int>();
        }

        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition) {
            var isArchetype = dataTypeDefinition.PropertyEditorAlias.Equals("Imulus.Archetype");
            if (isArchetype && !_processedDataTypeIds.Contains(dataTypeDefinition.Id)) {
                var preValues = _dataTypeService.GetPreValuesByDataTypeId(dataTypeDefinition.Id).FirstOrDefault();
                if (preValues != null) {
                    var data = JsonConvert.DeserializeObject<JObject>(preValues);
                    foreach (var fieldset in data["fieldsets"]) {
                        var fieldsetAlias = fieldset["alias"].ToString();
                        if (!_aliasToIdMappings.ContainsKey(fieldsetAlias)) {
                            _aliasToIdMappings.Add(fieldsetAlias, new List<KeyValuePair<string, Guid>>());
                        }
                        foreach (var property in fieldset["properties"]) {
                            _aliasToIdMappings[fieldsetAlias].Add(new KeyValuePair<string, Guid>(property["alias"].ToString(), new Guid(property["dataTypeGuid"].ToString())));
                        }
                    }
                }
                _processedDataTypeIds.Add(dataTypeDefinition.Id);
            }
            return isArchetype;
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue) {
            if (string.IsNullOrEmpty(propertyValue?.ToString())) {
                return Enumerable.Empty<ILinkedEntity>();
            }
            var entities = new List<ILinkedEntity>();
            var model = JsonConvert.DeserializeObject<JObject>(propertyValue.ToString());
            if (model != null) {
                try {
                    var parsers = new Dictionary<string, IPropertyParser>();
                    var dataTypes = new Dictionary<Guid, IDataTypeDefinition>();
                    if (model["fieldsets"].Count() > 0) {
                        var items = _aliasToIdMappings[model["fieldsets"].First()["alias"].ToString()];
                        foreach (var prop in items) {
                            IDataTypeDefinition dataType = null;
                            if (dataTypes.ContainsKey(prop.Value)) {
                                dataType = dataTypes[prop.Value];
                            }
                            else {
                                dataType = _dataTypeService.GetDataTypeDefinitionById(prop.Value);
                                dataTypes.Add(prop.Value, dataType);
                            }
                            if (dataType != null) {
                                var parser = PropertyParserResolver.Current.Parsers.FirstOrDefault(x => x.IsParserFor(dataType));
                                if (parser != null) {
                                    parsers.Add(prop.Key, parser);
                                }
                            }
                        }
                        foreach (var fieldset in model["fieldsets"]) {
                            foreach (var alias in parsers.Keys) {
                                var item = fieldset["properties"].Where(x => x["alias"].ToString() == alias).FirstOrDefault();
                                if (item != null) {
                                    entities.AddRange(parsers[alias].GetLinkedEntities(item["value"]));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    LogHelper.Error(this.GetType(), $"An error occured parsing Archetype property", ex);
                }
            }
            return entities;
        }
    }
}