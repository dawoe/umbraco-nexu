namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Community
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
    using Our.Umbraco.Nexu.Core.Models;
    using Our.Umbraco.Nexu.Core.ObjectResolution;

    /// <summary>
    /// The le blender grid editor parser.
    /// </summary>
    public class LeBlenderGridEditorParser : IGridEditorParser
    {

        /// <summary>
        /// The data type service.
        /// </summary>
        private readonly IDataTypeService dataTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeBlenderGridEditorParser"/> class.
        /// </summary>
        public LeBlenderGridEditorParser()
        {
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeBlenderGridEditorParser"/> class.
        /// </summary>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public LeBlenderGridEditorParser(IDataTypeService dataTypeService)
        {
            this.dataTypeService = dataTypeService;
        }

        /// <summary>
        /// Check if it is a parser for a speficic editor
        /// </summary>
        /// <param name="editorview">
        /// The editor view alias as defined in grid config
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(string editorview)
        {
            return editorview.Equals("/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html");
        }

        /// <summary>
        /// Gets the linked entities for this editor
        /// </summary>
        /// <param name="value">
        /// The grid editor value
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            var linkedEntities = new List<ILinkedEntity>();

            try
            {
                var dataTypes = new Dictionary<string, IDataTypeDefinition>();

                var jsonValue = JsonConvert.DeserializeObject<JArray>(value);

                foreach (var item in jsonValue)
                {
                    foreach (JProperty field in item)
                    {
                        var dataTypeGuid = field.Value["dataTypeGuid"].ToString();

                        IDataTypeDefinition dataType = null;

                        if (dataTypes.ContainsKey(dataTypeGuid))
                        {
                            dataType = dataTypes[dataTypeGuid];
                        }
                        else
                        {
                            dataType =
                                this.dataTypeService.GetDataTypeDefinitionById(
                                    new Guid(dataTypeGuid));

                            dataTypes.Add(dataTypeGuid, dataType);
                        }

                        var parser =
                            PropertyParserResolver.Current.Parsers.FirstOrDefault(
                                x => x.IsParserFor(dataType));

                        if (parser != null)
                        {
                            linkedEntities.AddRange(parser.GetLinkedEntities(field.Value["value"]));
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<LeBlenderGridEditorParser>("Error parsing le blender grid editor. Property data : " + value, exception);
            }

            return linkedEntities;
        }
    }
}
