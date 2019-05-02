namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    /// <summary>
    ///  Represents the tests for GetRelatedEntitiesFromPropertyEditorValue method on the NexuEntityParsingService
    /// </summary>    
    [TestFixture]
    public class GetRelatedEntitiesFromPropertyEditorValue_Tests : NexuEntityParsingServiceBaseTest
    {
        [Test]
        public void When_No_Parser_Found_GetRelatedEntitiesFromPropertyEditorValue_Should_Return_Empty_List()
        {
            // arrange
            var editorAlias = "Bla";

            Mock.Get(this.Service).Setup(x => x.GetParserForPropertyEditor(editorAlias));

            // act
            var result = this.Service.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, "bla");

            // assert
            Assert.IsNotNull(result);
            Assert.That(!result.Any());

            Mock.Get(this.Service).Verify(x => x.GetParserForPropertyEditor(editorAlias), Times.Once);
        }

        [Test]
        public void When_PropertyValue_Is_Empty_GetRelatedEntitiesFromPropertyEditorValuee_Should_Return_Empty_List()
        {
            // arrange
            var editorAlias = "Bla";
            object propertyValue = null;

            Mock.Get(this.Service).Setup(x => x.GetParserForPropertyEditor(editorAlias));

            // act
            var result = this.Service.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, propertyValue);

            // assert
            Assert.IsNotNull(result);
            Assert.That(!result.Any());

            Mock.Get(this.Service).Verify(x => x.GetParserForPropertyEditor(editorAlias), Times.Never);            
        }

        [Test]
        public void When_Parser_Found_GetRelatedEntitiesFromPropertyEditorValue_Should_Call_GetRelatedEntities_Method_From_Parser()
        {
            // arrange
            var editorAlias = "Bla";
            var propertyValue = "Foo";

            var parser = new Mock<IPropertyValueParser>();
            parser.Setup(x => x.GetRelatedEntities(propertyValue)).Returns(new List<IRelatedEntity>());

            Mock.Get(this.Service).Setup(x => x.GetParserForPropertyEditor(editorAlias)).Returns(parser.Object);

            // act
            var result = this.Service.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, propertyValue);

            // assert
            Assert.IsNotNull(result);
            Assert.That(!result.Any());

            Mock.Get(this.Service).Verify(x => x.GetParserForPropertyEditor(editorAlias), Times.Once);
            parser.Verify(x => x.GetRelatedEntities(propertyValue), Times.Once);
        }

        [Test]
        public void When_Parser_Found_GetRelatedEntitiesFromPropertyEditorValue_Should_Return_Unique_Relations_If_Duplicates_Are_found()
        {
            // arrange
            var editorAlias = "Bla";
            var propertyValue = "Foo";

            var relatedEntities = new List<IRelatedEntity>();

            var entity = new Mock<IRelatedEntity>();
            entity.SetupGet(x => x.RelationType).Returns(Common.Constants.RelationTypes.DocumentToDocument);
            entity.SetupGet(x => x.RelatedEntityUdi)
                .Returns(new StringUdi(new Uri("umb://document/ca4249ed2b234337b52263cabe5587d1")));

            relatedEntities.Add(entity.Object);
            relatedEntities.Add(entity.Object);

            var parser = new Mock<IPropertyValueParser>();
          
            parser.Setup(x => x.GetRelatedEntities(propertyValue)).Returns(relatedEntities);

            Mock.Get(this.Service).Setup(x => x.GetParserForPropertyEditor(editorAlias)).Returns(parser.Object);

            // act
            var result = this.Service.GetRelatedEntitiesFromPropertyEditorValue(editorAlias, propertyValue);

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count() == 1);

            Mock.Get(this.Service).Verify(x => x.GetParserForPropertyEditor(editorAlias), Times.Once);
            parser.Verify(x => x.GetRelatedEntities(propertyValue), Times.Once);
        }
    }
}
