namespace Our.Umbraco.Nexu.Core.Repositories
{
    using System.Collections.Generic;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Interfaces.Repositories;
    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents nexu relation repository.
    /// </summary>
    internal class NexuRelationRepository : IRelationRepository
    {
        /// <inheritdoc />
        public void PersistRelationsForContentItem(Udi contentItemUdi, IEnumerable<NexuRelation> relations)
        {
        }
    }
}
