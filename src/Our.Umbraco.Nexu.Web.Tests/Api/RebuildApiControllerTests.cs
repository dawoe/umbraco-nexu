namespace Our.Umbraco.Nexu.Web.Tests.Api
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;

    using global::Umbraco.Core.Composing;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common;
    using Our.Umbraco.Nexu.Common.Interfaces.Services;
    using Our.Umbraco.Nexu.Web.Api;
    using Our.Umbraco.Nexu.Web.Models;

    [TestFixture]
    public class RebuildApiControllerTests
    {
        private  Mock<IEntityParsingService> entityParsingServiceMock;

        private  Mock<IContentService> contentServiceMock;

        private  RebuildApiController controller;

        public static IEnumerable<TestCaseData> RebuildStatusCases
        {
            get
            {
                yield return new TestCaseData(true, "Foo", 123).SetName("TestGetRebuildStatus - Running");
                yield return new TestCaseData(false, string.Empty, 0).SetName("TestGetRebuildStatus - Not running");
            }
        }

        [SetUp]
        public void SetUp()
        {
            Current.Factory = new Mock<IFactory>().Object;
            this.entityParsingServiceMock = new Mock<IEntityParsingService>();
            this.contentServiceMock = new Mock<IContentService>();

            this.controller = new RebuildApiController(this.entityParsingServiceMock.Object, this.contentServiceMock.Object)
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

        [TearDown]
        public void TearDown()
        {
            Current.Reset();
        }

        [Test]
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
    }
}
