namespace Our.Umbraco.Nexu.Common.Constants
{
    using System;

    /// <summary>
    /// Represents the relation type guids
    /// </summary>
    public static class RelationTypes
    {
        /// <summary>
        /// Gets the document to document relation type id
        /// </summary>
        public static Guid DocumentToDocument => new Guid("F1E181D1-BAE7-4B33-9504-2C111E2A2245");

        /// <summary>
        /// Gets the document to document relation type id
        /// </summary>
        public static Guid DocumentToMedia => new Guid("9EE85524-DE88-42C4-901A-B24C1C777723");
    }
}
