namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Community
{
    using System.Web.Http.Routing.Constraints;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    /// <summary>
    /// The content list parser.
    /// </summary>
    public class ContentListParser : StackedContentParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentListParser"/> class.
        /// </summary>
        public ContentListParser()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentListParser"/> class.
        /// </summary>
        /// <param name="contentTypeService">
        /// The content type service.
        /// </param>
        /// <param name="dataTypeService">
        /// The data Type Service.
        /// </param>
        public ContentListParser(IContentTypeService contentTypeService, IDataTypeService dataTypeService)
            : base(contentTypeService, dataTypeService)
        {
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
        public override bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return dataTypeDefinition.PropertyEditorAlias.Equals("Our.Umbraco.ContentList");
        }
    }
}
