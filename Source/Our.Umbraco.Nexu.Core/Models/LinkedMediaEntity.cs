namespace Our.Umbraco.Nexu.Core.Models
{
    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Core.Interfaces;
    
    /// <summary>
    /// The linked media entity.
    /// </summary>
    public class LinkedMediaEntity : ILinkedEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedMediaEntity"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public LinkedMediaEntity(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the linked entity type.
        /// </summary>
        public LinkedEntityType LinkedEntityType
        {
            get
            {
                return LinkedEntityType.Media;
            }
        }
    }
}
