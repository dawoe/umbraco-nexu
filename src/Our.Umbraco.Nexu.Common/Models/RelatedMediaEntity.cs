namespace Our.Umbraco.Nexu.Common.Models
{
    using System;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Common.Constants;
    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    /// <summary>
    /// Represents a related media entity
    /// </summary>
    public class RelatedMediaEntity : IRelatedEntity
    {
        /// <inheritdoc />
        public Udi RelatedEntityUdi { get; set; }

        /// <inheritdoc />
        public Guid RelationType => RelationTypes.DocumentToMedia;       
    }
}
