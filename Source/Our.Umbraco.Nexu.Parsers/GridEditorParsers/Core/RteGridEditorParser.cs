namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The rte grid editor parser.
    /// </summary>
    public class RteGridEditorParser : IGridEditorParser
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
        /// Initializes a new instance of the <see cref="RteGridEditorParser"/> class.
        /// </summary>
        public RteGridEditorParser()
        {
            this.contentService = ApplicationContext.Current.Services.ContentService;
            this.mediaService = ApplicationContext.Current.Services.MediaService;
            this.cacheProvider = ApplicationContext.Current.ApplicationCache.StaticCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RteGridEditorParser"/> class.
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
        public RteGridEditorParser(IContentService contentService, IMediaService mediaService, ICacheProvider cacheProvider)
        {
            this.contentService = contentService;
            this.mediaService = mediaService;
            this.cacheProvider = cacheProvider;
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
            return editorview.Equals("rte");
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
                entities.AddRange(ParserHelper.ParseRichText(value));

                if (value.Contains("umb://document")
                    || value.Contains("umb://media"))
                {
                    // it's a v7.6 rte field
                    entities.AddRange(ParserHelper.ParseRichTextForV76(value, this.contentService, this.mediaService, this.cacheProvider));
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<RteGridEditorParser>("Error parsing rich text grid editor", exception);
            }

            return entities;
        }
    }
}
