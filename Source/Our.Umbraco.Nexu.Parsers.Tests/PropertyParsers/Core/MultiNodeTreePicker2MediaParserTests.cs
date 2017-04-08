namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The multi node tree picker 2 media parser tests.
    /// </summary>
    [TestFixture]
    public class MultiNodeTreePicker2MediaParserTests : BaseParserTest
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

            var prevalue = new JObject(new JProperty("type", "media"));

            dataTypeServiceMock.Setup(x => x.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id))
                .Returns(
                    new PreValueCollection(
                        new Dictionary<string, PreValue> { { "startNode", new PreValue(prevalue.ToString()) } }));

            var parser = new MultiNodeTreePicker2MediaParser(dataTypeServiceMock.Object);

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

            var prevalue = new JObject(new JProperty("type", "content"));

            dataTypeServiceMock.Setup(x => x.GetPreValuesCollectionByDataTypeId(dataTypeDefinition.Id))
                .Returns(
                    new PreValueCollection(
                        new Dictionary<string, PreValue> { { "startNode", new PreValue(prevalue.ToString()) } }));

            var parser = new MultiNodeTreePicker2MediaParser(dataTypeServiceMock.Object);

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

            var parser = new MultiNodeTreePicker2MediaParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }

        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithValue()
        {
            // arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            cacheProviderMock.Setup(x => x.GetCacheItem(It.IsAny<string>(), It.IsAny<Func<object>>()))
                .Returns((string k, Func<object> action) => action());

            var key1 = "386b813a1e1e4f2a91f72f06e893197b";
            var key2 = "606c077399ee4dc58e5f12ba38b3b527";

            var value = $"umb://media/{key1},umb://media/{key2}";

            var guid1 = Guid.Parse(key1);
            var guid2 = Guid.Parse(key2);

            var media1Id = 1500;
            var media2Id = 1600;

            var mediaService = new Mock<IMediaService>();

            var media1Mock = new Mock<IMedia>();
            media1Mock.SetupGet(x => x.Id).Returns(media1Id);

            var media2Mock = new Mock<IMedia>();
            media2Mock.SetupGet(x => x.Id).Returns(media2Id);

            mediaService.Setup(x => x.GetById(guid1)).Returns(media1Mock.Object);
            mediaService.Setup(x => x.GetById(guid2)).Returns(media2Mock.Object);

            var parser = new MultiNodeTreePicker2MediaParser(mediaService.Object, cacheProviderMock.Object);

            // act
            var result = parser.GetLinkedEntities(value);

            // verify
            mediaService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Exactly(2));

            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count());

            Assert.IsTrue(entities.Any(x => x.Id == media1Id && x.LinkedEntityType == LinkedEntityType.Media));
            Assert.IsTrue(entities.Any(x => x.Id == media2Id && x.LinkedEntityType == LinkedEntityType.Media));
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
            var parser = new MultiNodeTreePicker2MediaParser();

            // act
            var result = parser.GetLinkedEntities(null);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(0, entities.Count());
        }
    }
}
