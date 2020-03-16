namespace Our.Umbraco.Nexu.Common.Interfaces.Repositories
{
    using System.Collections.Generic;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents the RelationRepository interface.
    /// </summary>
    public interface IRelationRepository
    {
        /// <summary>
        /// Persists the relations for a content item
        /// </summary>
        /// <param name="contentItemUdi">
        /// The content item udi.
        /// </param>
        /// <param name="relations">
        /// The relations.
        /// </param>
        void PersistRelationsForContentItem(Udi contentItemUdi, IEnumerable<NexuRelation> relations);

        /// <summary>
        /// Gets the incoming relations for a item
        /// </summary>
        /// <param name="udi">
        /// The udi.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<NexuRelation> GetIncomingRelationsForItem(Udi udi);

        /// <summary>
        /// Gets the items that are in use from a list of unique doc identifier
        /// </summary>
        /// <param name="udis">
        /// The  list of unique doc identifier.
        /// </param>
        /// <returns>
        /// The <see cref="IList{T}"/>.
        /// </returns>
        IList<KeyValuePair<string, string>> GetUsedItemsFromList(IList<Udi> udis);
    }
}
