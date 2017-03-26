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
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            throw new System.NotImplementedException();
        }
    }
}
