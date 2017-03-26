namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The grid parser.
    /// </summary>
    public class GridParser : IPropertyParser
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
            return
                dataTypeDefinition.PropertyEditorAlias.Equals(global::Umbraco.Core.Constants.PropertyEditors.GridAlias);
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            throw new System.NotImplementedException();
        }
    }
}
