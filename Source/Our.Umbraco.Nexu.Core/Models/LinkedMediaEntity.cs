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
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

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
