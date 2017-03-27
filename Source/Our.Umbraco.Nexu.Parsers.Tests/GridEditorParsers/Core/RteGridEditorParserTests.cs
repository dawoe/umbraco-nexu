namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Core
{
    using NUnit.Framework;

    using Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core;

    /// <summary>
    /// The rte grid editor parser tests.
    /// </summary>
    [TestFixture]
    public class RteGridEditorParserTests : BaseParserTest
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
            var parser = new RteGridEditorParser();

            // act
            var result = parser.IsParserFor("rte");

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
            var parser = new RteGridEditorParser();

            // act
            var result = parser.IsParserFor("foo");

            // verify
            Assert.IsFalse(result);
        }
    }
}
