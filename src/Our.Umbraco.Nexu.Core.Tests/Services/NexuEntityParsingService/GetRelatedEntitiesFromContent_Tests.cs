namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    ///  Represents the tests for  GetRelatedEntitiesFromContent method on the NexuEntityParsingService
    /// </summary>    
    [TestFixture]
    public class GetRelatedEntitiesFromContent_Tests : NexuEntityParsingServiceBaseTest
    {     
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

            Mock.Get(this.Service).Setup(x => x.GetRelatedEntitiesFromProperty(property1)).Returns(prop1Entities);
            Mock.Get(this.Service).Setup(x => x.GetRelatedEntitiesFromProperty(property2)).Returns(prop2Entities);

            // act
            var result = this.Service.GetRelatedEntitiesFromContent(content).ToList();

            // assert
            Assert.IsNotNull(result);

            Assert.That(result.Count == 3);

            Mock.Get(this.Service).Verify(x => x.GetRelatedEntitiesFromProperty(property1), Times.Once);
            Mock.Get(this.Service).Verify(x => x.GetRelatedEntitiesFromProperty(property2), Times.Once);
        }
    }
}
