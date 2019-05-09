namespace Our.Umbraco.Nexu.Common.Interfaces.Factories
{
    using System.Collections.Generic;

    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents the DisplayModelFactory interface.
    /// </summary>
    internal interface IDisplayModelFactory
    {
        /// <summary>
        /// Converts relations to display models.
        /// </summary>
        /// <param name="relations">
        /// The relations.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<NexuRelationDisplay> ConvertRelationsToDisplayModels(IEnumerable<NexuRelation> relations);
    }
}
