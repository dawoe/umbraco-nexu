namespace Our.Umbraco.Nexu.Core.Composing.Collections
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Composing;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    /// <summary>
    /// Represents property value parser collection.
    /// </summary>
    public class PropertyValueParserCollection : BuilderCollectionBase<IPropertyValueParser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueParserCollection"/> class.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public PropertyValueParserCollection(IEnumerable<IPropertyValueParser> items)
            : base(items)
        {
        }
    }
}
