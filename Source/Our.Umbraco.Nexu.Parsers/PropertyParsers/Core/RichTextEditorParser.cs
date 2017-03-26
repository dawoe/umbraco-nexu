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
                var html = new HtmlDocument();
                html.LoadHtml(propertyValue.ToString());

                // get all image tags
                var images = html.DocumentNode.SelectNodes("//img");

                if (images != null)
                {
                    foreach (var img in images)
                    {
                        // umbraco sets the id of the image in the rel tag, so we check if that is present
                        if (!img.Attributes.Contains("rel"))
                        {
                            continue;
                        }

                        var relId = img.Attributes["rel"].Value;

                        // if no id go to next image
                        if (string.IsNullOrEmpty(relId))
                        {
                            continue;
                        }

                        var attemptId = relId.TryConvertTo<int>();

                        if (attemptId.Success)
                        {
                            linkedEntities.Add(new LinkedMediaEntity(attemptId.Result));
                        }
                    }
                }

                // get all links
                var anchors = html.DocumentNode.SelectNodes("//a");

                var localLinkRegex = new Regex("/{localLink:(?<id>\\d*)}", RegexOptions.CultureInvariant | RegexOptions.Compiled);

                if (anchors != null)
                {
                    foreach (var anchor in anchors)
                    {
                        // check if we have source attribute
                        if (!anchor.Attributes.Contains("href"))
                        {
                            continue;
                        }

                        var href = anchor.Attributes["href"].Value;

                        if (string.IsNullOrEmpty(href))
                        {
                            continue;
                        }

                        // test if the source is a local link
                        if (localLinkRegex.IsMatch(href))
                        {
                            var match = localLinkRegex.Match(href);

                            var attemptId = match.Groups["id"].Value.TryConvertTo<int>();

                            if (attemptId.Success)
                            {
                                linkedEntities.Add(new LinkedDocumentEntity(attemptId.Result));
                            }
                        }
                        else if (href.StartsWith("/media"))
                        {
                            // possible link to media item, umbraco sets data-id
                            if (!anchor.Attributes.Contains("data-id"))
                            {
                                continue;
                            }

                            var dataId = anchor.Attributes["data-id"].Value;

                            var attemptId = dataId.TryConvertTo<int>();

                            if (attemptId.Success)
                            {
                                linkedEntities.Add(new LinkedMediaEntity(attemptId.Result));
                            }
                        }
                    }
                }
            }
            catch
            {
                // TODO implement logging
            }

            return linkedEntities;
        }
    }
}
