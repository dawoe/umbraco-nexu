namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The multi node tree picker 2 content parser tests.
    /// </summary>
    [TestFixture]
    public class MultiNodeTreePicker2ContentParserTests
    {
        /// <summary>
        /// The test is parser for valid data type.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestIsParserForValidDataType()
        {
            // arrange
            var dataTypeServiceMock = new Mock<IDataTypeService>();

            var dataTypeDefinition =
                new DataTypeDefinition("Umbraco.MultiNodeTreePicker2")
                {
                    Id = 1
                };

            var prevalue = new JObject(new JProperty("type", "content"));

            dataTypeServiceMock.Setup(x => x.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id))
                .Returns(
                    new PreValueCollection(
                        new Dictionary<string, PreValue> { { "startNode", new PreValue(prevalue.ToString()) } }));

            var parser = new MultiNodeTreePicker2ContentParser(dataTypeServiceMock.Object);

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
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestIsParserForValidDataTypeWithInvalidPrevalue()
        {
            // arrange
            var dataTypeServiceMock = new Mock<IDataTypeService>();

            var dataTypeDefinition =
                new DataTypeDefinition("Umbraco.MultiNodeTreePicker2")
                {
                    Id = 1
                };

            var prevalue = new JObject(new JProperty("type", "media"));

            dataTypeServiceMock.Setup(x => x.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id))
                .Returns(
                    new PreValueCollection(
                        new Dictionary<string, PreValue> { { "startNode", new PreValue(prevalue.ToString()) } }));

            var parser = new MultiNodeTreePicker2ContentParser(dataTypeServiceMock.Object);

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
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestIsParserForInValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("foo");

            var parser = new MultiNodeTreePicker2ContentParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }
    }
}
