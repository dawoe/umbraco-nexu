namespace Our.Umbraco.Nexu.Parsers
{
    using System.Collections.Generic;

    using AutoMapper;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;
    public class ContentPickerParser : IPropertyParser
    {
        public bool IsParserFor(PropertyType property)
        {
            return property.PropertyEditorAlias.Equals(global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias);
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
