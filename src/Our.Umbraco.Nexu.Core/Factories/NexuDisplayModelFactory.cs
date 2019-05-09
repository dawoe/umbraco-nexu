namespace Our.Umbraco.Nexu.Core.Factories
{
    using System.Collections.Generic;

    using Our.Umbraco.Nexu.Common.Interfaces.Factories;
    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents the nexu display model factory
    /// </summary>
    internal class NexuDisplayModelFactory : IDisplayModelFactory
    {
        /// <inheritdoc />
        public IEnumerable<NexuRelationDisplay> ConvertRelationsToDisplayModels(IEnumerable<NexuRelation> relations)
        {
            return new List<NexuRelationDisplay>();
        }
    }
}
