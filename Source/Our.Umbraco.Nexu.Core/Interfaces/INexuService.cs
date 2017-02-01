namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// The NexuService interface.
    /// </summary>
    public interface INexuService
    {
        /// <summary>
        /// Sets up the needed the relation types
        /// </summary>
        void SetupRelationTypes();

        /// <summary>
        /// Gets all property parsrs
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<IPropertyParser> GetAllPropertyParsers();

        /// <summary>
        /// Get all parsers for content item.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<IPropertyParser> GetAllParsersForContentItem(IContent content);
    }
}