namespace Our.Umbraco.Nexu.Core.Composing.Components
{
    using global::Umbraco.Core.Composing;
    using global::Umbraco.Core.Events;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Core.Services.Implement;

    /// <summary>
    /// Represents a component to hook in to the content service events
    /// </summary>
    internal class ContentServiceEventsComponent : IComponent
    {
        /// <inheritdoc />
        public void Initialize()
        {
            ContentService.Saved += this.ContentServiceOnSaved;
        }

        /// <inheritdoc />
        public void Terminate()
        {
            ContentService.Saved -= this.ContentServiceOnSaved;
        }

        /// <summary>
        /// Event handler for content service saved event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>       
        private void ContentServiceOnSaved(IContentService sender, ContentSavedEventArgs e)
        {
            foreach (var contentItem in e.SavedEntities)
            {
                if (!contentItem.Blueprint)
                {
                    
                }
            }
        }
    }
}
