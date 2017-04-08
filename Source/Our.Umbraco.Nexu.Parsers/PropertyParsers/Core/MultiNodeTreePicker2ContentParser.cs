namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The multi node tree picker 2 content parser in Umbraco v7.6
    /// </summary>
    public class MultiNodeTreePicker2ContentParser : IPropertyParser
    {
        /// <summary>
        /// The data type service.
        /// </summary>
        private readonly IDataTypeService dataTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePicker2ContentParser"/> class.
        /// </summary>
        public MultiNodeTreePicker2ContentParser()
        {
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePicker2ContentParser"/> class.
        /// </summary>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public MultiNodeTreePicker2ContentParser(IDataTypeService dataTypeService)
        {
            this.dataTypeService = dataTypeService;
        }

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
            if (
               !dataTypeDefinition.PropertyEditorAlias.Equals("Umbraco.MultiNodeTreePicker2"))
            {
                return false;
            }

            var prevalues =
                this.dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id).PreValuesAsDictionary;

            if (!prevalues.ContainsKey("startNode"))
            {
                return false;
            }

            var startNodeType = JsonConvert.DeserializeObject<JObject>(prevalues["startNode"].Value).Value<string>("type");

            if (startNodeType == null || startNodeType != "content")
            {
                return false;
            }

            return true;
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
            return Enumerable.Empty<ILinkedEntity>();
        }
    }
}
