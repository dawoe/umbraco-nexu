namespace Our.Umbraco.Nexu.Parsers.Core
{
    using global::Umbraco.Core;

    /// <summary>
    /// Represents the nested content parser.
    /// </summary>
    public class NestedContentParser : BaseTextParser
    {
        /// <inheritdoc />
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.NestedContent);
        }      
    }
}
