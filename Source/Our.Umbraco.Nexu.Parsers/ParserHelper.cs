namespace Our.Umbraco.Nexu.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Hosting;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using HtmlAgilityPack;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The parser helper.
    /// </summary>
    public static class ParserHelper
    {
        private const string DocumentUdiPrefix = "umb://document/";

        private const string MediaUdiPrefix = "umb://media/";

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
            if (HttpContext.Current == null)
            {
                return HostingEnvironment.IsHosted
                    ? HostingEnvironment.MapPath(virtualPath)
                    : Path.Combine(Directory.GetCurrentDirectory(), virtualPath);
            }

            return HttpContext.Current.Server.MapPath(virtualPath);
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
        /// Parse rich text for v 76.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="contentService">
        /// The content service.
        /// </param>
        /// <param name="mediaService">
        /// The media service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache provider.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<ILinkedEntity> ParseRichTextForV76(
            string html,
            IContentService contentService,
            IMediaService mediaService,
            ICacheProvider cacheProvider)
        {
            var entities = new List<ILinkedEntity>();

            var documentUdiMatches = Regex.Matches(html, "umb://document/(.{32})");

            foreach (Match match in documentUdiMatches)
            {
                var udi = match.Value;

                if (IsDocumentUdi(udi))
                {
                    var id = MapDocumentUdiToId(contentService, cacheProvider, udi);

                    if (id > -1)
                    {
                        if (!entities.Any(x => x.Id == id && x.LinkedEntityType == LinkedEntityType.Document))
                        {
                            entities.Add(new LinkedDocumentEntity(id));
                        }
                    }
                }
            }

            var mediaUdiMatches = Regex.Matches(html, "umb://media/(.{32})");

            foreach (Match match in mediaUdiMatches)
            {
                var udi = match.Value;

                if (IsMediaUdi(udi))
                {
                    var id = MapMediaUdiToId(mediaService, cacheProvider, udi);

                    if (id > -1)
                    {
                        if (!entities.Any(x => x.Id == id && x.LinkedEntityType == LinkedEntityType.Media))
                        {
                            entities.Add(new LinkedMediaEntity(id));
                        }
                    }
                }
            }

            return entities;
        }

        /// <summary>
        /// Checks if a string is a new V7.6 document UDI
        /// </summary>
        /// <param name="udi">
        /// The uid.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDocumentUdi(string udi)
        {
            if (string.IsNullOrEmpty(udi))
            {
                return false;
            }

            return udi.StartsWith(DocumentUdiPrefix);
        }

        /// <summary>
        /// Check if a string is a new v7.6 media UDI
        /// </summary>
        /// <param name="udi">
        /// The udi.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsMediaUdi(string udi)
        {
            if (string.IsNullOrEmpty(udi))
            {
                return false;
            }

            return udi.StartsWith(MediaUdiPrefix);
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
        /// <param name="udi">
        /// The uid.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int MapDocumentUdiToId(IContentService contentService, ICacheProvider cacheProvider, string udi)
        {
            var key = udi.TrimStart(DocumentUdiPrefix);

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

        /// <summary>
        /// Maps a v7.6 media UDI to a integer id
        /// </summary>
        /// <param name="mediaService">
        /// The media service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache provider.
        /// </param>
        /// <param name="udi">
        /// The udi.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int MapMediaUdiToId(IMediaService mediaService, ICacheProvider cacheProvider, string udi)
        {
            var key = udi.TrimStart(MediaUdiPrefix);

            // cache the key and id in static cache for faster future lookups                    
            return cacheProvider.GetCacheItem<int>(
                $"Nexu_Media_Udi_Cache_{key}",
                () =>
                {
                    var attemptGuid = key.TryConvertTo<Guid>();

                    if (attemptGuid.Success)
                    {
                        var media = mediaService.GetById(attemptGuid.Result);

                        if (media != null)
                        {
                            return media.Id;
                        }
                    }

                    return -1;
                });
        }
    }
}
