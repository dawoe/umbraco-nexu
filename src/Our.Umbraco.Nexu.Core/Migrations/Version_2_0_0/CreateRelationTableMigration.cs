namespace Our.Umbraco.Nexu.Core.Migrations.Version_2_0_0
{
    using global::Umbraco.Core.Migrations;

    using Our.Umbraco.Nexu.Common.Constants;

    /// <summary>
    /// Represents the create relation table migration.
    /// </summary>
    internal class CreateRelationTableMigration : MigrationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRelationTableMigration"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public CreateRelationTableMigration(IMigrationContext context)
            : base(context)
        {
        }

        /// <inheritdoc />
        public override void Migrate()
        {
            this.Create.Table(DatabaseConstants.TableName)
                .WithColumn(DatabaseConstants.IdColumn).AsGuid().PrimaryKey(DatabaseConstants.PrimaryKey).NotNullable()
                .WithColumn(DatabaseConstants.ParentUdiColumn).AsString(100).NotNullable()
                .WithColumn(DatabaseConstants.ChildUdiColumn).AsString(100).NotNullable()
                .WithColumn(DatabaseConstants.RelationTypeColumn).AsGuid().NotNullable()
                .WithColumn(DatabaseConstants.PropertyAlias).AsString(100).NotNullable()
                .WithColumn(DatabaseConstants.CultureColumn).AsString(32).Nullable()
                .Do();

            this.Create.Index(DatabaseConstants.ParentUdiIndex).OnTable(DatabaseConstants.TableName).WithOptions().NonClustered()
                .OnColumn(DatabaseConstants.ParentUdiColumn).Ascending().Do();

            this.Create.Index(DatabaseConstants.ChildUdiIndex).OnTable(DatabaseConstants.TableName).WithOptions().NonClustered()
                .OnColumn(DatabaseConstants.ChildUdiColumn).Ascending().Do();
        }
    }
}
