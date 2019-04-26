namespace Our.Umbraco.Nexu.Common.Interfaces.Models
{
    using System;

    using global::Umbraco.Core;

    /// <summary>
    /// Represents the RelatedEntity interface.
    /// </summary>
    public interface IRelatedEntity
    {
        /// <summary>
        /// Gets or sets the related entity udi.
        /// </summary>
        Udi RelatedEntityUdi { get; set; }

        /// <summary>
        /// Gets the relation type.
        /// </summary>
        Guid RelationType { get; }       
    }
}
