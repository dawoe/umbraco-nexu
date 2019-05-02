namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System;

    using global::Umbraco.Core.Models;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    ///  Represents the tests for ParseContent method on the NexuEntityParsingService
    /// </summary>   
    [TestFixture]
    public class ParseContent_Tests : NexuEntityParsingServiceBaseTest
    {
        [Test]
        public void When_ParseContent_Is_Called_Relations_Should_Be_Retreived_For_Content_Item_When_Content_Is_Not_Blue_Print()
        {
            // arrange
            var content = Mock.Of<IContent>();
            content.Blueprint = false;

            Mock.Get(this.Service).Setup(x => x.GetRelatedEntitiesFromContent(content));

            // act
            this.Service.ParseContent(content);

            // assert
            Mock.Get(this.Service).Verify(x => x.GetRelatedEntitiesFromContent(content), Times.Once);
        }

        [Test]
        public void When_ParseContent_Is_Called_And_GetRelatedEntitiesFromContent_Throws_An_Exception_This_Should_Be_Logged()
        {
            // arrange
            var content = Mock.Of<IContent>();
            content.Id = 1234;
            content.Blueprint = false;

            var exceptionMessage = $"Something went wrong parsing content with id {content.Id}";
            var exception = new Exception(
                exceptionMessage,
                new NullReferenceException());

            Mock.Get(this.Service).Setup(x => x.GetRelatedEntitiesFromContent(content)).Throws(exception);

            this.LoggerMock.Setup(x => x.Error(typeof(NexuEntityParsingService), exceptionMessage, exception));

            // act
            this.Service.ParseContent(content);

            // assert
            Mock.Get(this.Service).Verify(x => x.GetRelatedEntitiesFromContent(content), Times.Once);
            this.LoggerMock.Verify(x => x.Error(typeof(NexuEntityParsingService), exceptionMessage, exception), Times.Once);
        }

        [Test]
        public void When_ParseContent_Is_Called_Relations_Should_Not_Be_Retreived_For_Content_Item_When_Content_Is_Blue_Print()
        {
            // arrange
            var content = Mock.Of<IContent>();
            content.Blueprint = true;

            Mock.Get(this.Service).Setup(x => x.GetRelatedEntitiesFromContent(content));

            // act
            this.Service.ParseContent(content);

            // assert
            Mock.Get(this.Service).Verify(x => x.GetRelatedEntitiesFromContent(content), Times.Never);
        }
    }
}
