namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The multi node tree picker 2 content parser.
    /// </summary>
    public class MultiNodeTreePicker2ContentParser : IPropertyParser
    {
        /// <summary>
        /// Check if it's a parser for a data type definition
        /// </summary>
        /// <param name="dataTypeDefinition">
        /// The data type definition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return false;
        }

        /// <summary>
        /// Gets the linked entites from the property value
        /// </summary>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            return Enumerable.Empty<ILinkedEntity>();
        }
    }
}
