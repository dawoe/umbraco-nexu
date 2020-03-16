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
    ///  Represents the tests for GetRelatedEntitiesFromProperty method on the NexuEntityParsingService
    /// </summary>    
    [TestFixture]
    public class GetRelatedEntitiesFromProperty_Tests : NexuEntityParsingServiceBaseTest
    {
        [Test]
        public void GetRelatedEntitiesFromProperty_Should_Parse_All_Cultures()
        {
            // arrange
            var editorAlias = "editorAlias";

            var cultureValues = new Dictionary<string, object>();

            var nlValue = "umb://document/ca4249ed2b234337b52263cabe5587d1";

            var enValue = "umb://document/ec4aafcc0c254f25a8fe705bfae1d324";

            cultureValues.Add("nl-NL", nlValue);
            cultureValues.Add("en-US", enValue);

            var propertyType =
                new PropertyType(editorAlias, ValueStorageType.Ntext)
                    {
                        Variations = ContentVariation.Culture
                    };

            var property = new Property(propertyType);

            foreach (var key in cultureValues.Keys)
            {
                property.SetValue(cultureValues[key], key);
            }

            var nlRelation = new RelatedDocumentEntity
                                 {
                                     RelatedEntityUdi = new StringUdi(new Uri(nlValue))
                                 };

            var enRelation = new RelatedDocumentEntity
                                 {
                                     RelatedEntityUdi = new StringUdi(new Uri(enValue))
                                 };

            Mock.Get(this.Service).Setup(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, nlValue)).Returns(new List<IRelatedEntity> { nlRelation });
            Mock.Get(this.Service).Setup(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, enValue)).Returns(new List<IRelatedEntity> { enRelation});

            // act
            var result = this.Service.GetRelatedEntitiesFromProperty(property);

            // assert
            Assert.IsNotNull(result);

            Assert.That(result.Keys.Count == 2);

            Mock.Get(this.Service).Verify(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, nlValue), Times.Once);
            Mock.Get(this.Service).Verify(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, enValue), Times.Once);
          
        }

        [Test]
        public void GetRelatedEntitiesFromProperty_For_Invariant_Property_Should_Parse_As_Invariant_Culture()
        {
            // arrange
            var editorAlias = "editorAlias";

          var nlValue = "umb://document/ca4249ed2b234337b52263cabe5587d1";

               

            var propertyType =
                new PropertyType(editorAlias, ValueStorageType.Ntext)
                {
                    Variations = ContentVariation.Nothing
                };

            var property = new Property(propertyType);

            property.SetValue(nlValue);             
           

            var nlRelation = new RelatedDocumentEntity
            {
                RelatedEntityUdi = new StringUdi(new Uri(nlValue))
            };
         
            Mock.Get(this.Service).Setup(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, nlValue)).Returns(new List<IRelatedEntity> { nlRelation });          

            // act
            var result = this.Service.GetRelatedEntitiesFromProperty(property);

            // assert
            Assert.IsNotNull(result);

            Assert.That(result.Keys.Count == 1);

            Assert.That(result.Keys.First() == "invariant");

            Mock.Get(this.Service).Verify(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, nlValue), Times.Once);            

        }
    }
}
