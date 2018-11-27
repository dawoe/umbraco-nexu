namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The NexuService interface.
    /// </summary>
    public interface INexuService
    {
        /// <summary>
        /// Gets all property parsrs
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<IPropertyParser> GetAllPropertyParsers();

        /// <summary>
        /// Get the linked entities for a content item
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary{S,T}"/>.
        /// </returns>
        Dictionary<string, IEnumerable<ILinkedEntity>> GetLinkedEntitesForContent(IContent content);

        /// <summary>
        /// Get all properties of content item we have a parser for
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<PropertyWithParser> GetParsablePropertiesForContent(IContent content);

        /// <summary>
        /// Delete all relations for content.
        /// </summary>
        /// <param name="contentid">
        /// The contentid.
        /// </param>
        void DeleteRelationsForContent(int contentid);

        /// <summary>
        /// Gets all nexu relations for content.
        /// </summary>
        /// <param name="contentId">
        /// The content id.
        /// </param>
        /// <param name="isParent">
        /// The is Parent.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<IRelation> GetNexuRelationsForContent(int contentId, bool isParent = true);

        /// <summary>
        /// Saves linked entities as relations
        /// </summary>
        /// <param name="contentid">
        /// The contentid.
        /// </param>
        /// <param name="linkedEntities">
        /// The linked entities.
        /// </param>
        void SaveLinkedEntitiesAsRelations(int contentid, Dictionary<string, IEnumerable<ILinkedEntity>> linkedEntities);

        /// <summary>
        /// Parses content and saves linked entitites
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        void ParseContent(IContent content);

        /// <summary>
        /// Sets up the needed the relation types
        /// </summary>
        void SetupRelationTypes();
    }
}