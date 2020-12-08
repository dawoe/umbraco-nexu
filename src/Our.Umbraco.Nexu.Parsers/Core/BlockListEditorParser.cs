namespace Our.Umbraco.Nexu.Parsers.Core
{
    public class BlockListEditorParser : BaseTextParser
    {
        public override bool IsParserFor(string propertyEditorAlias)
        {
            return propertyEditorAlias.Equals("Umbraco.BlockList");
        }
    }
}
