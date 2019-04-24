namespace Our.Umbraco.Nexu.Core.Composing.Collections
{
    using global::Umbraco.Core.Composing;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    /// <summary>
    /// Represents the property value parser collection builder
    /// </summary>
    public class PropertyValueParserCollectionBuilder 
        : OrderedCollectionBuilderBase<PropertyValueParserCollectionBuilder, PropertyValueParserCollection, IPropertyValueParser>
    {
        /// <inheritdoc />
        protected override PropertyValueParserCollectionBuilder This => this;
    }
}
