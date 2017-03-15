namespace Our.Umbraco.Nexu.Core
{
    /// <summary>
    /// The nexu context.
    /// </summary>
    public class NexuContext
    {
        /// <summary>
        /// The current instance.
        /// </summary>
        private static NexuContext instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="NexuContext"/> class from being created.
        /// </summary>
        private NexuContext()
        {
            this.IsProcessing = false;
            instance = this;
        }

        /// <summary>
        /// Gets the current context
        /// </summary>
        public static NexuContext Current => instance ?? new NexuContext();

        /// <summary>
        /// Gets or sets a value indicating whether is processing.
        /// </summary>
        public bool IsProcessing { get; set; }
    }
}
