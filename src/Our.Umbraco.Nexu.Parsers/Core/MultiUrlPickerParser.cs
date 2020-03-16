namespace Our.Umbraco.Nexu.Parsers.Core
{
    using global::Umbraco.Core;

    /// <summary>
    /// Represents the multi url picker
    /// </summary>
    public class MultiUrlPickerParser : BaseTextParser
    {
        /// <inheritdoc />
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.MultiUrlPicker);
        }
    }
}
