namespace Our.Umbraco.Nexu.Core.Composing
{
    using global::Umbraco.Core.Composing;

    using Our.Umbraco.Nexu.Core.Composing.Collections;

    /// <summary>
    /// Represents extension methods used in composing
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the property value parsers collection builder
        /// </summary>
        /// <param name="composition">
        /// The composition.
        /// </param>
        /// <returns>
        /// The <see cref="PropertyValueParserCollectionBuilder"/>.
        /// </returns>
        public static PropertyValueParserCollectionBuilder PropertyValueParsers(this Composition composition)
            => composition.WithCollectionBuilder<PropertyValueParserCollectionBuilder>();
    }
}
