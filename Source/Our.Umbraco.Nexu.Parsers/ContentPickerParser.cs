namespace Our.Umbraco.Nexu.Parsers
{
    using System.Collections.Generic;

    using Our.Umbraco.Nexu.Core.Interfaces;
    public class ContentPickerParser : IPropertyParser
    {
        public string IsParserFor { get; }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
