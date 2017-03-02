namespace Our.Umbraco.Nexu.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Persistence.Migrations.Upgrades.TargetVersionSevenThreeZero;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Mapping.Profiles;
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
        /// Test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.contentServiceMock = new Mock<IContentService>();

            Mapper.Initialize(config =>
            {
                config.ConstructServicesUsing(this.ResolveType);
                config.AddProfile<NexuMappingProfile>();
            });
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
        /// Resolves types
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ResolveType(Type type)
        {
            if (type == typeof(RelationsToRelatedDocumentsConverter))
            {
                return new RelationsToRelatedDocumentsConverter(this.contentServiceMock.Object);
            }
            
            Assert.Fail("Can not resolve type " + type.AssemblyQualifiedName);
            return null;
        }

        /// <summary>
        /// Test mapping of relations to related documents.
        /// </summary>
        [Test]
        [Category("Mappings")]
        public void TestMapRelationsToRelatedDocuments()
        {
            // arrange
            var input = new List<Relation>
                            {
                                new Relation(123, 1, Mock.Of<IRelationType>()),
                                new Relation(456, 1, Mock.Of<IRelationType>()),
                                new Relation(789, 1, Mock.Of<IRelationType>())
                            };

            IEnumerable<int> actualContentIds = new List<int>();

            var contentType = new Mock<IContentType>();
            contentType.SetupGet(x => x.Icon).Returns("page");

            var contentItems = new List<IContent>();

            var content123 = new Mock<IContent>();
            content123.SetupGet(x => x.Id).Returns(123);
            content123.SetupGet(x => x.Name).Returns("Content 123");
            content123.SetupGet(x => x.Published).Returns(true);
            content123.SetupGet(x => x.Trashed).Returns(false);
            content123.SetupGet(x => x.ContentType).Returns(contentType.Object);

            var content456 = new Mock<IContent>();
            content456.SetupGet(x => x.Id).Returns(456);
            content456.SetupGet(x => x.Name).Returns("Content 456");
            content456.SetupGet(x => x.Published).Returns(false);
            content456.SetupGet(x => x.Trashed).Returns(false);
            content456.SetupGet(x => x.ContentType).Returns(contentType.Object);

            var content789 = new Mock<IContent>();
            content789.SetupGet(x => x.Id).Returns(789);
            content789.SetupGet(x => x.Name).Returns("Content 789");
            content789.SetupGet(x => x.Published).Returns(false);
            content789.SetupGet(x => x.Trashed).Returns(true);
            content789.SetupGet(x => x.ContentType).Returns(contentType.Object);

            contentItems.Add(content123.Object);
            contentItems.Add(content456.Object);
            contentItems.Add(content789.Object);

            this.contentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<int>>()))
                .Callback(
                    (IEnumerable<int> ids) =>
                        {
                            actualContentIds = ids;
                        }).Returns(contentItems);

            // act
            var destination = Mapper.Map<IEnumerable<RelatedDocument>>(input);

            // verify
            this.contentServiceMock.Verify(x => x.GetByIds(It.IsAny<IEnumerable<int>>()), Times.Once);

            Assert.AreEqual(input.Select(x => x.Id), actualContentIds);

            Assert.IsNotNull(destination);
            Assert.AreEqual(input.Count, destination.Count());
        }

        /// <summary>
        /// The test mapping from content to related document.
        /// </summary>
        [Test]
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
