namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Property parser interface
    /// </summary>
    public interface IPropertyParser
    {
        /// <summary>
        /// Gets the property editor alias for which this parser is for
        /// </summary>
        string IsParserFor { get; }

        /// <summary>
        /// Get the linked entities from the property editor data
        /// </summary>
        /// <param name="value">The property value to parse</param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<ILinkedEntity> GetLinkedEntities(object value);
    }
}
