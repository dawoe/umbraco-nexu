namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Community
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
    /// The rjp multi url picker parser.
    /// </summary>
    public class RjpMultiUrlPickerParser : IPropertyParser
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
            return dataTypeDefinition.PropertyEditorAlias.Equals("RJP.MultiUrlPicker");
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
                var jsonValues = JsonConvert.DeserializeObject<JArray>(propertyValue.ToString());

                foreach (var item in jsonValues)
                {
                    if (item["id"] != null)
                    {
                        var attempId = item["id"].TryConvertTo<int>();

                        if (attempId.Success)
                        {
                            var isMedia = false;

                            if (item["isMedia"] != null)
                            {
                                var attemptIsMedia = item["isMedia"].TryConvertTo<bool>();

                                if (attemptIsMedia.Success)
                                {
                                    isMedia = attemptIsMedia.Result;
                                }
                            }

                            if (isMedia)
                            {
                                entities.Add(new LinkedMediaEntity(attempId.Result));
                            }
                            else
                            {
                                entities.Add(new LinkedDocumentEntity(attempId.Result));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ApplicationContext.Current.ProfilingLogger.Logger.Error<RjpMultiUrlPickerParser>("Error parsing multi url picker", exception);         
            }

            return entities;
        }
    }
}
