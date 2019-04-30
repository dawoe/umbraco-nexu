namespace Our.Umbraco.Nexu.Core.Tests.Services
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Core.Composing.Collections;
    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    ///  Represents the tests for GetRelatedEntitiesFromProperty method on the NexuEntityParsingService
    /// </summary>    
    [TestFixture]
    public class NexuEntityParsingService_GetRelatedEntitiesFromProperty_Tests
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

            Mock.Get(this.service).Setup(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, nlValue)).Returns(new List<IRelatedEntity>());
            Mock.Get(this.service).Setup(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, enValue)).Returns(new List<IRelatedEntity>());

            // act
            var result = this.service.GetRelatedEntitiesFromProperty(property);

            // assert
            Assert.IsNotNull(result);

            Assert.That(result.Keys.Count == 2);

            Mock.Get(this.service).Verify(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, nlValue), Times.Once);
            Mock.Get(this.service).Verify(x => x.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, enValue), Times.Once);
          
        }
    }
}
