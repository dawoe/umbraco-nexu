namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Core
{
    using NUnit.Framework;

    using Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core;

    /// <summary>
    /// The media grid editor parser tests.
    /// </summary>
    [TestFixture]
    public class MediaGridEditorParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid view
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestIsParserForValidView()
        {
            // arrange            
            var parser = new MediaGridEditorParser();

            // act
            var result = parser.IsParserFor("media");

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for in valid data type.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestIsParserForInValidView()
        {
            // arrange            
            var parser = new MediaGridEditorParser();

            // act
            var result = parser.IsParserFor("foo");

            // verify
            Assert.IsFalse(result);
        }
    }
}
