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
        /// Initializes a new instance of the <see cref="LinkedDocumentEntity"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public LinkedDocumentEntity(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the id of the entity
        /// </summary>
        public int Id { get; private set; }

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
