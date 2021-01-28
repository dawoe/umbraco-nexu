using System.Configuration;
using Umbraco.Core;

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
            this.PreventDelete = this.GetAppSetting<bool>(Constants.AppSettings.PreventDelete, false);
            this.PreventUnPublish = this.GetAppSetting<bool>(Constants.AppSettings.PreventUnpublish, false);
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

        /// <summary>
        /// Gets a value indicating whether prevent delete.
        /// </summary>
        public bool PreventDelete { get; }

        /// <summary>
        /// Gets a value indicating whether prevent un publish.
        /// </summary>
        public bool PreventUnPublish { get; set; }


        /// <summary>
        /// Gets the value of app setting 
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="defaultValue">The default value when the app setting is empty or not found.</param>
        /// <typeparam name="T">
        /// The return type
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private T GetAppSetting<T>(string key, T defaultValue)
        {
            var value = defaultValue;

            var setting = ConfigurationManager.AppSettings[key];

            if (setting == null)
            {
                return value;
            }

            var attempConvert = setting.TryConvertTo<T>();

            if (attempConvert.Success)
            {
                value = attempConvert.Result;
            }

            return value;
        }
    }
}
