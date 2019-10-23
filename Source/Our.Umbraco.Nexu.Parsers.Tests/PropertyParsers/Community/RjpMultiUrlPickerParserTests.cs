namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Community
{
    using System;
    using System.Linq;

    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Community;

    /// <summary>
    /// The rjp multi url picker parser tests.
    /// </summary>
    [TestFixture]
    public class RjpMultiUrlPickerParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid data type.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestIsParserForValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("RJP.MultiUrlPicker");

            var parser = new RjpMultiUrlPickerParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for valid data type.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestIsParserForNewDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("Umbraco.MultiUrlPicker");

            var parser = new RjpMultiUrlPickerParser();

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
        [Category("CommunityPropertyParsers")]
        public void TestIsParserForInValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("foo");

            var parser = new RjpMultiUrlPickerParser();

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
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange
            var parser = new RjpMultiUrlPickerParser();

            object propValue = null;

            // act
            var result = parser.GetLinkedEntities(propValue);

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
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesWithValue()
        {
            // arrange
            var parser = new RjpMultiUrlPickerParser();

            var contentId = 1072;
            var mediaId = 1092;

            string propValue = $@"[
                          {{
                            ""id"": ""{contentId}"",
                            ""name"": ""Extend"",
                            ""url"": ""/extend/"",
                            ""icon"": ""icon-stamp""
                          }},
                          {{
                            ""id"": {mediaId},
                            ""name"": ""Doc-Type-Grid-Editor---Developers-Guide-v1.1.pdf"",
                            ""url"": ""/media/1057/doc-type-grid-editor-developers-guide-v11.pdf"",
                            ""isMedia"": true,
                            ""icon"": ""icon-document""
                          }},
                          {{
                            ""name"": ""http://www.google.com"",
                            ""url"": ""http://www.google.com"",
                            ""icon"": ""icon-link""
                          }},
                          {{
                            ""id"": null,
                            ""name"": ""Doc-Type-Grid-Editor---Developers-Guide-v1.1.pdf"",
                            ""url"": ""/media/1057/doc-type-grid-editor-developers-guide-v11.pdf"",
                            ""isMedia"": true,
                            ""icon"": ""icon-document""
                          }}
                        ]";

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentId));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == mediaId));
        }

        /// <summary>
        /// The test get linked entities with Udi value.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesWithUdiValues()
        {
            // arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            cacheProviderMock.Setup(x => x.GetCacheItem(It.IsAny<string>(), It.IsAny<Func<object>>()))
                .Returns((string k, Func<object> action) => action());

            var contentKey = "633927e3e97e4769a9a00e563d0867c8";
            var mediaKey = "ee44da039a9546788fdfa76ae40d1581";

            var contentGuid = Guid.Parse(contentKey);
            var mediaGuid = Guid.Parse(mediaKey);

            var contentId = 1072;
            var mediaId = 1092;

            string propValue = $@"[
                          {{
                            ""udi"": ""umb://document/{contentKey}"",
                            ""name"": ""Extend"",
                            ""url"": ""/extend/""
                          }},
                          {{
                            ""udi"": ""umb://media/{mediaKey}"",
                            ""name"": ""Doc-Type-Grid-Editor---Developers-Guide-v1.1.pdf"",
                            ""url"": ""/media/1057/doc-type-grid-editor-developers-guide-v11.pdf""
                          }},
                          {{
                            ""name"": ""http://www.google.com"",
                            ""url"": ""http://www.google.com""
                          }}
                        ]";

            var contentServiceMock = new Mock<IContentService>();
            var mediaServiceMock = new Mock<IMediaService>();

            var contentMock = new Mock<IContent>();
            contentMock.SetupGet(x => x.Id).Returns(contentId);

            var mediaMock = new Mock<IMedia>();
            mediaMock.SetupGet(x => x.Id).Returns(mediaId);

            contentServiceMock.Setup(x => x.GetById(contentGuid)).Returns(contentMock.Object);
            mediaServiceMock.Setup(x => x.GetById(mediaGuid)).Returns(mediaMock.Object);

            var parser = new RjpMultiUrlPickerParser(contentServiceMock.Object, mediaServiceMock.Object, cacheProviderMock.Object);

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentId));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == mediaId));
        }
    }
}
