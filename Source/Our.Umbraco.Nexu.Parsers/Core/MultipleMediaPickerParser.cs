namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The media picker parser.
    /// </summary>
    public class MultipleMediaPickerParser : IPropertyParser
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
                    global::Umbraco.Core.Constants.PropertyEditors.MultipleMediaPickerAlias);
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

            var idlist = propertyValue.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            idlist.ForEach(
                x =>
                    {
                        var attemptId = x.TryConvertTo<int>();

                        if (attemptId.Success)
                        {
                            entities.Add(new LinkedMediaEntity(attemptId.Result));
                        }
                    });

            return entities;
        }
    }
}
