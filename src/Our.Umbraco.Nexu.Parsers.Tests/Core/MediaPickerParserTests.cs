namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using System.Linq;

    using global::Umbraco.Core;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Constants;
    using Our.Umbraco.Nexu.Parsers.Core;

    [TestFixture]
    public class MediaPickerParserTests
    {
        [Test]
        public void When_EditorAlias_Is_Not_Correct_IsParserFor_Should_Return_False()
        {
            // arrange       
            var parser = new MediaPickerParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.Boolean);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void When_EditorAlias_Is_Correct_IsParserFor_Should_Return_True()
        {
            // arrange
            var parser = new MediaPickerParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.MediaPicker);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void When_Value_Is_Not_Set_GetRelatedEntities_Return_Empty_List()
        {
            // arrange
            var parser = new MediaPickerParser();

            // act
            var result = parser.GetRelatedEntities(null).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count == 0);
        }

        [Test]
        public void When_Value_Is_Set_GetRelatedEntities_Return_List_With_Related_Entities()
        {
            // arrange
            var contentUdi =
                "umb://media/ca4249ed2b234337b52263cabe5587d1,umb://media/ca4249ed2b234337b52263cabe5587d2,umb://media/ca4249ed2b234337b52263cabe5587d8";

            var parser = new MediaPickerParser();

            // act
            var result = parser.GetRelatedEntities(contentUdi).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count == 3);          
            Assert.That(result.Count(x => x.RelationType == RelationTypes.DocumentToMedia) == 3);

            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/ca4249ed2b234337b52263cabe5587d1"));
            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/ca4249ed2b234337b52263cabe5587d2"));
            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/ca4249ed2b234337b52263cabe5587d8"));
        }
    }
}
