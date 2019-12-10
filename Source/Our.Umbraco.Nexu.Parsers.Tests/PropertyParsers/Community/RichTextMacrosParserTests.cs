namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Community
{
    using System;
    using System.Linq;

    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;
    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core;
    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Community;

    /// <summary>
    /// The rich text editor macros parser tests.
    /// </summary>
    public class RichTextMacrosParserTests : BaseParserTest
    {

        private const int Media1Id = 1111;
        private const int Media2Id = 2222;
        private Mock<IMedia> _media1Mock;
        private Mock<IMedia> _media2Mock;

        private const int Document1Id = 3333;
        private const int Document2Id = 4444;
        private Mock<IContent> _document1Mock;
        private Mock<IContent> _document2Mock;

        private Mock<IContentService> _contentServiceMock;
        private Mock<IMediaService> _mediaService;
        private Mock<ICacheProvider> _cacheProviderMock;


        #region Test data
        private static readonly string TestData = $@"
        <p>Test Test</p>
        <p>First media:</p>
        <div class=""umb-macro-holder Image mceNonEditable umb-macro-mce_1""><!-- <?UMBRACO_MACRO macroAlias=""Image"" imageCrop=""{Media1Id}"" /> --><ins><img class=""article__image"" src=""/media/{Media1Id}/fist_media.png?anchor=center&amp;mode=crop&amp;width=1093&amp;height=350&amp;rnd=132024128040000000"" alt=""First Media"" /></ins></div>
        <p>Second media:</p>
        <div class=""umb-macro-holder Image mceNonEditable umb-macro-mce_2""><!-- <?UMBRACO_MACRO macroAlias=""Image"" imageCrop=""{Media2Id}"" /> --><ins><img class=""article__image"" src=""/media/{Media2Id}/second_media.png?anchor=center&amp;mode=crop&amp;width=1093&amp;height=350&amp;rnd=132040352910000000"" alt=""SecondMedia"" /></ins></div>
        <p>First document:</p>
        <div class=""umb-macro-holder Message mceNonEditable umb-macro-mce_3""><!-- <?UMBRACO_MACRO macroAlias=""Message"" message=""{Document1Id}"" /> --><ins>
        <p class=""disclaimer-box"">DISCLAIMER<br />First document.</p>
        </ins></div>
        <p>Second document:</p>
        <div class=""umb-macro-holder Timeline mceNonEditable umb-macro-mce_4""><!-- <?UMBRACO_MACRO macroAlias=""Timeline"" timeline=""{Document2Id}"" /> --><ins>
        <div class=""timeline timeline--rte"">
        <div class=""timeline-item timeline-item--cs-light-teal"">
        <div class=""timeline-item__icon""><img class=""timeline-item__icon-image"" src=""/images/second_document.png"" alt="""" /></div>
        <span class=""timeline-item__year"">2019</span>
        <div class=""timeline-item__content"">
        <div class=""timeline-item__content-item"">
        <div class=""article"">
        <h3 class=""timeline-item__title"">Title</h3>
        <p>Second document.</p>
        </div>
        </div>
        </div>
        </div>
        <div class=""timeline-item timeline-item--cs-yellow"">
        <div class=""timeline-item__icon""><img class=""timeline-item__icon-image"" src=""/media/{Document2Id}/second_document.png?anchor=center&amp;mode=crop&amp;width=62&amp;height=61&amp;rnd=132092270570000000"" alt="""" /></div>
        <span class=""timeline-item__year"">2018</span>
        <div class=""timeline-item__content"">
        <div class=""timeline-item__content-item"">
        <div class=""article"">
        <h3 class=""timeline-item__title"">Title</h3>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis felis vitae risus pulvinar tincidunt. Nam ac venenatis enim. Aenean hendrerit justo sed.</p>
        </div>
        </div>
        </div>
        </div>
        <div class=""timeline-item timeline-item--cs-blue"">
        <div class=""timeline-item__icon""><img class=""timeline-item__icon-image"" src=""/media/{Document2Id}/timeline-icon.png?anchor=center&amp;mode=crop&amp;width=62&amp;height=61&amp;rnd=132092270570000000"" alt="""" /></div>
        <span class=""timeline-item__year"">2017</span>
        <div class=""timeline-item__content"">
        <div class=""timeline-item__content-item"">
        <div class=""article"">
        <h3 class=""timeline-item__title"">Title</h3>
        <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum mattis felis vitae risus pulvinar tincidunt. Nam ac venenatis enim. Aenean hendrerit justo sed.</p>
        </div>
        </div>
        </div>
        </div>
        </div>
        </ins></div>
        <div class=""umb-macro-holder Quote mceNonEditable umb-macro-mce_2""><!-- <?UMBRACO_MACRO macroAlias=""Quote"" quote=""6196"" /> --><ins>
        <blockquote class=""article__blockquote"">
        <div class=""article__blockquote-inner"">
        <div class=""article__blockquote-body"">Mauris blandit aliquet elit, eget tincidunt nibh pulvinar a. Vestibulum ac diam sit amet quam vehicula elementum sed sit amet dui.</div>
        Test Quote</div>
        </blockquote>
        </ins></div>
";
        #endregion

        [SetUp]
        public void Init()
        {
            // first singleton initializing
            var nexuContext = NexuContext.Current;

            _media1Mock = new Mock<IMedia>();
            _media1Mock.SetupGet(x => x.Id).Returns(Media1Id);

            _media2Mock = new Mock<IMedia>();
            _media2Mock.SetupGet(x => x.Id).Returns(Media2Id);

            _document1Mock = new Mock<IContent>();
            _document1Mock.SetupGet(x => x.Id).Returns(Document1Id);

            _document2Mock = new Mock<IContent>();
            _document2Mock.SetupGet(x => x.Id).Returns(Document2Id);

            _contentServiceMock = new Mock<IContentService>();
            _mediaService = new Mock<IMediaService>();

            _cacheProviderMock = new Mock<ICacheProvider>();
            _cacheProviderMock.Setup(x => x.GetCacheItem(It.IsAny<string>(), It.IsAny<Func<object>>()))
                .Returns((string k, Func<object> action) => action());
        }

        #region General

        /// <summary>
        /// The test is parser for valid data type.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestIsParserForValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias);

            var parser = new RichTextEditorMacrosParser(
                _contentServiceMock.Object,
                _mediaService.Object,
                _cacheProviderMock.Object);

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

            var parser = new RichTextEditorMacrosParser(
                _contentServiceMock.Object,
                _mediaService.Object,
                _cacheProviderMock.Object);

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }

        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestCreatingParser()
        {
            Assert.DoesNotThrow(() =>
            {
                var parser = new RichTextEditorMacrosParser(
                    _contentServiceMock.Object,
                    _mediaService.Object,
                    _cacheProviderMock.Object);

                Assert.IsNotNull(parser);
            });
        }

        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestParserDoesNotThrowOnEmptyValue()
        {
            Assert.DoesNotThrow(() =>
            {
                var parser = new RichTextEditorMacrosParser(
                    _contentServiceMock.Object,
                    _mediaService.Object,
                    _cacheProviderMock.Object);

                parser.GetLinkedEntities("");
            });
        }

        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestParserDoesNotThrowOnNonEmptyValue()
        {
            Assert.DoesNotThrow(() =>
            {
                var parser = new RichTextEditorMacrosParser(
                    _contentServiceMock.Object,
                    _mediaService.Object,
                    _cacheProviderMock.Object);

                parser.GetLinkedEntities(TestData);
            });
        }

        #endregion

        #region Media
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesForMacroMedias()
        {
            // arrange
            var html = TestData;
            var nexuContext = NexuContext.Current;
            nexuContext.MacroMediaAttributeNames = "imageCrop".ToLower();
            nexuContext.MacroDocumentAttributeNames = "";

            _mediaService.Setup(x => x.GetById(Media1Id)).Returns(_media1Mock.Object);
            _mediaService.Setup(x => x.GetById(Media2Id)).Returns(_media2Mock.Object);

            var parser = new RichTextEditorMacrosParser(
                             _contentServiceMock.Object,
                             _mediaService.Object,
                             _cacheProviderMock.Object);
            // act
            var result = parser.GetLinkedEntities(html);

            // verify
            Assert.IsNotNull(result);

            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count);

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == Media1Id));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == Media2Id));
        }

        #endregion

        #region Document
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesForMacroDocuments()
        {
            // arrange
            var html = TestData;
            var nexuContext = NexuContext.Current;

            nexuContext.MacroMediaAttributeNames = "";
            nexuContext.MacroDocumentAttributeNames = "timeline,message".ToLower();

            _contentServiceMock.Setup(x => x.GetById(Document1Id)).Returns(_document1Mock.Object);
            _contentServiceMock.Setup(x => x.GetById(Document2Id)).Returns(_document2Mock.Object);

            var parser = new RichTextEditorMacrosParser(
                             _contentServiceMock.Object,
                             _mediaService.Object,
                             _cacheProviderMock.Object);
            // act
            var result = parser.GetLinkedEntities(html);

            // verify
            Assert.IsNotNull(result);

            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count);

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == Document1Id));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == Document2Id));
        }
        #endregion

        #region Media and Document

        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesForMacroMediasAndDocuments()
        {
            // arrange
            var html = TestData;
            var nexuContext = NexuContext.Current;

            nexuContext.MacroMediaAttributeNames = "imageCrop".ToLower();
            nexuContext.MacroDocumentAttributeNames = "timeline,message".ToLower();

            _mediaService.Setup(x => x.GetById(Media1Id)).Returns(_media1Mock.Object);
            _mediaService.Setup(x => x.GetById(Media2Id)).Returns(_media2Mock.Object);

            _contentServiceMock.Setup(x => x.GetById(Document1Id)).Returns(_document1Mock.Object);
            _contentServiceMock.Setup(x => x.GetById(Document2Id)).Returns(_document2Mock.Object);

            var parser = new RichTextEditorMacrosParser(
                _contentServiceMock.Object,
                _mediaService.Object,
                _cacheProviderMock.Object);

            // act
            var result = parser.GetLinkedEntities(html);

            // verify
            Assert.IsNotNull(result);

            var entities = result.ToList();
            Assert.AreEqual(4, entities.Count);

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == Media1Id));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == Media2Id));

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == Document1Id));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == Document2Id));
        }

        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesForNonRegisteredMacroAttributes()
        {
            // arrange
            var html = TestData;
            var nexuContext = NexuContext.Current;

            nexuContext.MacroMediaAttributeNames = "";
            nexuContext.MacroDocumentAttributeNames = "";

            var parser = new RichTextEditorMacrosParser(
                _contentServiceMock.Object,
                _mediaService.Object,
                _cacheProviderMock.Object);

            // act
            var result = parser.GetLinkedEntities(html);

            // verify
            Assert.IsNotNull(result);

            var entities = result.ToList();
            Assert.IsEmpty(entities);
        }


        /// <summary>
        /// Test getting linked entities with a empty value
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange
            var html = "";
            var nexuContext = NexuContext.Current;

            nexuContext.MacroMediaAttributeNames = "";
            nexuContext.MacroDocumentAttributeNames = "";

            var parser = new RichTextEditorMacrosParser(
                _contentServiceMock.Object,
                _mediaService.Object,
                _cacheProviderMock.Object);

            // act
            var result = parser.GetLinkedEntities(html);

            // verify
            Assert.IsNotNull(result);

            var entities = result.ToList();
            Assert.IsEmpty(entities);
        }
        #endregion
    }
}