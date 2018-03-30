namespace Our.Umbraco.Nexu.Core
{
    using System.Configuration;

    using global::Umbraco.Core;

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
            this.ItemInProgress = string.Empty;
            this.ItemsProcessed = 0;
            this.DocumentToDocumentRelationTypeExists = false;
            this.DocumentToMediaRelationTypeExists = false;
            this.PreventDelete = this.GetAppSetting<bool>(Constants.AppSettings.AllowDelete);
            this.PreventUnPublish = this.GetAppSetting<bool>(Constants.AppSettings.AllowUnPublish);
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

        /// <summary>
        /// Gets or sets the item in progress.
        /// </summary>
        public string ItemInProgress { get; set; }

        /// <summary>
        /// Gets or sets the items processed.
        /// </summary>
        public int ItemsProcessed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether document to document relation type exists.
        /// </summary>
        public bool DocumentToDocumentRelationTypeExists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether document to media relation type exists.
        /// </summary>
        public bool DocumentToMediaRelationTypeExists { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether prevent delete.
        /// </summary>
        public bool PreventDelete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether prevent un publish.
        /// </summary>
        public bool PreventUnPublish { get; set; }

        /// <summary>
        /// Gets the value of app setting 
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="T">
        /// The return type
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private T GetAppSetting<T>(string key)
        {
            var value = default(T);

            var setting = ConfigurationManager.AppSettings[key];

            if (setting != null)
            {
                var attempConvert = setting.TryConvertTo<T>();

                if (attempConvert.Success)
                {
                    value = attempConvert.Result;
                }
            }


            return value;
        }
    }
}
