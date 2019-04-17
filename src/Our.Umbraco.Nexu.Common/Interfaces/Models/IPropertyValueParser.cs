namespace Our.Umbraco.Nexu.Common.Interfaces.Models
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// Represents the PropertyValueParser interface.
    /// </summary>
    public interface IPropertyValueParser
    {
        /// <summary>
        /// Checks if this is the parser for the current property
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsParserFor(Property property);

        /// <summary>
        /// Gets the related entities from the property value
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IRelatedEntity}"/>.
        /// </returns>
        IEnumerable<IRelatedEntity> GetRelatedEntities(Property property);
    }
}
