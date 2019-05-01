namespace Our.Umbraco.Nexu.Common.Interfaces.Services
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    /// <summary>
    /// Represents the entity parsing service
    /// </summary>
    internal interface IEntityParsingService
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
