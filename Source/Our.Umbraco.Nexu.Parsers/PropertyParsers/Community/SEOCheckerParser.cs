namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Community
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Core.Cache;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;
    public class SEOCheckerParser : IPropertyParser
    {
        /// <summary>
        /// The media service.
        /// </summary>
        private readonly IMediaService mediaService;

        /// <summary>
        /// The cache provider.
        /// </summary>
        private readonly ICacheProvider cacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SEOCheckerParser"/> class.
        /// </summary>
        public SEOCheckerParser()
        {
            this.mediaService = ApplicationContext.Current.Services.MediaService;
            this.cacheProvider = ApplicationContext.Current.ApplicationCache.StaticCache;
        }

        /// <inheritdoc />
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return dataTypeDefinition.PropertyEditorAlias.Equals("SEOChecker.SEOCheckerSocialPropertyEditor");
        }
        /// <inheritdoc />
        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }
            var entities = new List<ILinkedEntity>();
            var model = XDocument.Parse(propertyValue.ToString()).Root;
            if (model != null)
            {
                try
                {
                    var socialImage = model.Descendants("socialImage").FirstOrDefault();
                    if (socialImage == null)
                    {
                        return Enumerable.Empty<ILinkedEntity>();
                    }

                    if (string.IsNullOrEmpty(socialImage.Value))
                    {
                        return Enumerable.Empty<ILinkedEntity>();
                    }
                     var id = ParserHelper.MapMediaUdiToId(this.mediaService, this.cacheProvider, socialImage.Value);

                    if (id > -1)
                    {
                        entities.Add(new LinkedMediaEntity(id));
                    }

                    return entities;
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this.GetType(), $"An error occured parsing SEO Checker property", ex);
                }
            }
            return entities;
        }
        
    }
}
