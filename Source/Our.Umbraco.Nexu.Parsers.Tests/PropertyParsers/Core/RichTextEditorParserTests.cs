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
    /// The rich text editor parser tests.
    /// </summary>
    public class RichTextEditorParserTests : BaseParserTest
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
            var dataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias);

            var parser = new RichTextEditorParser();

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

            var parser = new RichTextEditorParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
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
            var parser = new RichTextEditorParser();

            var propertyType = new PropertyType(
                              global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias,
                              DataTypeDatabaseType.Ntext,
                              "cp1");

            var property = new Property(propertyType, null);

            // act
            var result = parser.GetLinkedEntities(property.Value);

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
            var parser = new RichTextEditorParser();

            var propertyType = new PropertyType(
                              global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias,
                              DataTypeDatabaseType.Ntext,
                              "cp1");

            var html =
                @"<p>This content linking to <a data-id=""1069"" href=""/{localLink:1069}"" title=""Our Umbraco"">Our Umbraco</a> and <a data-id=""1070"" href=""/{localLink:1070}"" title=""Codegarden"">Codegarden</a></p>
                    <p>We can also insert several images</p>
                    <p><img style=""width: 500px; height: 375px;"" src=""/media/1054/pensive-parakeet.jpg?width=500&amp;height=375"" alt="""" rel=""1087"" data-id=""1087"" /></p>
                    <p> </p>
                    <p>And another one :</p>
                    <p><img style=""width: 500px; height: 375px;"" src=""/media/1050/costa-rican-frog.jpg?width=500&amp;height=375"" alt="""" rel=""1086"" data-id=""1086"" /></p>
                    <p> </p>
                    <p>And link to files in the media section</p>
                    <p><a data-id=""1084"" href=""/media/1056/pic04.jpg"" title=""pic04.jpg"">Image linked</a></p>
                    <p><a data-id=""1092"" href=""/media/1057/doc-type-grid-editor-developers-guide-v11.pdf"" title=""Doc-Type-Grid-Editor---Developers-Guide-v1.1.pdf"">Pdf linked</a></p>
                    <p> </p>
                    <p> </p>";

            var property = new Property(propertyType, html);

            // act
            var result = parser.GetLinkedEntities(property.Value);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(6, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 1069));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 1070));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 1086));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 1087));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 1084));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 1092));
        }

        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
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

            var parser = new RichTextEditorParser(
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
