namespace Our.Umbraco.Nexu.Common.Models
{
    using System;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Persistence.DatabaseAnnotations;

    using NPoco;

    using Our.Umbraco.Nexu.Common.Constants;

    /// <summary>
    /// Represents a nexu relation
    /// </summary>
    [TableName(DatabaseConstants.TableName)]
    [PrimaryKey(DatabaseConstants.IdColumn, AutoIncrement = false)]
    public class NexuRelation
    {
        private Udi udi;

        /// <summary>
        /// Initializes a new instance of the <see cref="NexuRelation"/> class.
        /// </summary>
        public NexuRelation()
        {
            this.Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Column(DatabaseConstants.IdColumn)]
        [PrimaryKeyColumn(AutoIncrement = false, Clustered = false, Name = DatabaseConstants.PrimaryKey)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the parent udi
        /// </summary>
        [Column(DatabaseConstants.ParentUdiColumn)]
        public string ParentUdi { get; set; }

        /// <summary>
        /// Gets or sets the child udi
        /// </summary>
        [Column(DatabaseConstants.ChildUdiColumn)]
        public string ChildUdi { get; set; }

        /// <summary>
        /// Gets or sets the relation type.
        /// </summary>
        [Column(DatabaseConstants.RelationTypeColumn)]
        public Guid RelationType { get; set; }

        /// <summary>
        /// Gets or sets the property id.
        /// </summary>
        [Column(DatabaseConstants.PropertyAlias)]
        public string PropertyAlias { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        [Column(DatabaseConstants.CultureColumn)]
        public string Culture { get; set; }

        /// <summary>
        /// Gets the udi.
        /// </summary>
        [Ignore]
        public Udi Udi
        {
            get
            {
                if (this.udi != null)
                {
                    return this.udi;
                }

                this.udi = new GuidUdi("nexurelation", this.Id);

                return this.udi;
            }
        }
    }
}
