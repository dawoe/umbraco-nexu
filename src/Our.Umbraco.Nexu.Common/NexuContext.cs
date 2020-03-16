namespace Our.Umbraco.Nexu.Common
{
    /// <summary>
    /// Represents the nexu context.
    /// </summary>
    public class NexuContext
    {
        private static NexuContext instance;

        private static readonly object padlock = new object();

        private bool isProcessing;

        private string itemInProgress;

        private int itemsProcessed;

        /// <summary>
        /// Prevents a default instance of the <see cref="NexuContext"/> class from being created.
        /// </summary>
        private NexuContext()
        {
            this.isProcessing = false;
            this.itemInProgress = string.Empty;
            this.itemsProcessed = 0;
            instance = this;
        }

        /// <summary>
        /// Gets the current context
        /// </summary>
        public static NexuContext Current
        {
            get
            {
                lock (padlock)
                {
                    return instance ?? (instance = new NexuContext());
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is processing.
        /// </summary>
        public bool IsProcessing
        {
            get => this.isProcessing;
            set
            {
                lock (padlock)
                {
                    this.isProcessing = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the item in progress.
        /// </summary>
        public string ItemInProgress
        {
            get => this.itemInProgress;
            set
            {
                lock (padlock)
                {
                    this.itemInProgress = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the items processed.
        /// </summary>
        public int ItemsProcessed
        {
            get => this.itemsProcessed;
            set
            {
                lock (padlock)
                {
                    this.itemsProcessed = value;
                }
            }
        }
    }
}
