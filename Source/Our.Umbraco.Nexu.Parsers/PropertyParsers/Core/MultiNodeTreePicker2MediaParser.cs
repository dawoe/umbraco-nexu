namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The multi node tree picker 2 media parser in Umbraco v7.6
    /// </summary>
    public class MultiNodeTreePicker2MediaParser : IPropertyParser
    {
        /// <summary>
        /// The data type service.
        /// </summary>
        private readonly IDataTypeService dataTypeService;

        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IMediaService mediaService;

        /// <summary>
        /// The cache.
        /// </summary>
        private readonly ICacheProvider cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePicker2MediaParser"/> class.
        /// </summary>
        public MultiNodeTreePicker2MediaParser()
        {
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            this.mediaService = ApplicationContext.Current.Services.MediaService;
            this.cache = ApplicationContext.Current.ApplicationCache.StaticCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePicker2MediaParser"/> class.
        /// </summary>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public MultiNodeTreePicker2MediaParser(IDataTypeService dataTypeService)
        {
            this.dataTypeService = dataTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePicker2MediaParser"/> class.
        /// </summary>
        /// <param name="mediaService">
        /// The content service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache provider.
        /// </param>
        public MultiNodeTreePicker2MediaParser(IMediaService mediaService, ICacheProvider cacheProvider)
        {
            this.mediaService = mediaService;
            this.cache = cacheProvider;
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
            if (
               !dataTypeDefinition.PropertyEditorAlias.Equals("Umbraco.MultiNodeTreePicker2"))
            {
                return false;
            }

            var prevalues =
                this.dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id).FormatAsDictionary();

            if (!prevalues.ContainsKey("startNode"))
            {
                return false;
            }

            var startNodeType = JsonConvert.DeserializeObject<JObject>(prevalues["startNode"].Value).Value<string>("type");

            if (startNodeType == null || startNodeType != "media")
            {
                return false;
            }

            return true;
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

            var udiArray = propertyValue.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var udi in udiArray)
            {
                if (ParserHelper.IsMediaUdi(udi))
                {
                    var id = ParserHelper.MapMediaUdiToId(this.mediaService, this.cache, udi);

                    if (id > -1)
                    {
                        entities.Add(new LinkedMediaEntity(id));
                    }
                }
            }

            return entities;
        }
    }
}
