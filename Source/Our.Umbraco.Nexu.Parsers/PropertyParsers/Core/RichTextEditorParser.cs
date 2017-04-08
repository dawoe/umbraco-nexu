namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The rich text editor parser.
    /// </summary>
    public class RichTextEditorParser : IPropertyParser
    {
        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The media service.
        /// </summary>
        private readonly IMediaService mediaService;

        /// <summary>
        /// The cache provider.
        /// </summary>
        private readonly ICacheProvider cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextEditorParser"/> class.
        /// </summary>
        public RichTextEditorParser()
        {
            this.contentService = ApplicationContext.Current.Services.ContentService;
            this.mediaService = ApplicationContext.Current.Services.MediaService;
            this.cacheProvider = ApplicationContext.Current.ApplicationCache.StaticCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextEditorParser"/> class.
        /// </summary>
        /// <param name="contentService">
        /// The content service.
        /// </param>
        /// <param name="mediaService">
        /// The media service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache provider.
        /// </param>
        public RichTextEditorParser(IContentService contentService, IMediaService mediaService, ICacheProvider cacheProvider)
        {
            this.contentService = contentService;
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
            return
                dataTypeDefinition.PropertyEditorAlias.Equals(
                    global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias);
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
            var linkedEntities = new List<ILinkedEntity>();

            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return linkedEntities;
            }

            try
            {
                var html = propertyValue.ToString();
                linkedEntities.AddRange(ParserHelper.ParseRichText(html));

                if (html.Contains("umb://document")
                    || html.Contains("umb://media"))
                {
                    // it's a v7.6 rte field
                    linkedEntities.AddRange(ParserHelper.ParseRichTextForV76(html, this.contentService, this.mediaService, this.cacheProvider));
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<RichTextEditorParser>("Error parsing rich text editor", exception);
            }

            return linkedEntities;
        }
    }
}
