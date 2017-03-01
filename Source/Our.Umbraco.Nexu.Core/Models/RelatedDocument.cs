namespace Our.Umbraco.Nexu.Core.Models
{
    /// <summary>
    /// The related document.
    /// </summary>
    internal class RelatedDocument
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether published.
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether trashed.
        /// </summary>
        public bool Trashed { get; set; }
    }
}
