namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Community
{
    using System.Linq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.GridEditorParsers.Community;

    /// <summary>
    /// The media grid editor parser tests.
    /// </summary>
    [TestFixture]
    public class LeBlenderGridEditorParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid view
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CommunityGridEditorParsers")]
        public void TestIsParserForValidView()
        {
            // arrange            
            var parser = new LeBlenderGridEditorParser();

            // act
            var result = parser.IsParserFor("/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html");

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for in valid data type.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CommunityGridEditorParsers")]
        public void TestIsParserForInValidView()
        {
            // arrange            
            var parser = new LeBlenderGridEditorParser();

            // act
            var result = parser.IsParserFor("foo");

            // verify
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The test get linked entities with empty value.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CommunityGridEditorParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange            
            var parser = new LeBlenderGridEditorParser();

            // act
            var result = parser.GetLinkedEntities(null);

            // verify
            Assert.IsNotNull(result);

            Assert.AreEqual(0, result.Count());
        }

        /// <summary>
        /// The test get linked entities.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CommunityGridEditorParsers")]
        public void TestGetLinkedEntities()
        {
            // arrange            
            var parser = new LeBlenderGridEditorParser();

            string value = @" [
  {
    ""nodeId"": {
      ""value"": ""umb://document/680f252a8501444b840715302132c53c"",
      ""dataTypeGuid"": ""fd1e0da5-5606-4862-b679-5d0cf3a52a59"",
      ""editorAlias"": ""nodeId"",
      ""editorName"": ""Content Page""
    },
    ""themePicker"": {
      ""value"": ""31"",
      ""dataTypeGuid"": ""395625fb-05b7-47b8-aace-eb57052de203"",
      ""editorAlias"": ""themePicker"",
      ""editorName"": ""Theme Picker""
    }
  }
]";

            // act
            var result = parser.GetLinkedEntities(value).ToList();

            // verify
            Assert.IsNotNull(result);

            Assert.AreEqual(1, result.Count());

            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Content && x.Id == ???));
        }
    }
}
