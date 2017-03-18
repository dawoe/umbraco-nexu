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
                yield return new TestCaseData(-1).SetName("TestRebuildJob - Root node");
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
                                  this.contentServiceMock.Object)
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

            if (startnode == -1)
            {
                this.contentServiceMock.Setup(x => x.GetRootContent())
                    .Returns(new List<IContent> { startContent.Object });
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
            if (startnode == -1)
            {
                this.contentServiceMock.Verify(x => x.GetRootContent(), Times.Once);
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
