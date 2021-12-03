namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using System.Linq;

    using global::Umbraco.Core;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Constants;
    using Our.Umbraco.Nexu.Parsers.Core;

    [TestFixture]
    public class BlockListContentParserTests
    {
        [Test]
        public void When_EditorAlias_Is_Not_Correct_IsParserFor_Should_Return_False()
        {
            // arrange       
            var parser = new BlockListEditorParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.Boolean);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void When_EditorAlias_Is_Correct_IsParserFor_Should_Return_True()
        {
            // arrange
            var parser = new BlockListEditorParser();

            // act
            var result = parser.IsParserFor("Umbraco.BlockList");

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void When_Value_Is_Not_Set_GetRelatedEntities_Return_Empty_List()
        {
            // arrange
            var parser = new BlockListEditorParser();

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
            var contentUdi = @"{
   ""layout"":{
      ""Umbraco.BlockList"":[
         {
            ""contentUdi"":""umb://element/0e4eaf12e06e4743860cc00076661dee""
         }
      ]
   },
   ""contentData"":[
      {
         ""contentTypeKey"":""08f41fb4-15a7-44c2-96d9-e8ed69707df6"",
         ""udi"":""umb://element/086b88ada4f34d56b7f457682d351c4f"",
         ""image"":""[\r\n  {\r\n    \""key\"": \""d65c7900-6814-4b09-8576-8cb9ec35f68d\"",\r\n    \""mediaKey\"": \""9b8ee0f2-9037-497f-8886-74b16e65d24e\"",\r\n    \""crops\"": [],\r\n    \""focalPoint\"": {\r\n      \""left\"": 0.5,\r\n      \""top\"": 0.5\r\n    }\r\n  }\r\n]"",
         ""caption"":""This is an optional caption for a test image."",
         ""altText"":null
      }
   ],
   ""settingsData"":[
      
   ]
}";

            var parser = new BlockListEditorParser();

            // act
            var result = parser.GetRelatedEntities(contentUdi).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count == 1);
            Assert.That(result.Count(x => x.RelationType == RelationTypes.DocumentToMedia) == 1);

            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/9b8ee0f29037497f888674b16e65d24e" && x.RelationType == RelationTypes.DocumentToMedia));
        }
    }
}
