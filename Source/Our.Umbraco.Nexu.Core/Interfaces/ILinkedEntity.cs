namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using Our.Umbraco.Nexu.Core.Enums;

    /// <summary>
    /// The LinkedEntity interface.
    /// </summary>
    public interface ILinkedEntity
    {
        /// <summary>
        /// Gets the id of the entity
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets  linked entity type.
        /// </summary>
        LinkedEntityType LinkedEntityType { get; }
    }
}
