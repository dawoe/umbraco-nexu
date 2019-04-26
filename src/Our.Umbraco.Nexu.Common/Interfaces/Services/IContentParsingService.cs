namespace Our.Umbraco.Nexu.Common.Interfaces.Services
{
    using global::Umbraco.Core.Models;

    /// <summary>
    /// Represents the entity parsing service
    /// </summary>
    public interface IEntityParsingService
    {
        /// <summary>
        /// Parses a content item for related entities
        /// </summary>
        /// <param name="content">
        /// The content item that needs to be parsed
        /// </param>
        /// <param name="parseEditedContentOnly">
        /// Indicate if only edited content needs to be parsed
        /// </param>
        /// <param name="parseAllCultures">
        /// Indicates if all cultures need to be parsed
        /// </param>
        void ParseContent(IContent content, bool parseEditedContentOnly = true, bool parseAllCultures = false);
    }
}
