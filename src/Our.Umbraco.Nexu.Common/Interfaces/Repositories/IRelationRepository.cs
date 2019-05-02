namespace Our.Umbraco.Nexu.Common.Interfaces.Repositories
{
    using System.Collections.Generic;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents the RelationRepository interface.
    /// </summary>
    internal interface IRelationRepository
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
    }
}
