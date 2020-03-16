namespace Our.Umbraco.Nexu.Parsers.Core
{
    using global::Umbraco.Core;

    /// <summary>
    /// Represents the rich text editor parser
    /// </summary>
    public class RichTextEditorParser : BaseTextParser
    {
        /// <inheritdoc />
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.TinyMce);
        }        
    }
}
