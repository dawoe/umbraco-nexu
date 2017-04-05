namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;

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
            if (string.IsNullOrEmpty(value))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            var entities = new List<ILinkedEntity>();

            try
            {
                entities.AddRange(ParserHelper.ParseRichText(value));
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<RteGridEditorParser>("Error parsing rich text grid editor", exception);
            }

            return entities;
        }
    }
}
