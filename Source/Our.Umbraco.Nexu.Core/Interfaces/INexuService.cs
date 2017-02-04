namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// The NexuService interface.
    /// </summary>
    internal interface INexuService
    {
        /// <summary>
        /// Gets all property parsrs
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<IPropertyParser> GetAllPropertyParsers();

        /// <summary>
        /// Get all properties of content item we have a parser for
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<PropertyType> GetParsablePropertiesForContent(IContent content);

        /// <summary>
        /// Sets up the needed the relation types
        /// </summary>
        void SetupRelationTypes();
    }
}