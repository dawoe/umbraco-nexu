namespace Our.Umbraco.Nexu.Parsers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using global::Umbraco.Core;

    /// <summary>
    /// Represents the parser utilities. This objects has useful methods for helping to parse content
    /// </summary>
    internal static class ParserUtilities
    {
        /// <summary>
        /// Gets all the document udi from a text
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Udi}"/>.
        /// </returns>
        public static IEnumerable<Udi> GetDocumentUdiFromText(string text)
        {
            return GetUdiFromText(text, "umb://document/(.{32})");
        }

        /// <summary>
        /// Gets all the media udi from a text
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Udi}"/>.
        /// </returns>
        public static IEnumerable<Udi> GetMediaUdiFromText(string text)
        {
            return GetUdiFromText(text, "umb://media/(.{32})");
        }

        /// <summary>
        /// Get all udi from a text that match the regex
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="regex">
        /// The regex.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Udi}"/>.
        /// </returns>
        private static IEnumerable<Udi> GetUdiFromText(string text, string regex)
        {
            var udiList = new List<Udi>();

            var udiMatches = Regex.Matches(text, regex);

            foreach (Match match in udiMatches)
            {
                udiList.Add(new StringUdi(new Uri(match.Value)));
            }

            return udiList.DistinctBy(x => x.ToString());
        }
    }
}
