namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

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
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
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
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
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
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
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

        /// <summary>
        /// The test get linked entities with empty value.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange
            var parser = new MultiNodeTreePickerContentParser();          

            // act
            var result = parser.GetLinkedEntities(null);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(0, entities.Count());
        }

        /// <summary>
        /// The test get linked entities with value.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithValue()
        {
            // arrange
            var parser = new MultiNodeTreePickerContentParser();          

            // act
            var result = parser.GetLinkedEntities("100,101,104");

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(3, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 100));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 101));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 104));
        }
    }
}
