namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core
{
    using System.Collections.Generic;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The media grid editor parser.
    /// </summary>
    public class MediaGridEditorParser : IGridEditorParser
    {
        public bool IsParserFor(string editorview)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}
