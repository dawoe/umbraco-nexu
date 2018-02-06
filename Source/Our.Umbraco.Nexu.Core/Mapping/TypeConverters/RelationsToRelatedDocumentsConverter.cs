using System.Threading;

namespace Our.Umbraco.Nexu.Core.Mapping.TypeConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutoMapper;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The relation to related document type converter
    /// </summary>
    internal class RelationsToRelatedDocumentsConverter : ITypeConverter<IEnumerable<IRelation>, IEnumerable<RelatedDocument>>
    {
        /// <summary>
        /// The content service.
        /// </summary>
        private readonly IContentService contentService;

        /// <summary>
        /// The localization service.
        /// </summary>
        private readonly ILocalizationService localizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationsToRelatedDocumentsConverter"/> class.
        /// </summary>
        public RelationsToRelatedDocumentsConverter()
        {
            this.contentService = ApplicationContext.Current.Services.ContentService;
            this.localizationService = ApplicationContext.Current.Services.LocalizationService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationsToRelatedDocumentsConverter"/> class.
        /// </summary>
        /// <param name="contentservice">
        /// The contentservice.
        /// </param>
        /// <param name="localizationService">
        /// The localization Service.
        /// </param>
        public RelationsToRelatedDocumentsConverter(IContentService contentservice, ILocalizationService localizationService)
        {
            this.contentService = contentservice;
            this.localizationService = localizationService;
        }

        /// <summary>
        /// Converts the source to destination type
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<RelatedDocument> Convert(ResolutionContext context)
        {
            var destination = new List<RelatedDocument>();

            if (context.IsSourceValueNull)
            {
                return destination;
            }

            var source = context.SourceValue as List<IRelation>;

            if (source == null)
            {
                return destination;
            }

            if (!source.Any())
            {
                return destination;
            }

            var ids = source.Select(x => x.ParentId).ToList();

            var contentItems = this.contentService.GetByIds(ids).ToList();

            var language = Thread.CurrentThread.CurrentCulture.Name;

            destination = Mapper.Map<IEnumerable<RelatedDocument>>(contentItems).ToList();

            // set the comments from the relation
            foreach (var item in destination)
            {
                var relation = source.FirstOrDefault(src => src.ParentId == item.Id);

                if (relation != null)
                {
                    if (!string.IsNullOrEmpty(relation.Comment))
                    {
                        var commentItems = relation.Comment.Split(
                            new string[] { "||" },
                            StringSplitOptions.RemoveEmptyEntries);

                        foreach (var prop in commentItems)
                        {
                            var matches = Regex.Matches(prop, "(.*)(?:\\[\\[)([^\\]\\]]*)(?:\\]\\])");

                            var tabname = matches[0].Groups[2].Value;

                            if (string.IsNullOrEmpty(tabname))
                            {
                                tabname = "Generic";
                            }

                            if (tabname.StartsWith("#"))
                            {
                                if (this.localizationService.DictionaryItemExists(tabname.Substring(1)))
                                {
                                    var dictionaryItem = this.localizationService.GetDictionaryItemByKey(tabname.Substring(1));
                                    if (dictionaryItem != null)
                                    {
                                        var translations = dictionaryItem.Translations.ToList();
                                        if (translations.Any())
                                        {
                                            var translation = translations.SingleOrDefault(x => x.Language.IsoCode == language);
                                            if (translation != null)
                                            {
                                                tabname = translation.Value;
                                            }
                                        }
                                    }
                                }
                            }

                            var propertyName = matches[0].Groups[1].Value;
                            if (propertyName.StartsWith("#"))
                            {
                                if (this.localizationService.DictionaryItemExists(propertyName.Substring(1)))
                                {
                                    var dictionaryItem = this.localizationService.GetDictionaryItemByKey(propertyName.Substring(1));
                                    if (dictionaryItem != null)
                                    {
                                        var translations = dictionaryItem.Translations.ToList();
                                        if (translations.Any())
                                        {
                                            var translation = translations.SingleOrDefault(x => x.Language.IsoCode == language);
                                            if (translation != null)
                                            {
                                                propertyName = translation.Value;
                                            }
                                        }
                                    }
                                }
                            }
                            if (item.Properties.ContainsKey(tabname))
                            {
                                item.Properties[tabname].Add(propertyName);
                            }
                            else
                            {
                                item.Properties.Add(tabname, new List<string> { propertyName });
                            }
                        }
                    }
                }

            }

            return destination;
        }
    }
}
