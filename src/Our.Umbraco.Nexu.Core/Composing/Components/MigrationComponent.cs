namespace Our.Umbraco.Nexu.Core.Composing.Components
{
    using global::Umbraco.Core.Composing;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Migrations;
    using global::Umbraco.Core.Migrations.Upgrade;
    using global::Umbraco.Core.Scoping;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Migrations;

    /// <summary>
    /// Represents the migration component.
    /// </summary>
    internal class MigrationComponent : IComponent
    {
        /// <summary>
        /// The scope provider.
        /// </summary>
        private readonly IScopeProvider scopeProvider;

        /// <summary>
        /// The migration builder.
        /// </summary>
        private readonly IMigrationBuilder migrationBuilder;

        /// <summary>
        /// The key value service.
        /// </summary>
        private readonly IKeyValueService keyValueService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationComponent"/> class.
        /// </summary>
        /// <param name="scopeProvider">
        /// The scope provider.
        /// </param>
        /// <param name="migrationBuilder">
        /// The migration builder.
        /// </param>
        /// <param name="keyValueService">
        /// The key value service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MigrationComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            this.scopeProvider = scopeProvider;
            this.migrationBuilder = migrationBuilder;
            this.keyValueService = keyValueService;
            this.logger = logger;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            var upgrader = new Upgrader(new NexuMigrationPlan());

            upgrader.Execute(this.scopeProvider, this.migrationBuilder, this.keyValueService, this.logger);
        }

        /// <inheritdoc />
        public void Terminate()
        {            
        }
    }
}
