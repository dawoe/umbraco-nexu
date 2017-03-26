namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// The GridEditorParser interface.
    /// </summary>
    public interface IGridEditorParser
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
        bool IsParserFor(string editorview);

        /// <summary>
        /// Gets the linked entities for this editor
        /// </summary>
        /// <param name="value">
        /// The grid editor value
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<ILinkedEntity> GetLinkedEntities(string value);
    }
}
