namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// Property parser interface
    /// </summary>
    public interface IPropertyParser
    {
        /// <summary>
        /// Check if it's a parser for this property
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsParserFor(PropertyType property);

        /// <summary>
        /// Check if it's a parser for a data type definition
        /// </summary>
        /// <param name="dataTypeDefinition">
        /// The data type definition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsParserFor(IDataTypeDefinition dataTypeDefinition);

        /// <summary>
        /// Get the linked entities from the property editor data
        /// </summary>
        /// <param name="property">The property to parse</param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<ILinkedEntity> GetLinkedEntities(Property property);
    }
}
