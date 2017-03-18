namespace Our.Umbraco.Nexu.Core.Models
{
    using System.Collections.Generic;

    using global::Umbraco.Web.Media.EmbedProviders.Settings;

    /// <summary>
    /// The related document.
    /// </summary>
    internal class RelatedDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedDocument"/> class.
        /// </summary>
        public RelatedDocument()
        {
            this.Properties = new Dictionary<string, List<string>>();
        }

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

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public Dictionary<string, List<string>> Properties { get; set; }
    }
}
