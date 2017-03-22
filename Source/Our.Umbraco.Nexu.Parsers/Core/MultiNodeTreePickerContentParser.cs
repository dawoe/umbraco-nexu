namespace Our.Umbraco.Nexu.Parsers.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Parsers.Community;

    /// <summary>
    /// The multi node tree picker content parser.
    /// </summary>
    public class MultiNodeTreePickerContentParser : IPropertyParser
    {

        /// <summary>
        /// The data type service.
        /// </summary>
        private readonly IDataTypeService dataTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePickerContentParser"/> class.
        /// </summary>
        public MultiNodeTreePickerContentParser()
        {
            this.dataTypeService = ApplicationContext.Current.Services.DataTypeService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiNodeTreePickerContentParser"/> class.
        /// </summary>
        /// <param name="dataTypeService">
        /// The data type service.
        /// </param>
        public MultiNodeTreePickerContentParser(IDataTypeService dataTypeService)
        {
            this.dataTypeService = dataTypeService;
        }

        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            throw new System.NotImplementedException();
        }
    }
}
