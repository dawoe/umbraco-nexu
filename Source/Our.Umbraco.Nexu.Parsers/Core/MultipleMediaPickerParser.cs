namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The media picker parser.
    /// </summary>
    public class MultipleMediaPickerParser : IPropertyParser
    {
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return
                dataTypeDefinition.PropertyEditorAlias.Equals(
                    global::Umbraco.Core.Constants.PropertyEditors.MultipleMediaPickerAlias);
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            throw new System.NotImplementedException();
        }
    }
}
