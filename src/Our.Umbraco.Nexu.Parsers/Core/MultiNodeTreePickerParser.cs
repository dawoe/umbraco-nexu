namespace Our.Umbraco.Nexu.Parsers.Core
{
    using global::Umbraco.Core;

    /// <summary>
    /// Represents the multi node tree picker parser
    /// </summary>
    public class MultiNodeTreePickerParser : BaseTextParser
    {
        /// <inheritdoc />
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.MultiNodeTreePicker);
        }       
    }
}
