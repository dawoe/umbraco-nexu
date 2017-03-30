namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Community
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Interfaces;

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
            return Enumerable.Empty<ILinkedEntity>();
        }
    }
}
