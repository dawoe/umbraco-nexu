namespace Our.Umbraco.Nexu.Core.Composers
{
    using global::Umbraco.Core;
    using global::Umbraco.Core.Composing;

    using Our.Umbraco.Nexu.Core.Components;

    /// <summary>
    /// The nexu composer.
    /// </summary>
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    internal class NexuComposer : IUserComposer
    {
        /// <inheritdoc />
        public void Compose(Composition composition)
        {
            composition.Components().Append<ContentServiceEventsComponent>();
        }
    }
}
