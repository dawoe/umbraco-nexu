namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The legacy media picker parser.
    /// </summary>
    public class LegacyMediaPickerParser : IPropertyParser
    {
        public bool IsParserFor(PropertyType property)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(Property property)
        {
            throw new System.NotImplementedException();
        }
    }
}
