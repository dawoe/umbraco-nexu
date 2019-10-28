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
    using Our.Umbraco.Nexu.Core.ObjectResolution;

    /// <summary>
    /// The doc type grid editor parser.
    /// </summary>
    public class DocTypeGridEditorParser : IGridEditorParser
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
        /// Initializes a new instance of the <see cref="DocTypeGridEditorParser"/> class.
        /// </summary>
        public DocTypeGridEditorParser()
        {
            this.contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocTypeGridEditorParser"/> class.
        /// </summary>
        /// <param name="contentTypeService">
        /// The content type service.
        /// </param>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public DocTypeGridEditorParser(IContentTypeService contentTypeService, IDataTypeService dataTypeService)
        {
            this.contentTypeService = contentTypeService;
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
            return editorview.ToLowerInvariant().Equals("/app_plugins/doctypegrideditor/views/doctypegrideditor.html");
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

            var entities = new List<ILinkedEntity>();

            try
            {
                var dataTypes = new Dictionary<int, IDataTypeDefinition>();

                var jsonValue = JsonConvert.DeserializeObject<JObject>(value);

                var doctypeAlias = jsonValue["dtgeContentTypeAlias"].ToString();

                if (!string.IsNullOrEmpty(doctypeAlias))
                {
                    var contentType = this.contentTypeService.GetContentType(doctypeAlias);

                    if (contentType != null)
                    {
                        var valueArray = jsonValue["value"];

                        if (valueArray != null)
                        {
                            foreach (JProperty item in valueArray)
                            {
                                var propAlias = item.Name;

                                var property = contentType.CompositionPropertyTypes.FirstOrDefault(x => x.Alias == propAlias);

                                if (property != null)
                                {
                                    IDataTypeDefinition dataType = null;

                                    var dataTypeDefinitionId = property.DataTypeDefinitionId;

                                    if (dataTypes.ContainsKey(dataTypeDefinitionId))
                                    {
                                        dataType = dataTypes[dataTypeDefinitionId];
                                    }
                                    else
                                    {
                                        dataType =
                                            this.dataTypeService.GetDataTypeDefinitionById(
                                                dataTypeDefinitionId);

                                        dataTypes.Add(dataTypeDefinitionId, dataType);
                                    }

                                    var parser =
                                        PropertyParserResolver.Current.Parsers.FirstOrDefault(
                                            x => x.IsParserFor(dataType));

                                    if (parser != null)
                                    {
                                        entities.AddRange(parser.GetLinkedEntities(item.Value));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<DocTypeGridEditorParser>("Error parsing doc type grid editor", exception);
            }

            return entities;
        }
    }
}
