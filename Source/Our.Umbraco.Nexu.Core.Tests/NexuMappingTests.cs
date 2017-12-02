namespace Our.Umbraco.Nexu.Core.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using AutoMapper;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Mapping.TypeConverters;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The nexu mapping tests.
    /// </summary>
    [TestFixture]
    public class NexuMappingTests
    {
        /// <summary>
        /// The content service mock.
        /// </summary>
        private Mock<IContentService> contentServiceMock;

        /// <summary>
        /// The localization service mock.
        /// </summary>
        private Mock<ILocalizationService> localizationServiceMock;

        /// <summary>
        /// Test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.contentServiceMock = new Mock<IContentService>();
            this.localizationServiceMock = new Mock<ILocalizationService>();

            Mapper.CreateMap<IEnumerable<IRelation>, IEnumerable<RelatedDocument>>().ConvertUsing(new RelationsToRelatedDocumentsConverter(this.contentServiceMock.Object, this.localizationServiceMock.Object));

            Mapper.CreateMap<IContent, RelatedDocument>()
                .ForMember(x => x.Properties, opt => opt.Ignore())
                .ForMember(x => x.Icon, opt => opt.MapFrom(src => src.ContentType.Icon));
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.contentServiceMock = null;
            Mapper.Reset();
        }
        
        /// <summary>
        /// Test mapping of relations to related documents.
        /// </summary>
        [Test]
        [Category("Mappings")]
        public void TestMapRelationsToRelatedDocumentssWithDictionaryItemsInPropAndTabnames()
        {
            // arrange
            this.localizationServiceMock.Setup(x => x.DictionaryItemExists(It.IsAny<string>())).Returns(true);
            this.localizationServiceMock.Setup(x => x.GetDictionaryItemByKey(It.IsAny<string>()))
                .Returns(
                    (string s) =>
                        {
                            var language = Mock.Of<ILanguage>();

                            Mock.Get(language).SetupGet(x => x.IsoCode)
                                .Returns(Thread.CurrentThread.CurrentCulture.Name);

                            var translation = Mock.Of<IDictionaryTranslation>();

                            Mock.Get(translation).SetupGet(x => x.Value).Returns(s);
                            Mock.Get(translation).SetupGet(x => x.Language).Returns(language);

                            var dictionary = Mock.Of<IDictionaryItem>();

                            Mock.Get(dictionary).SetupGet(x => x.ItemKey).Returns(s);
                            Mock.Get(dictionary).SetupGet(x => x.Translations)
                                .Returns(new List<IDictionaryTranslation> { translation });
                            return dictionary;
                        });

            var relation123Mock = new Mock<IRelation>();
            relation123Mock.SetupGet(x => x.ParentId).Returns(123);
            relation123Mock.SetupGet(x => x.Comment).Returns("#media [[#Images]] || rte [[Content]]");

            var relation456Mock = new Mock<IRelation>();
            relation456Mock.SetupGet(x => x.ParentId).Returns(456);
            relation456Mock.SetupGet(x => x.Comment).Returns("picker [[Links]] || rte [[Content]]");

            var relation789Mock = new Mock<IRelation>();
            relation789Mock.SetupGet(x => x.ParentId).Returns(789);
            relation789Mock.SetupGet(x => x.Comment).Returns("picker [[Images]] || media [[Images]]");

            var input = new List<IRelation>
                            {
                               relation123Mock.Object,
                               relation456Mock.Object,
                               relation789Mock.Object
                            };

            IEnumerable<int> actualContentIds = new List<int>();

            var contentTypeMock = new Mock<IContentType>();
            contentTypeMock.SetupGet(x => x.Icon).Returns("page");

            var contentItems = new List<IContent>();

            var content123Mock = new Mock<IContent>();
            content123Mock.SetupGet(x => x.Id).Returns(123);
            content123Mock.SetupGet(x => x.Name).Returns("Content 123");
            content123Mock.SetupGet(x => x.Published).Returns(true);
            content123Mock.SetupGet(x => x.Trashed).Returns(false);
            content123Mock.SetupGet(x => x.ContentType).Returns(contentTypeMock.Object);

            var content456Mock = new Mock<IContent>();
            content456Mock.SetupGet(x => x.Id).Returns(456);
            content456Mock.SetupGet(x => x.Name).Returns("Content 456");
            content456Mock.SetupGet(x => x.Published).Returns(false);
            content456Mock.SetupGet(x => x.Trashed).Returns(false);
            content456Mock.SetupGet(x => x.ContentType).Returns(contentTypeMock.Object);

            var content789MOck = new Mock<IContent>();
            content789MOck.SetupGet(x => x.Id).Returns(789);
            content789MOck.SetupGet(x => x.Name).Returns("Content 789");
            content789MOck.SetupGet(x => x.Published).Returns(false);
            content789MOck.SetupGet(x => x.Trashed).Returns(true);
            content789MOck.SetupGet(x => x.ContentType).Returns(contentTypeMock.Object);

            contentItems.Add(content123Mock.Object);
            contentItems.Add(content456Mock.Object);
            contentItems.Add(content789MOck.Object);

            this.contentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<int>>()))
                .Callback(
                    (IEnumerable<int> ids) =>
                        {
                            actualContentIds = ids;
                        }).Returns(contentItems);

            // act
            var destination = Mapper.Map<IEnumerable<RelatedDocument>>(input).ToList();

            // verify
            this.localizationServiceMock.Verify(x => x.GetDictionaryItemByKey(It.IsAny<string>()), Times.Exactly(2));
            this.contentServiceMock.Verify(x => x.GetByIds(It.IsAny<IEnumerable<int>>()), Times.Once);

            Assert.AreEqual(input.Select(x => x.ParentId), actualContentIds);

            Assert.IsNotNull(destination);
            Assert.AreEqual(input.Count, destination.Count());

            // properties
            var related123Props = destination.First(x => x.Id == 123).Properties;

            Assert.IsTrue(related123Props.Keys.Contains("Images"));
            Assert.IsTrue(related123Props.Keys.Contains("Content"));
 
            Assert.IsTrue(related123Props["Images"].Exists(x => x.Trim() == "media"));
            Assert.IsTrue(related123Props["Content"].Exists(x => x.Trim() == "rte"));

            var related456Props = destination.First(x => x.Id == 456).Properties;

            Assert.IsTrue(related456Props.Keys.Contains("Links"));
            Assert.IsTrue(related456Props.Keys.Contains("Content"));

            Assert.IsTrue(related456Props["Links"].Exists(x => x.Trim() == "picker"));
            Assert.IsTrue(related456Props["Content"].Exists(x => x.Trim() == "rte"));

            var related789Props = destination.First(x => x.Id == 789).Properties;

            Assert.IsTrue(related789Props.Keys.Contains("Images"));

            Assert.IsTrue(related789Props["Images"].Exists(x => x.Trim() == "picker"));
            Assert.IsTrue(related789Props["Images"].Exists(x => x.Trim() == "media"));

        }

        [Test]
        [Category("Mappings")]
        public void TestMapRelationsToRelatedDocuments()
        {
            // arrange
            var relation123Mock = new Mock<IRelation>();
            relation123Mock.SetupGet(x => x.ParentId).Returns(123);
            relation123Mock.SetupGet(x => x.Comment).Returns("media [[Images]] || rte [[Content]]");

            var relation456Mock = new Mock<IRelation>();
            relation456Mock.SetupGet(x => x.ParentId).Returns(456);
            relation456Mock.SetupGet(x => x.Comment).Returns("picker [[Links]] || rte [[Content]]");

            var relation789Mock = new Mock<IRelation>();
            relation789Mock.SetupGet(x => x.ParentId).Returns(789);
            relation789Mock.SetupGet(x => x.Comment).Returns("picker [[Images]] || media [[Images]]");

            var input = new List<IRelation>
                            {
                                relation123Mock.Object,
                                relation456Mock.Object,
                                relation789Mock.Object
                            };

            IEnumerable<int> actualContentIds = new List<int>();

            var contentTypeMock = new Mock<IContentType>();
            contentTypeMock.SetupGet(x => x.Icon).Returns("page");

            var contentItems = new List<IContent>();

            var content123Mock = new Mock<IContent>();
            content123Mock.SetupGet(x => x.Id).Returns(123);
            content123Mock.SetupGet(x => x.Name).Returns("Content 123");
            content123Mock.SetupGet(x => x.Published).Returns(true);
            content123Mock.SetupGet(x => x.Trashed).Returns(false);
            content123Mock.SetupGet(x => x.ContentType).Returns(contentTypeMock.Object);

            var content456Mock = new Mock<IContent>();
            content456Mock.SetupGet(x => x.Id).Returns(456);
            content456Mock.SetupGet(x => x.Name).Returns("Content 456");
            content456Mock.SetupGet(x => x.Published).Returns(false);
            content456Mock.SetupGet(x => x.Trashed).Returns(false);
            content456Mock.SetupGet(x => x.ContentType).Returns(contentTypeMock.Object);

            var content789MOck = new Mock<IContent>();
            content789MOck.SetupGet(x => x.Id).Returns(789);
            content789MOck.SetupGet(x => x.Name).Returns("Content 789");
            content789MOck.SetupGet(x => x.Published).Returns(false);
            content789MOck.SetupGet(x => x.Trashed).Returns(true);
            content789MOck.SetupGet(x => x.ContentType).Returns(contentTypeMock.Object);

            contentItems.Add(content123Mock.Object);
            contentItems.Add(content456Mock.Object);
            contentItems.Add(content789MOck.Object);

            this.contentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<int>>()))
                .Callback(
                    (IEnumerable<int> ids) =>
                        {
                            actualContentIds = ids;
                        }).Returns(contentItems);

            // act
            var destination = Mapper.Map<IEnumerable<RelatedDocument>>(input).ToList();

            // verify
            this.contentServiceMock.Verify(x => x.GetByIds(It.IsAny<IEnumerable<int>>()), Times.Once);

            Assert.AreEqual(input.Select(x => x.ParentId), actualContentIds);

            Assert.IsNotNull(destination);
            Assert.AreEqual(input.Count, destination.Count());

            // properties
            var related123Props = destination.First(x => x.Id == 123).Properties;

            Assert.IsTrue(related123Props.Keys.Contains("Images"));
            Assert.IsTrue(related123Props.Keys.Contains("Content"));

            Assert.IsTrue(related123Props["Images"].Exists(x => x.Trim() == "media"));
            Assert.IsTrue(related123Props["Content"].Exists(x => x.Trim() == "rte"));

            var related456Props = destination.First(x => x.Id == 456).Properties;

            Assert.IsTrue(related456Props.Keys.Contains("Links"));
            Assert.IsTrue(related456Props.Keys.Contains("Content"));

            Assert.IsTrue(related456Props["Links"].Exists(x => x.Trim() == "picker"));
            Assert.IsTrue(related456Props["Content"].Exists(x => x.Trim() == "rte"));

            var related789Props = destination.First(x => x.Id == 789).Properties;

            Assert.IsTrue(related789Props.Keys.Contains("Images"));

            Assert.IsTrue(related789Props["Images"].Exists(x => x.Trim() == "picker"));
            Assert.IsTrue(related789Props["Images"].Exists(x => x.Trim() == "media"));

        }

        /// <summary>
        /// The test mapping from content to related document.
        /// </summary>
        [Test]
        [Category("Mappings")]
        public void TestMappingFromContentToRelatedDocument()
        {
            // arrange
            var contentTypeMock = new Mock<IContentType>();
            contentTypeMock.SetupGet(x => x.Icon).Returns("page");

            var contentMock = new Mock<IContent>();
            contentMock.SetupGet(x => x.Id).Returns(123);
            contentMock.SetupGet(x => x.Name).Returns("Content 123");
            contentMock.SetupGet(x => x.Published).Returns(true);
            contentMock.SetupGet(x => x.Trashed).Returns(false);
            contentMock.SetupGet(x => x.ContentType).Returns(contentTypeMock.Object);

            // act
            var result = Mapper.Map<RelatedDocument>(contentMock.Object);

            // verify
            Assert.IsNotNull(result);

            Assert.AreEqual(contentMock.Object.Id, result.Id);
            Assert.AreEqual(contentMock.Object.Name, result.Name);
            Assert.AreEqual(contentMock.Object.Published, result.Published);
            Assert.AreEqual(contentMock.Object.Trashed, result.Trashed);
            Assert.AreEqual(contentTypeMock.Object.Icon, result.Icon);
        }
    }
}
