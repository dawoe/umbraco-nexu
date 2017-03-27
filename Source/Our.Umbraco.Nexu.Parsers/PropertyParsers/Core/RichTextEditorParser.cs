namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using HtmlAgilityPack;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The rich text editor parser.
    /// </summary>
    public class RichTextEditorParser : IPropertyParser
    {
        /// <summary>
        /// Check if it's a parser for a data type definition
        /// </summary>
        /// <param name="dataTypeDefinition">
        /// The data type definition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return
                dataTypeDefinition.PropertyEditorAlias.Equals(
                    global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias);
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            var linkedEntities = new List<ILinkedEntity>();

            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return linkedEntities;
            }

            try
            {
                linkedEntities.AddRange(ParserHelper.ParseRichText(propertyValue.ToString()));
            }
            catch
            {
                // TODO implement logging
            }

            return linkedEntities;
        }
    }
}
