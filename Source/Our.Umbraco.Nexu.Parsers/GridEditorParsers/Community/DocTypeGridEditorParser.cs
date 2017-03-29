namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Community
{
    using System.Collections.Generic;
    using System.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;
    /// <summary>
    /// The doc type grid editor parser.
    /// </summary>
    public class DocTypeGridEditorParser : IGridEditorParser
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
            return false;
        }

        /// <summary>
        /// Gets the linked entities for this editor
        /// </summary>
        /// <param name="value">
        /// The grid editor value
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(string value)
        {
            return Enumerable.Empty<ILinkedEntity>();
        }
    }
}
