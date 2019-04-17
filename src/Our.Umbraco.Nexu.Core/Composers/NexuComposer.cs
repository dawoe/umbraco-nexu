namespace Our.Umbraco.Nexu.Core.Composers
{
    using global::Umbraco.Core;
    using global::Umbraco.Core.Composing;

    using Our.Umbraco.Nexu.Core.Components;

    /// <summary>
    /// Represents the composer to handle all registrations for nexu
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
