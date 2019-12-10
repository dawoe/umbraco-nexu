using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Our.Umbraco.Nexu.Core;
using Our.Umbraco.Nexu.Core.Enums;
using Our.Umbraco.Nexu.Core.Interfaces;
using Our.Umbraco.Nexu.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Community
{
    public class RichTextEditorMacroParser : IPropertyParser
    {
        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService _contentService;

        /// <summary>
        /// The media service.
        /// </summary>
        private readonly IMediaService _mediaService;

        /// <summary>
        /// The cache provider.
        /// </summary>
        private readonly ICacheProvider _cacheProvider;

        /// <summary>
        /// Number format provider.
        /// </summary>
        private static readonly IFormatProvider _numberFormatProvider = new NumberFormatInfo();

        /// <summary>
        /// Macro regex persisted format.
        /// </summary>
        private static readonly Regex MacroPersistedFormat =
            new Regex(@"(<\?UMBRACO_MACRO (?:.+?)??macroAlias=[""']([^""\'\n\r]+?)[""'].+?)(?:/>|>.*?</\?UMBRACO_MACRO>)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextEditorMacroParser"/> class.
        /// </summary>
        public RichTextEditorMacroParser()
        {
            _mediaService = ApplicationContext.Current.Services.MediaService;
            _cacheProvider = ApplicationContext.Current.ApplicationCache.StaticCache;

            GetCustomMacroAttributes();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextEditorMacroParser"/> class.
        /// </summary>
        /// <param name="contentService">
        /// The content service.
        /// </param>
        /// <param name="mediaService">
        /// The media service.
        /// </param>
        /// <param name="cacheProvider">
        /// The cache provider.
        /// </param>
        public RichTextEditorMacroParser(IContentService contentService, IMediaService mediaService, ICacheProvider cacheProvider)
        {
            _contentService = contentService;
            _mediaService = mediaService;
            _cacheProvider = cacheProvider;

            GetCustomMacroAttributes();
        }

        private const string Separator = ",";

        private void GetCustomMacroAttributes()
        {
            var nexuContext = NexuContext.Current;

            MacroDocumentAttributes = nexuContext.MacroDocumentAttributeNames.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            MacroMediaAttributes = nexuContext.MacroMediaAttributeNames.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        private IEnumerable<string> MacroDocumentAttributes { get; set; }

        private IEnumerable<string> MacroMediaAttributes { get; set; }

        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
            => dataTypeDefinition.PropertyEditorAlias.Equals(Constants.PropertyEditors.TinyMCEAlias);

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            var entitiesResults = new List<ILinkedEntity>();
            var propertyText = propertyValue.ToString();

            if (propertyText.IsNullOrWhiteSpace()) return entitiesResults;

            var html = new HtmlDocument();
            html.LoadHtml(propertyText);

            var macroNodes = html.DocumentNode.SelectNodes("//comment()[contains(., '<?UMBRACO_MACRO')]");

            if (macroNodes == null || !macroNodes.Any()) return entitiesResults;

            foreach (var macroNode in macroNodes)
            {
                var attributes = GetMacrosAttributesWithValues(macroNode);
                var documentIds = GetAttributesDocumentIds(attributes);
                var mediaIds = GetAttributesMediaIds(attributes);

                foreach (var documentId in documentIds)
                {
                    var linkedEntity = GetLinkedEntity(documentId, LinkedEntityType.Document);

                    if (linkedEntity != null && !entitiesResults.Any(x => x.Id == linkedEntity.Id
                                                 && x.LinkedEntityType == LinkedEntityType.Document))
                    {
                        entitiesResults.Add(linkedEntity);
                    }
                }

                foreach (var mediaId in mediaIds)
                {
                    var linkedEntity = GetLinkedEntity(mediaId, LinkedEntityType.Media);

                    if (linkedEntity != null && !entitiesResults.Any(x => x.Id == linkedEntity.Id
                                                 && x.LinkedEntityType == LinkedEntityType.Media))
                    {
                        entitiesResults.Add(linkedEntity);
                    }
                }
            }

            return entitiesResults;
        }

        private IEnumerable<string> GetAttributesMediaIds(Dictionary<string, string> attributes)
            => attributes.Where(attribute => MacroMediaAttributes.Contains(attribute.Key.ToLower()))
                .Select(attribute => attribute.Value.Trim());

        private IEnumerable<string> GetAttributesDocumentIds(Dictionary<string, string> attributes)
            => attributes.Where(attribute => MacroDocumentAttributes.Contains(attribute.Key.ToLower()))
                .Select(attribute => attribute.Value.Trim());

        private ILinkedEntity GetLinkedEntity(string id, LinkedEntityType linkedEntityType)
        {
            if (!int.TryParse(id, NumberStyles.AllowDecimalPoint, _numberFormatProvider, out var _id)) return null;

            switch (linkedEntityType)
            {
                case LinkedEntityType.Document:
                    if (MapDocumentId(_id) > -1) return new LinkedDocumentEntity(_id);
                    break;
                case LinkedEntityType.Media:
                    if (MapMediaId(_id) > -1) return new LinkedMediaEntity(_id);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(linkedEntityType), linkedEntityType, null);
            }

            return null;
        }

        private static Dictionary<string, string> GetMacrosAttributesWithValues(HtmlNode macroNode)
        {
            var macroMatch = MacroPersistedFormat.Match(macroNode.InnerHtml);

            return XmlHelper.GetAttributesFromElement(macroMatch.Value)
                .ToDictionary(x => x.Key, y => y.Value);
        }

        private int MapDocumentId(int documentId)
            => _cacheProvider.GetCacheItem<int>(
                $"Nexu_Document_Id_Cache_{documentId}",
                () =>
                {
                    var content = _contentService.GetById(documentId);
                    return content?.Id ?? -1;
                });

        private int MapMediaId(int mediaId)
            => _cacheProvider.GetCacheItem<int>(
                $"Nexu_Media_Id_Cache_{mediaId}",
                () =>
                {
                    var media = _mediaService.GetById(mediaId);
                    return media?.Id ?? -1;
                });
    }
}