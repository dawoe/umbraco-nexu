namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using Our.Umbraco.Nexu.Core.Enums;

    /// <summary>
    /// The LinkedEntity interface.
    /// </summary>
    public interface ILinkedEntity
    {
        /// <summary>
        /// Gets or sets the id of the entity
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the linked entity type.
        /// </summary>
        LinkedEntityType LinkedEntityType { get; set; }
    }
}
