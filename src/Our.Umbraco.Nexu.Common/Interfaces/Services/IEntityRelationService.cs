namespace Our.Umbraco.Nexu.Common.Interfaces.Services
{
    using System.Collections.Generic;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents the entity relation service
    /// </summary>
    public interface IEntityRelationService
    {
        /// <summary>
        /// Gets relations for a item.
        /// </summary>
        /// <param name="udi">
        /// The udi.
        /// </param>
        /// <returns>
        /// The <see cref="IList{T}"/>.
        /// </returns>
        IList<NexuRelationDisplayModel> GetRelationsForItem(Udi udi);


        IList<NexuRelationDisplayModel> GetUsedItemsFromList(IList<Udi> udis);

        bool CheckLinksInDescendants(GuidUdi rootId);
    }
}
