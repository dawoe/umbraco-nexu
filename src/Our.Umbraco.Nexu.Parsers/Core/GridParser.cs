namespace Our.Umbraco.Nexu.Parsers.Core
{
    using global::Umbraco.Core;

    /// <summary>
    /// Represents the parser for the grid editor
    /// </summary>
    public class GridParser : BaseTextParser
    {
        /// <inheritdoc />
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals(Constants.PropertyEditors.Aliases.Grid);
        }        
    }
}
