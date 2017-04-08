namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System;
    using System.Linq;

    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The media picker 2 parser tests.
    /// </summary>
    [TestFixture]
    public class MediaPicker2ParserTests : BaseParserTest
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
            var dataTypeDefinition = new DataTypeDefinition("Umbraco.MediaPicker2");

            var parser = new MediaPicker2Parser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsTrue(result);
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

            var parser = new MediaPicker2Parser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test getting linked entities with a value
        /// </summary>
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

            var mediaId1 = 1500;
            var mediaId2 = 1600;

            var mediaServiceMock = new Mock<IMediaService>();

            var media1Mock = new Mock<IMedia>();
            media1Mock.SetupGet(x => x.Id).Returns(mediaId1);

            var media2Mock = new Mock<IMedia>();
            media2Mock.SetupGet(x => x.Id).Returns(mediaId2);

            mediaServiceMock.Setup(x => x.GetById(guid1)).Returns(media1Mock.Object);
            mediaServiceMock.Setup(x => x.GetById(guid2)).Returns(media2Mock.Object);

            var parser = new MediaPicker2Parser(mediaServiceMock.Object, cacheProviderMock.Object);

            // act
            var result = parser.GetLinkedEntities(value);

            // verify
            mediaServiceMock.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Exactly(2));

            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count());

            Assert.IsTrue(entities.Any(x => x.Id == mediaId1 && x.LinkedEntityType == LinkedEntityType.Media));
            Assert.IsTrue(entities.Any(x => x.Id == mediaId2 && x.LinkedEntityType == LinkedEntityType.Media));
        }

        /// <summary>
        /// Test getting linked entities with a empty value
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange
            var parser = new MediaPicker2Parser();

            // act
            var result = parser.GetLinkedEntities(null);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(0, entities.Count());
        }
    }
}
