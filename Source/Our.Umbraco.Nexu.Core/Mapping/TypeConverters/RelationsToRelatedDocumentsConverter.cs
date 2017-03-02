namespace Our.Umbraco.Nexu.Core.Mapping.TypeConverters
{
    using System.Collections.Generic;

    using AutoMapper;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The relation to related document type converter
    /// </summary>
    internal class RelationsToRelatedDocumentsConverter : ITypeConverter<IEnumerable<IRelation>, IEnumerable<RelatedDocument>>
    {
        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationsToRelatedDocumentsConverter"/> class.
        /// </summary>
        public RelationsToRelatedDocumentsConverter()
        {
            this.contentService = ApplicationContext.Current.Services.ContentService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationsToRelatedDocumentsConverter"/> class.
        /// </summary>
        /// <param name="contentservice">
        /// The contentservice.
        /// </param>
        public RelationsToRelatedDocumentsConverter(IContentService contentservice)
        {
            this.contentService = contentservice;
        }

        /// <summary>
        /// Converts the source to destination type
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<RelatedDocument> Convert(ResolutionContext context)
        {
            var destination = new List<RelatedDocument>();

            return destination;
        }
    }
}
