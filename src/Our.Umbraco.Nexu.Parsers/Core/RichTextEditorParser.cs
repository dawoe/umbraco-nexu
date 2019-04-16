namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    internal class RichTextEditorParser : IPropertyValueParser
    {
        public bool IsParserFor(Property property)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IRelatedEntity> GetRelatedEntities(Property property)
        {
            throw new System.NotImplementedException();
        }
    }
}
