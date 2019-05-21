namespace Our.Umbraco.Nexu.Common.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the nexu relation display model
    /// </summary>
    public class NexuRelationDisplay
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NexuRelationDisplay"/> class.
        /// </summary>
        public NexuRelationDisplay()
        {
            this.Properties = new List<NexuRelationPropertyDisplay>();
        }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the entity name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public IList<NexuRelationPropertyDisplay> Properties { get; set; }
    }
}
