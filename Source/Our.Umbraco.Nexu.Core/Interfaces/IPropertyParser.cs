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
        /// Gets the linked entites from the property value
        /// </summary>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue);
    }
}
