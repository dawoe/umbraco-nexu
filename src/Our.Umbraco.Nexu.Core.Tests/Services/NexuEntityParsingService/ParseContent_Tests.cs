namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Core.Composing.Collections;
    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    ///  Represents the tests for ParseContent method on the NexuEntityParsingService
    /// </summary>   
    [TestFixture]
    public class ParseContent_Tests
    {
        /// <summary>
        /// The service instance used in all tests
        /// </summary>
        private NexuEntityParsingService service;

        /// <summary>
        /// The setup that is run for all tests
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var serviceMock = new Mock<NexuEntityParsingService>(
                                  new PropertyValueParserCollection(new List<IPropertyValueParser>()))
                                  {
                                      CallBase = true
                                  };


            this.service = serviceMock.Object;
        }

        [Test]
        public void When_ParseContent_Is_Called_Relations_Should_Be_Retreived_For_Content_Item_When_Content_Is_Not_Blue_Print()
        {
            // arrange
            var content = Mock.Of<IContent>();
            content.Blueprint = false;

            Mock.Get(this.service).Setup(x => x.GetRelatedEntitiesFromContent(content));

            // act
            this.service.ParseContent(content);

            // assert
            Mock.Get(this.service).Verify(x => x.GetRelatedEntitiesFromContent(content), Times.Once);
        }

        [Test]
        public void When_ParseContent_Is_Called_Relations_Should_Not_Be_Retreived_For_Content_Item_When_Content_Is_Blue_Print()
        {
            // arrange
            var content = Mock.Of<IContent>();
            content.Blueprint = true;

            Mock.Get(this.service).Setup(x => x.GetRelatedEntitiesFromContent(content));

            // act
            this.service.ParseContent(content);

            // assert
            Mock.Get(this.service).Verify(x => x.GetRelatedEntitiesFromContent(content), Times.Never);
        }
    }
}
