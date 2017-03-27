namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core
{
    using System.Collections.Generic;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The rte grid editor parser.
    /// </summary>
    public class RteGridEditorParser : IGridEditorParser
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
            return editorview.Equals("rte");
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
            throw new System.NotImplementedException();
        }
    }
}
