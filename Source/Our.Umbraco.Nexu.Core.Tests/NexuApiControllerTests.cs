namespace Our.Umbraco.Nexu.Core.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using System.Web.Routing;

    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Tests.TestHelpers;
    using global::Umbraco.Web;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.WebApi;

    [TestFixture]
    public class NexuApiControllerTests : BaseRoutingTest
    {
        private Mock<INexuService> nexuServiceMock;

        private NexuApiController controller;

        /// <summary>
        /// Gets or sets the umbraco context.
        /// </summary>
        protected UmbracoContext UmbracoContext { get; set; }
        
        /// <summary>
        /// Gets or sets the route data.
        /// </summary>
        protected RouteData RouteData { get; set; }


        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            this.nexuServiceMock = new Mock<INexuService>();

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

            this.controller = new NexuApiController(this.nexuServiceMock.Object, this.UmbracoContext);
        }

        [TearDown]
        public override void TearDown()
        {
            this.controller = null;
            this.nexuServiceMock = null;
            base.TearDown();
        }

        [Test]
        [Category("Api")]
        public void TestGetIncomingLinks()
        {
            // arrange 
            var contentId = 123;

            this.nexuServiceMock.Setup(x => x.GetNexuRelationsForContent(contentId, false))
                .Returns(new List<IRelation>());

            // act
            var result = this.controller.GetIncomingLinks(contentId);

            // verify
            this.nexuServiceMock.Verify(x => x.GetNexuRelationsForContent(contentId, false), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

        }
    }
}
