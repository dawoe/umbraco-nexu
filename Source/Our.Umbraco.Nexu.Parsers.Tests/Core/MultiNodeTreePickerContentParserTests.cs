namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Parsers.Community;
    using Our.Umbraco.Nexu.Parsers.Core;

    /// <summary>
    /// The multi node tree picker content parser tests.
    /// </summary>
    [TestFixture]
    public class MultiNodeTreePickerContentParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid data type.
        /// </summary>
        [Test]
        [Category("Parsers")]
        [Category("CoreParsers")]
        public void TestIsParserForValidDataType()
        {
            // arrange
            var dataTypeServiceMock = new Mock<IDataTypeService>();

            var dataTypeDefinition =
                new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.MultiNodeTreePickerAlias)
                    {
                        Id = 1
                    };

            var prevalue = new JObject(new JProperty("type", "content"));

            dataTypeServiceMock.Setup(x => x.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id))
                .Returns(
                    new PreValueCollection(
                        new Dictionary<string, PreValue> { { "startNode", new PreValue(prevalue.ToString()) } }));

            var parser = new MultiNodeTreePickerContentParser(dataTypeServiceMock.Object);

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            dataTypeServiceMock.Verify(x => x.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id), Times.Once);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for valid data type with invalid prevalue.
        /// </summary>
        [Test]
        [Category("Parsers")]
        [Category("CoreParsers")]
        public void TestIsParserForValidDataTypeWithInvalidPrevalue()
        {
            // arrange
            var dataTypeServiceMock = new Mock<IDataTypeService>();

            var dataTypeDefinition =
                new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.MultiNodeTreePickerAlias)
                {
                    Id = 1
                };

            var prevalue = new JObject(new JProperty("type", "media"));

            dataTypeServiceMock.Setup(x => x.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id))
                .Returns(
                    new PreValueCollection(
                        new Dictionary<string, PreValue> { { "startNode", new PreValue(prevalue.ToString()) } }));

            var parser = new MultiNodeTreePickerContentParser(dataTypeServiceMock.Object);

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            dataTypeServiceMock.Verify(x => x.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id), Times.Once);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// The test is parser for in valid data type.
        /// </summary>
        [Test]
        [Category("Parsers")]
        [Category("CoreParsers")]
        public void TestIsParserForInValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("foo");

            var parser = new MultiNodeTreePickerContentParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }
    }
}
