namespace Our.Umbraco.Nexu.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using HtmlAgilityPack;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The parser helper.
    /// </summary>
    public static class ParserHelper
    {
        private const string DocumentUdiPrefix = "umb://document/";

        /// <summary>
        /// The get linked entities from csv string.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<T> GetLinkedEntitiesFromCsvString<T>(string input) where T : ILinkedEntity
        {
            var idlist = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var entities = new List<T>();

            idlist.ForEach(
                x =>
                {
                    var attemptId = x.TryConvertTo<int>();

                    if (attemptId.Success)
                    {
                        entities.Add((T)Activator.CreateInstance(typeof(T), attemptId.Result));
                    }
                });

            return entities;
        }

        /// <summary>
        /// Maps the path virtual path to a absolute path
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string MapPath(string virtualPath)
        {
            return HttpContext.Current == null
                        ? Path.Combine(Directory.GetCurrentDirectory(), virtualPath)
                        : HttpContext.Current.Server.MapPath(virtualPath);
        }

        public static IEnumerable<ILinkedEntity> ParseRichText(string value)
        {
            var linkedEntities = new List<ILinkedEntity>();

            var html = new HtmlDocument();
            html.LoadHtml(value);

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

            return linkedEntities;
        }

        /// <summary>
        /// Checks if a string is a new V7.6 Udi
        /// </summary>
        /// <param name="uid">
        /// The uid.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDocumentUdi(string uid)
        {
            return uid.StartsWith(DocumentUdiPrefix);
        }

        /// <summary>
        /// Maps a v7.6 udi to a interger id
        /// </summary>
        /// <param name="contentService">
        /// The content service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache provider.
        /// </param>
        /// <param name="uid">
        /// The uid.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int MapDocumentUdiToId(IContentService contentService, ICacheProvider cacheProvider, string uid)
        {
            var key = uid.TrimStart(DocumentUdiPrefix);

            // cache the key and id in static cache for faster future lookups                    
            return cacheProvider.GetCacheItem<int>(
                $"Nexu_Document_Udi_Cache_{key}",
                () =>
                {
                    var attemptGuid = key.TryConvertTo<Guid>();

                    if (attemptGuid.Success)
                    {
                        var content = contentService.GetById(attemptGuid.Result);

                        if (content != null)
                        {
                            return content.Id;
                        }
                    }

                    return -1;
                });
        }
    }
}
