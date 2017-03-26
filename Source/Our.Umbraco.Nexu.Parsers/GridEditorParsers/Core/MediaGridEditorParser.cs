namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core
{
    using System.Collections.Generic;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The media grid editor parser.
    /// </summary>
    public class MediaGridEditorParser : IGridEditorParser
    {
        /// <summary>
        /// Check if it is a parser for a speficic editor
        /// </summary>
        /// <param name="editorview">
        /// The editor view alias as defined in grid config
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(string editorview)
        {
            return editorview.Equals("media");
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}
