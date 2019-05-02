namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Core.Composing.Collections;
    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    ///  Represents the tests for  GetRelatedEntitiesFromContent method on the NexuEntityParsingService
    /// </summary>    
    [TestFixture]
    public class GetRelatedEntitiesFromContent_Tests
    {
        /// <summary>
        /// The service instance used in all tests
        /// </summary>
        private NexuEntityParsingService service;

        /// <summary>
        /// The logger mock.
        /// </summary>
        private Mock<ILogger> loggerMock;

        /// <summary>
        /// The setup that is run for all tests
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.loggerMock = new Mock<ILogger>();

            var serviceMock = new Mock<NexuEntityParsingService>(
                                  new PropertyValueParserCollection(new List<IPropertyValueParser>()), this.loggerMock.Object)
                                  {
                                      CallBase = true
                                  };


            this.service = serviceMock.Object;
        }

        [Test]
        public void When_GetRelatedEntitiesFromContent_Is_Called_It_Should_Get_RelatedEntities_For_Each_Property()
        {
            // arrange
            var contentKey = Guid.NewGuid();

            var content = Mock.Of<IContent>();
            content.Blueprint = false;
            content.Key = contentKey;

            var editorAlias = "alias";

            var propertyType =
                new PropertyType(editorAlias, ValueStorageType.Ntext)
                    {
                        Variations = ContentVariation.Culture
                    };

            var property1 = new Property(propertyType);
            property1.Id = 123;

            var property2 = new Property(propertyType);
            property2.Id = 456;

            content.Properties = new PropertyCollection(new[] { property1, property2 });

            var entityGuid = Guid.NewGuid();

            var prop1Entities = new Dictionary<string, IEnumerable<IRelatedEntity>>();
            prop1Entities.Add("nl", new List<IRelatedEntity> { new RelatedDocumentEntity { RelatedEntityUdi = Udi.Create("document", entityGuid )}});

            var prop2Entities = new Dictionary<string, IEnumerable<IRelatedEntity>>();
            prop2Entities.Add("nl", new List<IRelatedEntity> { new RelatedDocumentEntity { RelatedEntityUdi = Udi.Create("document", entityGuid) } });
            prop2Entities.Add("en", new List<IRelatedEntity> { new RelatedDocumentEntity { RelatedEntityUdi = Udi.Create("document", entityGuid) } });

            Mock.Get(this.service).Setup(x => x.GetRelatedEntitiesFromProperty(property1)).Returns(prop1Entities);
            Mock.Get(this.service).Setup(x => x.GetRelatedEntitiesFromProperty(property2)).Returns(prop2Entities);

            // act
            var result = this.service.GetRelatedEntitiesFromContent(content).ToList();

            // assert
            Assert.IsNotNull(result);

            Assert.That(result.Count == 3);

            Mock.Get(this.service).Verify(x => x.GetRelatedEntitiesFromProperty(property1), Times.Once);
            Mock.Get(this.service).Verify(x => x.GetRelatedEntitiesFromProperty(property2), Times.Once);
        }
    }
}
