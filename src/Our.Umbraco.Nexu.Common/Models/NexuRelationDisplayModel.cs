namespace Our.Umbraco.Nexu.Common.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the nexu relation display model.
    /// </summary>
    public class NexuRelationDisplayModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NexuRelationDisplayModel"/> class.
        /// </summary>
        public NexuRelationDisplayModel()
        {
            this.Properties = new List<NexuRelationPropertyDisplay>();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is published.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is trashed.
        /// </summary>
        public bool IsTrashed { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public IList<NexuRelationPropertyDisplay> Properties { get; set; }
    }
}
