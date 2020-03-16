namespace Our.Umbraco.Nexu.Core.Composing.Composers
{
    using global::Umbraco.Core;
    using global::Umbraco.Core.Composing;

    using Our.Umbraco.Nexu.Common.Interfaces.Repositories;
    using Our.Umbraco.Nexu.Common.Interfaces.Services;
    using Our.Umbraco.Nexu.Core.Composing.Components;
    using Our.Umbraco.Nexu.Core.Repositories;
    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    /// Represents the composer to handle all registrations for nexu
    /// </summary>
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class NexuComposer : IUserComposer
    {
        /// <inheritdoc />
        public void Compose(Composition composition)
        {                     
            composition.Register<IEntityParsingService, NexuEntityParsingService>();
            composition.Register<IRelationRepository, NexuRelationRepository>();
            composition.Register<IEntityRelationService, NexuEntityRelationService>();
            

            composition.Components().Append<MigrationComponent>();
            composition.Components().Append<ContentServiceEventsComponent>();
        }
    }
}
