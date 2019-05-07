namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using System.Linq;

    using global::Umbraco.Core;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Constants;
    using Our.Umbraco.Nexu.Parsers.Core;

    [TestFixture]
    public class NestedContentParserTests
    {
        [Test]
        public void When_EditorAlias_Is_Not_Correct_IsParserFor_Should_Return_False()
        {
            // arrange       
            var parser = new NestedContentParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.Boolean);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void When_EditorAlias_Is_Correct_IsParserFor_Should_Return_True()
        {
            // arrange
            var parser = new NestedContentParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.NestedContent);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void When_Value_Is_Not_Set_GetRelatedEntities_Return_Empty_List()
        {
            // arrange
            var parser = new NestedContentParser();

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
            var contentUdi = @"[[
   {
      ""key"":""69d6cf63-f0e5-463d-9441-e1b4ad6fb25e"",
      ""name"":""Item 1"",
      ""ncContentTypeAlias"":""nestedItem"",
      ""text"":""<p>asdfasfa as <a data-udi=\""umb://document/3cce2545e3ac44ecbf55a52cc5965db3\"" href=\""/{localLink:umb://document/3cce2545e3ac44ecbf55a52cc5965db3}\"" title=\""About Us\"">asd </a>asd as </p>"",
      ""callToAction"":""[{\""name\"":\""Products\"",\""udi\"":\""umb://document/ec4aafcc0c254f25a8fe705bfae1d324\""}]""
   },
   {
      ""key"":""ac61e2d2-d8e4-418d-aa91-99ead3df3f8d"",
      ""name"":""Item 2"",
      ""ncContentTypeAlias"":""nestedItem"",
      ""text"":""<p>asdfasdf a as </p>"",
      ""callToAction"":""[{\""name\"":\""Umbraco Campari Meeting Room\"",\""udi\"":\""umb://media/662af6ca411a4c93a6c722c4845698e7\""}]""
   }
]]";

            var parser = new NestedContentParser();

            // act
            var result = parser.GetRelatedEntities(contentUdi).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count == 3);
            Assert.That(result.Count(x => x.RelationType == RelationTypes.DocumentToDocument) == 2);
            Assert.That(result.Count(x => x.RelationType == RelationTypes.DocumentToMedia) == 1);

            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://document/3cce2545e3ac44ecbf55a52cc5965db3" && x.RelationType == RelationTypes.DocumentToDocument));
            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://document/ec4aafcc0c254f25a8fe705bfae1d324" && x.RelationType == RelationTypes.DocumentToDocument));
            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/662af6ca411a4c93a6c722c4845698e7" && x.RelationType == RelationTypes.DocumentToMedia));
        }
    }
}
