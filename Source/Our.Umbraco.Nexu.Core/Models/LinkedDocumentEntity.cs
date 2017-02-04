namespace Our.Umbraco.Nexu.Core.Models
{
    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The linked document entity.
    /// </summary>
    public class LinkedDocumentEntity : ILinkedEntity
    {
        /// <summary>
        /// Gets or sets the id of the entity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets  linked entity type.
        /// </summary>
        public LinkedEntityType LinkedEntityType
        {
            get
            {
                return LinkedEntityType.Document;
            }
        }
    }
}
