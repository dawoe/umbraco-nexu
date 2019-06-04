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
        void ParseContent(IContent content);
    }
}
