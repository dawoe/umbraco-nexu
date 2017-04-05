namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

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

            var linkedEntities = new List<ILinkedEntity>();

            var jsonValue = JsonConvert.DeserializeObject<JObject>(value);

            var attemptId = jsonValue["id"].ToString().TryConvertTo<int>();

            if (attemptId.Success)
            {
                linkedEntities.Add(new LinkedMediaEntity(attemptId.Result));
            }

            return linkedEntities;
        }
    }
}
