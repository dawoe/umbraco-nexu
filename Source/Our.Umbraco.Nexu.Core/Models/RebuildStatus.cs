namespace Our.Umbraco.Nexu.Core.Models
{
    /// <summary>
    /// The rebuild status.
    /// </summary>
    internal class RebuildStatus
    {
        /// <summary>
        /// Gets or sets the is processing.
        /// </summary>
        public string IsProcessing { get; set; }

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the items processed.
        /// </summary>
        public int ItemsProcessed { get; set; }
    }
}
