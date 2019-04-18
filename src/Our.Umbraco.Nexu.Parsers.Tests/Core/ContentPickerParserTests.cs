namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Constants;
    using Our.Umbraco.Nexu.Parsers.Core;

    /// <summary>
    /// The rich text editor parser tests.
    /// </summary>
    [TestFixture]
    public class ContentPickerParserTests : BaseParserTest
    {
        [Test]
        public void When_EditorAlias_Is_Not_Correct_IsParserFor_Should_Return_False()
        {
            // arrange
            var prop = this.CreateProperty(Constants.PropertyEditors.Aliases.Boolean);

            var parser = new ContentPickerParser();

            // act
            var result = parser.IsParserFor(prop);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void When_EditorAlias_Is_Correct_IsParserFor_Should_Return_True()
        {
            // arrange
            var prop = this.CreateProperty(Constants.PropertyEditors.Aliases.ContentPicker);

            var parser = new ContentPickerParser();

            // act
            var result = parser.IsParserFor(prop);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void When_Value_Is_Not_Set_GetRelatedEntities_Return_Empty_List()
        {
            // arrange
            var prop = this.CreatePropertyWithValues(Constants.PropertyEditors.Aliases.ContentPicker, new Dictionary<string, object>());

            var parser = new ContentPickerParser();

            // act
            var result = parser.GetRelatedEntities(prop);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void When_Value_Is_Set_GetRelatedEntities_Return_List_With_Related_Entities()
        {
            // arrange
            var cultureValues = new Dictionary<string, object>();

            var nlValue = "umb://document/ca4249ed2b234337b52263cabe5587d1";

            var enValue = "umb://document/ec4aafcc0c254f25a8fe705bfae1d324";

            cultureValues.Add("nl-NL", nlValue);
            cultureValues.Add("en-US", enValue);

            var prop = this.CreatePropertyWithValues(Constants.PropertyEditors.Aliases.ContentPicker, cultureValues);

            var parser = new ContentPickerParser();

            // act
            var result = parser.GetRelatedEntities(prop).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result.Count(x => x.Culture == "nl-nl"));
            Assert.AreEqual(1, result.Count(x => x.Culture == "en-us"));

            Assert.IsTrue(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://document/ca4249ed2b234337b52263cabe5587d1" && x.Culture == "nl-nl" && x.RelationType == RelationTypes.DocumentToDocument));
            Assert.IsTrue(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://document/ec4aafcc0c254f25a8fe705bfae1d324" && x.Culture == "en-us" && x.RelationType == RelationTypes.DocumentToDocument));
        }
    }
}
