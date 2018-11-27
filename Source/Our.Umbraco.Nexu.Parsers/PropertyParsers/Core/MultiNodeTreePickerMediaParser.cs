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
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The multi node tree picker media parser.
    /// </summary>
    public class MultiNodeTreePickerMediaParser : IPropertyParser
    {

        /// <summary>
        /// The data type service.
        /// </summary>
        private readonly IDataTypeService dataTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePickerMediaParser"/> class.
        /// </summary>
        public MultiNodeTreePickerMediaParser()
        {
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePickerMediaParser"/> class.
        /// </summary>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public MultiNodeTreePickerMediaParser(IDataTypeService dataTypeService)
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
                !dataTypeDefinition.PropertyEditorAlias.Equals(
                    global::Umbraco.Core.Constants.PropertyEditors.MultiNodeTreePickerAlias))
            {
                return false;
            }

            var prevalues =
                this.dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id).FormatAsDictionary();

            if (!prevalues.ContainsKey("startNode"))
            {
                return false;
            }

            var startNodeType = JsonConvert.DeserializeObject<JObject>(prevalues["startNode"].Value).Value<string>("type");

            if (startNodeType == null || startNodeType != "media")
            {
                return false;
            }

            return true;
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            return ParserHelper.GetLinkedEntitiesFromCsvString<LinkedMediaEntity>(propertyValue.ToString());
        }
    }
}
