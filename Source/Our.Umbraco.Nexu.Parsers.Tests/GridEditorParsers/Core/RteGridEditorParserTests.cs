namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Core
{
    using System;
    using System.Linq;

    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The rte grid editor parser tests.
    /// </summary>
    [TestFixture]
    public class RteGridEditorParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid view
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestIsParserForValidView()
        {
            // arrange            
            var parser = new RteGridEditorParser();

            // act
            var result = parser.IsParserFor("rte");

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for in valid data type.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestIsParserForInValidView()
        {
            // arrange            
            var parser = new RteGridEditorParser();

            // act
            var result = parser.IsParserFor("foo");

            // verify
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The test get linked entities with empty value.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange            
            var parser = new RteGridEditorParser();

            // act
            var result = parser.GetLinkedEntities(null);

            // verify
            Assert.IsNotNull(result);

            Assert.AreEqual(0, result.Count());
        }

        /// <summary>
        /// The test get linked entities.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestGetLinkedEntities()
        {
            // arrange            
            var parser = new RteGridEditorParser();

            var contentId = 1068;

            string value = $@"<p>Test rich text editor with <a data-id=\""{contentId}\"" href=\""/{{localLink:{contentId}}}\"" title=\""Explore\"">links</a></p>\n<p> </p>";

            // act
            var result = parser.GetLinkedEntities(value).ToList();

            // verify
            Assert.IsNotNull(result);

            Assert.AreEqual(1, result.Count());

            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentId));
        }

        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestGetLinkedEntitiesWithValueV76()
        {
            // arrange
            var cacheProviderMock = new Mock<ICacheProvider>();
            cacheProviderMock.Setup(x => x.GetCacheItem(It.IsAny<string>(), It.IsAny<Func<object>>()))
                .Returns((string k, Func<object> action) => action());

            var contentKey = "9bbbfed045e046bdabf2ea0c9ceef572";
            var media1Key = "f6aaf1ee4d2d47ad937a53d59687eace";
            var media2Key = "edfc78f2521244c883893063988837c8";

            var html =
                $@"<p>This is rich text </p>
                <p>This can contain links to <a data-udi=""umb://document/{contentKey}"" href=""/{{localLink:umb://document/{contentKey}}}"" title=""Contact"">content</a></p>
                <p>Have images inserted </p>
                <p><img style=""width: 500px; height:375px;"" src=""/media/1054/img_20160817_113309.jpg?width=500&amp;height=375"" alt="""" data-udi=""umb://media/{media1Key}"" /></p>
                <p> </p>
                <p>And links to <a data-udi=""umb://media/{media2Key}"" href=""/media/1057/djace.pdf"" title=""Djace.pdf"">files</a></p>";

            var contentGuid = Guid.Parse(contentKey);
            var media1Guid = Guid.Parse(media1Key);
            var media2Guid = Guid.Parse(media2Key);

            var contentId = 1500;
            var media1Id = 1601;
            var media2Id = 1602;

            var contentMock = new Mock<IContent>();
            contentMock.SetupGet(x => x.Id).Returns(contentId);

            var contentServiceMock = new Mock<IContentService>();
            contentServiceMock.Setup(x => x.GetById(contentGuid)).Returns(contentMock.Object);

            var mediaService = new Mock<IMediaService>();

            var media1Mock = new Mock<IMedia>();
            media1Mock.SetupGet(x => x.Id).Returns(media1Id);

            var media2Mock = new Mock<IMedia>();
            media2Mock.SetupGet(x => x.Id).Returns(media2Id);

            mediaService.Setup(x => x.GetById(media1Guid)).Returns(media1Mock.Object);
            mediaService.Setup(x => x.GetById(media2Guid)).Returns(media2Mock.Object);

            var parser = new RteGridEditorParser(
                             contentServiceMock.Object,
                             mediaService.Object,
                             cacheProviderMock.Object);
            // act
            var result = parser.GetLinkedEntities(html);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(3, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentId));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == media1Id));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == media2Id));
        }
    }
}
