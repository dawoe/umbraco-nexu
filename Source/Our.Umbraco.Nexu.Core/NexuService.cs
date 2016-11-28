namespace Our.Umbraco.Nexu.Core
{
    /// <summary>
    /// Nexu service
    /// </summary>
    public class NexuService
    {
        /// <summary>
        /// Internal service instance
        /// </summary>
        private static NexuService service;

        /// <summary>
        /// Prevents a default instance of the <see cref="NexuService"/> class from being created.
        /// </summary>
        private NexuService()
        {
            service = this;
        }

        /// <summary>
        /// The current nexu service instance
        /// </summary>
        public static NexuService Current => service ?? new NexuService();
    }
}
