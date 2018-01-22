namespace Our.Umbraco.Nexu.Core.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using System.Web.Routing;

    using AutoMapper;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Tests.TestHelpers;
    using global::Umbraco.Web;
    using global::Umbraco.Web.UI.Umbraco.Controls;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;
    using Our.Umbraco.Nexu.Core.WebApi;

    /// <summary>
    /// The nexu api controller tests.
    /// </summary>
    [TestFixture]
    public class NexuApiControllerTests : BaseRoutingTest
    {
        #region Private variables

        /// <summary>
        /// The nexu service mock.
        /// </summary>
        private Mock<INexuService> nexuServiceMock;

        /// <summary>
        /// The mapping engine mock.
        /// </summary>
        private Mock<IMappingEngine> mappingEngineMock;

        /// <summary>
        /// The content service mock.
        /// </summary>
        private Mock<IContentService> contentServiceMock;

        /// <summary>
        /// The media service.
        /// </summary>
        private Mock<IMediaService> mediaServiceMock;

        /// <summary>
        /// The controller.
        /// </summary>
        private NexuApiController controller;

        /// <summary>
        /// Gets or sets the umbraco context.
        /// </summary>
        protected UmbracoContext UmbracoContext { get; set; }

        /// <summary>
        /// Gets or sets the route data.
        /// </summary>
        protected RouteData RouteData { get; set; }

        #endregion

        #region Test cases

        /// <summary>
        /// Gets the rebuild status cases.
        /// </summary>
        public IEnumerable<TestCaseData> RebuildStatusCases
        {
            get
            {
                yield return new TestCaseData(true, "Foo", 123).SetName("TestGetRebuildStatus - Running");
                yield return new TestCaseData(false, string.Empty, 0).SetName("TestGetRebuildStatus - Not running");
            }
        }

        /// <summary>
        /// Gets the rebuild test cases.
        /// </summary>
        public IEnumerable<TestCaseData> RebuildTestCases
        {
            get
            {
                yield return new TestCaseData(Constants.System.Root).SetName("TestRebuildJob - Root node");
                yield return new TestCaseData(123).SetName("TestRebuildJob - Specific start node");
            }
        }

        #endregion

        #region Setup & Teardown

        /// <summary>
        /// Initialize the test
        /// </summary>
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            this.nexuServiceMock = new Mock<INexuService>();
            this.mappingEngineMock = new Mock<IMappingEngine>();
            this.contentServiceMock = new Mock<IContentService>();
            this.mediaServiceMock = new Mock<IMediaService>();

            // Mocked settings are now necessary
            SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());

            // Routing context necessary for published content request
            this.RouteData = new RouteData();
            var routingContext = this.GetRoutingContext(
                "http://localhost",
                -1,
                this.RouteData,
                true,
                UmbracoConfig.For.UmbracoSettings());

            // setup umbraco context
            this.UmbracoContext = routingContext.UmbracoContext;

            this.controller = new NexuApiController(
                                  this.UmbracoContext,
                                  this.nexuServiceMock.Object,
                                  this.mappingEngineMock.Object,
                                  this.contentServiceMock.Object,
                                  this.mediaServiceMock.Object)
            {
                Request = new HttpRequestMessage
                {
                    Properties =
                                                            {
                                                                    {
                                                                        HttpPropertyKeys.HttpConfigurationKey,
                                                                        new HttpConfiguration()
                                                                    }
                                                            }
                }
            };
        }

        /// <summary>
        /// Tear down test
        /// </summary>
        [TearDown]
        public override void TearDown()
        {
            this.controller = null;
            this.nexuServiceMock = null;
            this.mappingEngineMock = null;
            this.contentServiceMock = null;
            base.TearDown();
        }

        #endregion

        #region Tests

        /// <summary>
        /// The test get incoming links.
        /// </summary>
        [Test]
        [Category("Api")]
        public void TestGetIncomingLinks()
        {
            // arrange 
            var contentId = 123;

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(contentId, false))
                .Returns(new List<IRelation>());

            this.mappingEngineMock.Setup(x => x.Map<IEnumerable<RelatedDocument>>(It.IsAny<IEnumerable<IRelation>>()))
                .Returns(new List<RelatedDocument>());

            // act
            var result = this.controller.GetIncomingLinks(contentId);

            // verify
            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(contentId, false), Times.Once);
            this.mappingEngineMock.Verify(
                x => x.Map<IEnumerable<RelatedDocument>>(It.IsAny<IEnumerable<IRelation>>()),
                Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var objectContent = (ObjectContent)result.Content;

            Assert.IsNotNull(objectContent.Value);
            var model = (IEnumerable<RelatedDocument>)objectContent.Value;

            Assert.IsNotNull(model);
        }

        /// <summary>
        /// The check descendants for incoming links should return false when there are no children.
        /// </summary>
        [Test]
        [Category("Api")]
        public void CheckContentDescendantsForIncomingLinksShouldReturnFalseWhenThereAreNoChildren()
        {
            // arrange
            var contentId = 123;

            var children = new List<IContent>();

            this.contentServiceMock.Setup(x => x.GetChildren(contentId)).Returns(children);

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(It.IsAny<int>(), false))
                .Returns(new List<IRelation>());

            // act
            var result = this.controller.CheckContentDescendantsForIncomingLinks(contentId);

            // arrange
            Assert.IsFalse(result);

            this.contentServiceMock.Verify(x => x.GetChildren(contentId), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(It.IsAny<int>(), false), Times.Never);
        }

        /// <summary>
        /// The check descendants for incoming links should return false when they have no incoming links.
        /// </summary>
        [Test]
        [Category("Api")]
        public void CheckContentDescendantsForIncomingLinksShouldReturnFalseWhenTheyHaveNoIncomingLinks()
        {
            // arrange
            var contentId = 123;

            var children = new List<IContent>();

            var child1 = new Mock<IContent>();
            child1.Setup(x => x.Id).Returns(456);

            children.Add(child1.Object);

            var child2 = new Mock<IContent>();
            child2.Setup(x => x.Id).Returns(789);

            children.Add(child2.Object);

            var descendants = new List<IContent>();

            var descendant1 = new Mock<IContent>();
            descendant1.Setup(x => x.Id).Returns(000);

            this.contentServiceMock.Setup(x => x.GetChildren(contentId)).Returns(children);
            this.contentServiceMock.Setup(x => x.GetChildren(child1.Object.Id)).Returns(new List<IContent>());
            this.contentServiceMock.Setup(x => x.GetChildren(child2.Object.Id)).Returns(descendants);

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(It.IsAny<int>(), false))
                .Returns(new List<IRelation>());

            // act
            var result = this.controller.CheckContentDescendantsForIncomingLinks(contentId);

            // arrange
            Assert.IsFalse(result);

            this.contentServiceMock.Verify(x => x.GetChildren(contentId), Times.Once);
            this.contentServiceMock.Verify(x => x.GetChildren(child1.Object.Id), Times.Once);
            this.contentServiceMock.Verify(x => x.GetChildren(child2.Object.Id), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(It.IsAny<int>(), false), Times.Exactly(children.Count + descendants.Count));
        }

        /// <summary>
        /// The check descendants for incoming links should return true when one has incoming links.
        /// </summary>
        [Test]
        [Category("Api")]
        public void CheckContentDescendantsForIncomingLinksShouldReturnTrueWhenIncomingLinksForFirstLevel()
        {
            // arrange
            var contentId = 123;

            var children = new List<IContent>();

            var child1 = new Mock<IContent>();
            child1.Setup(x => x.Id).Returns(456);

            children.Add(child1.Object);

            var child2 = new Mock<IContent>();
            child2.Setup(x => x.Id).Returns(000);

            children.Add(child2.Object);

            var child3 = new Mock<IContent>();
            child3.Setup(x => x.Id).Returns(789);

            children.Add(child3.Object);

            this.contentServiceMock.Setup(x => x.GetChildren(contentId)).Returns(children);

            var relations = new List<IRelation>();

            relations.Add(Mock.Of<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(456, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(000, false))
                .Returns(relations);

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(789, false))
                .Returns(new List<IRelation>());

            // act
            var result = this.controller.CheckContentDescendantsForIncomingLinks(contentId);

            // arrange
            Assert.IsTrue(result);

            this.contentServiceMock.Verify(x => x.GetChildren(contentId), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(456, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(000, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(789, false), Times.Never);
        }

        /// <summary>
        /// The check content descendants for incoming links should return true when incoming links for deeper level.
        /// </summary>
        [Test]
        [Category("Api")]
        public void CheckContentDescendantsForIncomingLinksShouldReturnTrueWhenIncomingLinksForDeeperLevel()
        {
            // arrange
            var contentId = 123;

            var children = new List<IContent>();

            var child1 = new Mock<IContent>();
            child1.Setup(x => x.Id).Returns(456);

            children.Add(child1.Object);

            var child2 = new Mock<IContent>();
            child2.Setup(x => x.Id).Returns(000);

            children.Add(child2.Object);

            var child3 = new Mock<IContent>();
            child3.Setup(x => x.Id).Returns(789);

            children.Add(child3.Object);

            var descendants_1 = new List<IContent>();

            var descendant1 = new Mock<IContent>();
            descendant1.Setup(x => x.Id).Returns(006);

            descendants_1.Add(descendant1.Object);

            var descendants_2 = new List<IContent>();

            var descendant2 = new Mock<IContent>();
            descendant2.Setup(x => x.Id).Returns(007);

            descendants_2.Add(descendant2.Object);

            this.contentServiceMock.Setup(x => x.GetChildren(contentId)).Returns(children);
            this.contentServiceMock.Setup(x => x.GetChildren(child1.Object.Id)).Returns(descendants_1);
            this.contentServiceMock.Setup(x => x.GetChildren(child2.Object.Id)).Returns(descendants_2);
            this.contentServiceMock.Setup(x => x.GetChildren(child3.Object.Id)).Returns(new List<IContent>());

            var relations = new List<IRelation>();

            relations.Add(Mock.Of<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(456, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(000, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(789, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(006, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(007, false))
                .Returns(relations);

            // act
            var result = this.controller.CheckContentDescendantsForIncomingLinks(contentId);

            // arrange
            Assert.IsTrue(result);

            this.contentServiceMock.Verify(x => x.GetChildren(contentId), Times.Once);
            this.contentServiceMock.Verify(x => x.GetChildren(child1.Object.Id), Times.Once);
            this.contentServiceMock.Verify(x => x.GetChildren(child2.Object.Id), Times.Once);
            this.contentServiceMock.Verify(x => x.GetChildren(child3.Object.Id), Times.Never);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(456, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(000, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(789, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(006, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(007, false), Times.Once);
        }

        /// <summary>
        /// The check media descendants for incoming links should return false when there are no children.
        /// </summary>
        [Test]
        [Category("Api")]
        public void CheckMediaDescendantsForIncomingLinksShouldReturnFalseWhenThereAreNoChildren()
        {
            // arrange
            var mediaId = 123;

            var children = new List<IMedia>();

            this.mediaServiceMock.Setup(x => x.GetChildren(mediaId)).Returns(children);

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(It.IsAny<int>(), false))
                .Returns(new List<IRelation>());

            // act
            var result = this.controller.CheckMediaDescendantsForIncomingLinks(mediaId);

            // arrange
            Assert.IsFalse(result);

            this.mediaServiceMock.Verify(x => x.GetChildren(mediaId), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(It.IsAny<int>(), false), Times.Never);
        }

        /// <summary>
        /// The check media descendants for incoming links should return false when they have no incoming links.
        /// </summary>
        [Test]
        [Category("Api")]
        public void CheckMediaDescendantsForIncomingLinksShouldReturnFalseWhenTheyHaveNoIncomingLinks()
        {
            // arrange
            var mediaId = 123;

            var children = new List<IMedia>();

            var child1 = new Mock<IMedia>();
            child1.Setup(x => x.Id).Returns(456);

            children.Add(child1.Object);

            var child2 = new Mock<IMedia>();
            child2.Setup(x => x.Id).Returns(789);

            children.Add(child2.Object);

            var descendants = new List<IMedia>();

            var descendant1 = new Mock<IMedia>();
            descendant1.Setup(x => x.Id).Returns(000);

            this.mediaServiceMock.Setup(x => x.GetChildren(mediaId)).Returns(children);
            this.mediaServiceMock.Setup(x => x.GetChildren(child1.Object.Id)).Returns(new List<IMedia>());
            this.mediaServiceMock.Setup(x => x.GetChildren(child2.Object.Id)).Returns(descendants);

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(It.IsAny<int>(), false))
                .Returns(new List<IRelation>());

            // act
            var result = this.controller.CheckMediaDescendantsForIncomingLinks(mediaId);

            // arrange
            Assert.IsFalse(result);

            this.mediaServiceMock.Verify(x => x.GetChildren(mediaId), Times.Once);
            this.mediaServiceMock.Verify(x => x.GetChildren(child1.Object.Id), Times.Once);
            this.mediaServiceMock.Verify(x => x.GetChildren(child2.Object.Id), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(It.IsAny<int>(), false), Times.Exactly(children.Count + descendants.Count));
        }

        /// <summary>
        /// The check media descendants for incoming links should return true when one has incoming links.
        /// </summary>
        [Test]
        [Category("Api")]
        public void CheckMediaDescendantsForIncomingLinksShouldReturnTrueWhenOneHasIncomingLinksForFirstLevel()
        {
            // arrange
            var mediaId = 123;

            var children = new List<IMedia>();

            var child1 = new Mock<IMedia>();
            child1.Setup(x => x.Id).Returns(456);

            children.Add(child1.Object);

            var child2 = new Mock<IMedia>();
            child2.Setup(x => x.Id).Returns(000);

            children.Add(child2.Object);

            var child3 = new Mock<IMedia>();
            child3.Setup(x => x.Id).Returns(789);

            children.Add(child3.Object);

            this.mediaServiceMock.Setup(x => x.GetChildren(mediaId)).Returns(children);

            var relations = new List<IRelation>();

            relations.Add(Mock.Of<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(456, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(000, false))
                .Returns(relations);

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(789, false))
                .Returns(new List<IRelation>());

            // act
            var result = this.controller.CheckMediaDescendantsForIncomingLinks(mediaId);

            // arrange
            Assert.IsTrue(result);

            this.mediaServiceMock.Verify(x => x.GetChildren(mediaId), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(456, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(000, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(789, false), Times.Never);
        }

        /// <summary>
        /// The check media descendants for incoming links should return true when incoming links for deeper level.
        /// </summary>
        [Test]
        [Category("Api")]
        public void CheckMediaDescendantsForIncomingLinksShouldReturnTrueWhenIncomingLinksForDeeperLevel()
        {
            // arrange
            var contentId = 123;

            var children = new List<IMedia>();

            var child1 = new Mock<IMedia>();
            child1.Setup(x => x.Id).Returns(456);

            children.Add(child1.Object);

            var child2 = new Mock<IMedia>();
            child2.Setup(x => x.Id).Returns(000);

            children.Add(child2.Object);

            var child3 = new Mock<IMedia>();
            child3.Setup(x => x.Id).Returns(789);

            children.Add(child3.Object);

            var descendants_1 = new List<IMedia>();

            var descendant1 = new Mock<IMedia>();
            descendant1.Setup(x => x.Id).Returns(006);

            descendants_1.Add(descendant1.Object);

            var descendants_2 = new List<IMedia>();

            var descendant2 = new Mock<IMedia>();
            descendant2.Setup(x => x.Id).Returns(007);

            descendants_2.Add(descendant2.Object);

            this.mediaServiceMock.Setup(x => x.GetChildren(contentId)).Returns(children);
            this.mediaServiceMock.Setup(x => x.GetChildren(child1.Object.Id)).Returns(descendants_1);
            this.mediaServiceMock.Setup(x => x.GetChildren(child2.Object.Id)).Returns(descendants_2);
            this.mediaServiceMock.Setup(x => x.GetChildren(child3.Object.Id)).Returns(new List<IMedia>());

            var relations = new List<IRelation>();

            relations.Add(Mock.Of<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(456, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(000, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(789, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(006, false))
                .Returns(new List<IRelation>());

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(007, false))
                .Returns(relations);

            // act
            var result = this.controller.CheckMediaDescendantsForIncomingLinks(contentId);

            // arrange
            Assert.IsTrue(result);

            this.mediaServiceMock.Verify(x => x.GetChildren(contentId), Times.Once);
            this.mediaServiceMock.Verify(x => x.GetChildren(child1.Object.Id), Times.Once);
            this.mediaServiceMock.Verify(x => x.GetChildren(child2.Object.Id), Times.Once);
            this.mediaServiceMock.Verify(x => x.GetChildren(child3.Object.Id), Times.Never);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(456, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(000, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(789, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(006, false), Times.Once);

            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(007, false), Times.Once);
        }


        /// <summary>
        /// Test getting rebuild status.
        /// </summary>
        /// <param name="running">
        /// The running.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="processed">
        /// The processed.
        /// </param>
        [Test]
        [Category("Api")]
        [TestCaseSource(nameof(RebuildStatusCases))]
        public void TestGetRebuildStatus(bool running, string item, int processed)
        {
            // arrange
            NexuContext.Current.IsProcessing = running;
            NexuContext.Current.ItemInProgress = item;
            NexuContext.Current.ItemsProcessed = processed;

            // act
            var result = this.controller.GetRebuildStatus();

            // verify
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var objectContent = (ObjectContent)result.Content;

            Assert.IsNotNull(objectContent.Value);
            var model = (RebuildStatus)objectContent.Value;

            Assert.AreEqual(running, model.IsProcessing);
            Assert.AreEqual(item, model.ItemName);
            Assert.AreEqual(processed, model.ItemsProcessed);
        }

        [Test]
        [Category("Api")]
        [TestCaseSource(nameof(RebuildTestCases))]
        public void TestRebuildJob(int startnode)
        {
            // arrange
            var startContent = new Mock<IContent>();
            startContent.SetupGet(x => x.Name).Returns("Start content");
            startContent.SetupGet(x => x.Id).Returns(1234);

            var recycleBinContent = new Mock<IContent>();
            recycleBinContent.SetupGet(x => x.Name).Returns("Recycled 1");
            recycleBinContent.SetupGet(x => x.Id).Returns(654);

            if (startnode == Constants.System.Root)
            {
                this.contentServiceMock.Setup(x => x.GetRootContent())
                    .Returns(new List<IContent> { startContent.Object });
                this.contentServiceMock.Setup(x => x.GetChildren(Constants.System.RecycleBinContent)).Returns(new List<IContent>
                                                                                                                  {
                                                                                                                      recycleBinContent.Object
                                                                                                                  });
            }
            else
            {
                this.contentServiceMock.Setup(x => x.GetById(startnode)).Returns(startContent.Object);
            }

            var child1 = new Mock<IContent>();
            child1.SetupGet(x => x.Name).Returns("Child 1");
            child1.SetupGet(x => x.Id).Returns(456);

            var child2 = new Mock<IContent>();
            child2.SetupGet(x => x.Name).Returns("Child 2");
            child2.SetupGet(x => x.Id).Returns(789);

            var childList = new List<IContent>()
                                {
                                   child1.Object,
                                   child2.Object
                                };

            this.contentServiceMock.Setup(x => x.GetChildren(startContent.Object.Id)).Returns(childList);

            this.contentServiceMock.Setup(x => x.GetChildren(child1.Object.Id)).Returns(new List<IContent>());

            this.contentServiceMock.Setup(x => x.GetChildren(child2.Object.Id)).Returns(new List<IContent>());

            this.nexuServiceMock.Setup(x => x.ParseContent(It.IsAny<IContent>()));

            // act
            this.controller.RebuildJob(startnode);

            // verify
            if (startnode == Constants.System.Root)
            {
                this.contentServiceMock.Verify(x => x.GetRootContent(), Times.Once);
                this.contentServiceMock.Verify(x => x.GetChildren(Constants.System.RecycleBinContent), Times.Once);

                this.nexuServiceMock.Verify(x => x.ParseContent(recycleBinContent.Object), Times.Once);
                this.contentServiceMock.Verify(x => x.GetChildren(recycleBinContent.Object.Id), Times.Once);
            }
            else
            {
                this.contentServiceMock.Verify(x => x.GetById(startnode), Times.Once);
            }

            this.nexuServiceMock.Verify(x => x.ParseContent(startContent.Object), Times.Once);

            this.contentServiceMock.Verify(x => x.GetChildren(startContent.Object.Id), Times.Once);

            this.nexuServiceMock.Verify(x => x.ParseContent(child1.Object), Times.Once);
            this.contentServiceMock.Verify(x => x.GetChildren(child1.Object.Id), Times.Once);

            this.nexuServiceMock.Verify(x => x.ParseContent(child2.Object), Times.Once);
            this.contentServiceMock.Verify(x => x.GetChildren(child2.Object.Id), Times.Once);            
        }

        #endregion
    }
}
