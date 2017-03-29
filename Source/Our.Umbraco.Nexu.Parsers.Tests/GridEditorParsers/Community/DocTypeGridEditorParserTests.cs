namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Community
{
    using NUnit.Framework;

    using Our.Umbraco.Nexu.Parsers.GridEditorParsers.Community;

    /// <summary>
    /// The doc type grid editor parser tests.
    /// </summary>
    [TestFixture]
    public class DocTypeGridEditorParserTests : BaseParserTest
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
            var parser = new DocTypeGridEditorParser();

            // act
            var result = parser.IsParserFor("/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.html");

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
            var parser = new DocTypeGridEditorParser();

            // act
            var result = parser.IsParserFor("foo");

            // verify
            Assert.IsFalse(result);
        }
    }
}
