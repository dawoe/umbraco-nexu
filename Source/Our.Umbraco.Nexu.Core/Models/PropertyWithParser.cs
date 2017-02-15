namespace Our.Umbraco.Nexu.Core.Models
{
    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The property with parser.
    /// </summary>
    internal class PropertyWithParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyWithParser"/> class.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="parser">
        /// The parser.
        /// </param>
        public PropertyWithParser(Property property, IPropertyParser parser)
        {
            this.Property = property;
            this.Parser = parser;
        }

        /// <summary>
        /// Gets  the property.
        /// </summary>
        public Property Property { get; }

        /// <summary>
        /// Gets  the parser.
        /// </summary>
        public IPropertyParser Parser { get;  }
    }
}
