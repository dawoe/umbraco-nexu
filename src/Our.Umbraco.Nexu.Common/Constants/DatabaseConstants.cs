namespace Our.Umbraco.Nexu.Common.Constants
{
    /// <summary>
    /// Represents the constants used in the database table
    /// </summary>
    public static class DatabaseConstants
    {
        /// <summary>
        /// The table name.
        /// </summary>
        public const string TableName = "Nexu_Relations";

        /// <summary>
        /// The id column.
        /// </summary>
        public const string IdColumn = "id";

        /// <summary>
        /// The primary key.
        /// </summary>
        public const string PrimaryKey = "PK_" + TableName;

        /// <summary>
        /// The parent udi column.
        /// </summary>
        public const string ParentUdiColumn = "parent_udi";

        /// <summary>
        /// The child udi column.
        /// </summary>
        public const string ChildUdiColumn = "child_udi";

        /// <summary>
        /// The relation type column.
        /// </summary>
        public const string RelationTypeColumn = "relation_type";

        /// <summary>
        /// The property id column.
        /// </summary>
        public const string PropertyAlias = "property_alias";

        /// <summary>
        /// The culture column.
        /// </summary>
        public const string CultureColumn = "culture_column";

        /// <summary>
        /// The parent udi index.
        /// </summary>
        public const string ParentUdiIndex = "IX_" + TableName + "_" + ParentUdiColumn;

        /// <summary>
        /// The child udi index.
        /// </summary>
        public const string ChildUdiIndex = "IX_" + TableName + "_" + ChildUdiColumn;
    }
}
