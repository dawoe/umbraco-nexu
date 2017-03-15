namespace Our.Umbraco.Nexu.Core.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using System.Web.Routing;

    using AutoMapper;

    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Models;
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
                yield return new TestCaseData(true).SetName("TestGetRebuildStatus - Running");
                yield return new TestCaseData(false).SetName("TestGetRebuildStatus - Not running");
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
                                  this.mappingEngineMock.Object)
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

        [Test]
        [Category("Api")]
        [TestCaseSource(nameof(RebuildStatusCases))]
        public void TestGetRebuildStatus(bool running)
        {
            // arrange
            NexuContext.Current.IsProcessing = running;

            // act
            var result = this.controller.GetRebuildStatus();

            // verify
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

            Assert.IsNotNull(result.Content);
            var objectContent = (ObjectContent)result.Content;

            Assert.IsNotNull(objectContent.Value);
            var model = (bool)objectContent.Value;

            Assert.AreEqual(running, model);
        }

        #endregion
    }
}
