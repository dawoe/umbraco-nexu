namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using System.Linq;

    using global::Umbraco.Core;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Parsers.Core;

    /// <summary>
    /// The rich text editor parser tests.
    /// </summary>
    [TestFixture]
    public class ContentPickerParserTests 
    {
        [Test]
        public void When_EditorAlias_Is_Not_Correct_IsParserFor_Should_Return_False()
        {
            // arrange       
            var parser = new ContentPickerParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.Boolean);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void When_EditorAlias_Is_Correct_IsParserFor_Should_Return_True()
        {
            // arrange
            var parser = new ContentPickerParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.ContentPicker);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void When_Value_Is_Not_Set_GetRelatedEntities_Return_Empty_List()
        {
            // arrange
            var parser = new ContentPickerParser();

            // act
            var result = parser.GetRelatedEntities(null);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void When_Value_Is_Set_GetRelatedEntities_Return_List_With_Related_Entities()
        {
            // arrange
            var contentUdi = "umb://document/ca4249ed2b234337b52263cabe5587d1";

            var parser = new ContentPickerParser();

            // act
            var result = parser.GetRelatedEntities(contentUdi).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Exists(x => x.RelatedEntityUdi.ToString() == contentUdi));          
        }
    }
}
