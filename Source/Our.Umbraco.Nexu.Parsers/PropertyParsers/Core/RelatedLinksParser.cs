namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The related links parser.
    /// </summary>
    public class RelatedLinksParser : IPropertyParser
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
                    global::Umbraco.Core.Constants.PropertyEditors.RelatedLinksAlias);
        }

        /// <summary>
        /// Gets the linked entites from the property value
        /// </summary>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            var entities = new List<ILinkedEntity>();

            try
            {
                var jsonValue = JsonConvert.DeserializeObject<List<object>>(propertyValue.ToString());

                foreach (JObject item in jsonValue)
                {
                    if (this.IsInternalLink(item))
                    {
                        var attemptId = this.GetInternalId(item).TryConvertTo<int>();

                        if (attemptId.Success)
                        {
                            entities.Add(new LinkedDocumentEntity(attemptId.Result));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<RelatedLinksParser>("Error parsing related links", exception);
            }

            return entities;
        }

        /// <summary>
        /// Check if it is a internal link.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsInternalLink(JObject item)
        {
            bool isInternal = false;

            var isInternalProperty = item["isInternal"];

            if (isInternalProperty != null)
            {
                isInternal = isInternalProperty.ToObject<bool>();                
            }

            return isInternal;
        }

        /// <summary>
        /// Gets the internal id.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetInternalId(JObject item)
        {
            var internalIdProperty = item["internal"];

            return internalIdProperty?.ToObject<string>();
        }
    }
}
