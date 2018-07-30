namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Community
{
    using global::Umbraco.Core;
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
    }
}
