namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The media picker parser for sites using V7.6 and up
    /// </summary>
    public class MediaPicker2Parser : IPropertyParser
    {
        /// <summary>
        /// The media service.
        /// </summary>
        private readonly IMediaService mediaService;

        /// <summary>
        /// The cache provider.
        /// </summary>
        private readonly ICacheProvider cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPicker2Parser"/> class.
        /// </summary>
        public MediaPicker2Parser()
        {
            this.mediaService = ApplicationContext.Current.Services.MediaService;
            this.cacheProvider = ApplicationContext.Current.ApplicationCache.StaticCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPicker2Parser"/> class.
        /// </summary>
        /// <param name="mediaService">
        /// The media service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache provider.
        /// </param>
        public MediaPicker2Parser(IMediaService mediaService, ICacheProvider cacheProvider)
        {
            this.mediaService = mediaService;
            this.cacheProvider = cacheProvider;
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
            return dataTypeDefinition.PropertyEditorAlias.Equals("Umbraco.MediaPicker2");
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
                    var id = ParserHelper.MapMediaUdiToId(this.mediaService, this.cacheProvider, udi);

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
