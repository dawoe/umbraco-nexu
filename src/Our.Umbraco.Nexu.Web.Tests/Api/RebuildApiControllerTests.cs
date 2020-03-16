namespace Our.Umbraco.Nexu.Web.Tests.Api
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Composing;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
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
        private Mock<IEntityParsingService> entityParsingServiceMock;

        private Mock<IContentService> contentServiceMock;

        private RebuildApiController controller;

        private Mock<ILogger> loggerMock;

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
            this.loggerMock = new Mock<ILogger>();

            this.controller = new RebuildApiController(this.entityParsingServiceMock.Object, this.contentServiceMock.Object, this.loggerMock.Object)
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

        [Test]
        public void TestRebuildJob()
        {
            // arrange
            var startContent = new Mock<IContent>();
            startContent.SetupGet(x => x.Name).Returns("Start content");
            startContent.SetupGet(x => x.Id).Returns(1234);

            var recycleBinContent = new Mock<IContent>();
            recycleBinContent.SetupGet(x => x.Name).Returns("Recycled 1");
            recycleBinContent.SetupGet(x => x.Id).Returns(654);


            this.contentServiceMock.Setup(x => x.GetRootContent())
                .Returns(new List<IContent> { startContent.Object });

            long totalRecords = 0;

            this.contentServiceMock.Setup(x => x.GetPagedChildren(Constants.System.RecycleBinContent, 0, int.MaxValue, out totalRecords, null, null)).Returns(new List<IContent>
                                                                                                                  {
                                                                                                                      recycleBinContent.Object
                                                                                                                  });

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

            this.contentServiceMock.Setup(x => x.GetPagedChildren(startContent.Object.Id, 0, int.MaxValue, out totalRecords, null, null)).Returns(childList);

            this.contentServiceMock.Setup(x => x.GetPagedChildren(child1.Object.Id, 0, int.MaxValue, out totalRecords, null, null)).Returns(new List<IContent>());

            this.contentServiceMock.Setup(x => x.GetPagedChildren(child2.Object.Id, 0, int.MaxValue, out totalRecords, null, null)).Returns(new List<IContent>());

            this.entityParsingServiceMock.Setup(x => x.ParseContent(It.IsAny<IContent>()));

            // act
            this.controller.RebuildJob(null);

            // verify
            this.contentServiceMock.Verify(x => x.GetRootContent(), Times.Once);
            this.contentServiceMock.Verify(x => x.GetPagedChildren(Constants.System.RecycleBinContent, 0, int.MaxValue, out totalRecords, null, null), Times.Once);

            this.entityParsingServiceMock.Verify(x => x.ParseContent(recycleBinContent.Object), Times.Once);
            //this.contentServiceMock.Verify(x => x.GetChildren(recycleBinContent.Object.Id), Times.Once);


            this.entityParsingServiceMock.Verify(x => x.ParseContent(startContent.Object), Times.Once);

            this.contentServiceMock.Verify(x => x.GetPagedChildren(startContent.Object.Id, 0, int.MaxValue, out totalRecords, null, null), Times.Once);

            this.entityParsingServiceMock.Verify(x => x.ParseContent(child1.Object), Times.Once);
            this.contentServiceMock.Verify(x => x.GetPagedChildren(child1.Object.Id, 0, int.MaxValue, out totalRecords, null, null), Times.Once);

            this.entityParsingServiceMock.Verify(x => x.ParseContent(child2.Object), Times.Once);
            this.contentServiceMock.Verify(x => x.GetPagedChildren(child2.Object.Id, 0, int.MaxValue, out totalRecords, null, null), Times.Once);
        }
    }
}
