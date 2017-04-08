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
    /// The related link parser tests in Umbraco V7.6
    /// </summary>
    [TestFixture]
    public class RelatedLink2ParserTests : BaseParserTest
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
            var dataTypeDefinition =
                new DataTypeDefinition("Umbraco.RelatedLinks2");

            var parser = new RelatedLinks2Parser();

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

            var parser = new RelatedLinks2Parser();

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
            var parser = new RelatedLinks2Parser();

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
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithValue()
        {
            // arrange
            var key = "6e79e3fc615c4e47bb7ba0115b8dfd87";

            var guid = Guid.Parse(key);

            var contentId = 1079;

            string propValue = $@"[
                      {{
                        ""caption"": ""External"",
                        ""link"": ""http://www.google.be"",
                        ""newWindow"": false,
                        ""edit"": false,
                        ""isInternal"": false,
                        ""type"": ""external"",
                        ""title"": ""External""
                      }},
                      {{
                        ""caption"": ""Internal page empty"",
                        ""link"": null,
                        ""newWindow"": false,
                        ""internal"": null,
                        ""edit"": false,
                        ""isInternal"": true,
                        ""internalName"": """",
                        ""type"": ""internal"",
                        ""title"": ""Internal page empty""
                      }},
                      {{
                        ""caption"": ""Internal page"",
                        ""link"": ""umb://document/{key}"",
                        ""newWindow"": false,
                        ""internal"": ""umb://document/{key}"",
                        ""edit"": false,
                        ""isInternal"": true,
                        ""internalName"": ""Contact"",
                        ""type"": ""internal"",
                        ""title"": ""Internal page""
                      }},
                      {{
                        ""caption"": ""Empty external"",
                        ""link"": ""http://"",
                        ""newWindow"": false,
                        ""edit"": false,
                        ""isInternal"": false,
                        ""type"": ""external"",
                        ""title"": ""Empty external""
                      }}
                    ]";

            var contentMock = new Mock<IContent>();
            contentMock.SetupGet(x => x.Id).Returns(contentId);

            var contentServiceMock = new Mock<IContentService>();
            contentServiceMock.Setup(x => x.GetById(guid)).Returns(contentMock.Object);

            var cacheProviderMock = new Mock<ICacheProvider>();
            cacheProviderMock.Setup(x => x.GetCacheItem(It.IsAny<string>(), It.IsAny<Func<object>>()))
                .Returns((string k, Func<object> action) => action());


            var parser = new RelatedLinks2Parser(contentServiceMock.Object, cacheProviderMock.Object);

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(1, entities.Count());

           
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentId));
        }
    }
}
