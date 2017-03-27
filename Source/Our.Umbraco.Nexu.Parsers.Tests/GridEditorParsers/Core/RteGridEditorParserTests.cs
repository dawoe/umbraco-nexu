namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Core
{
    using System.Linq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
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

        /// <summary>
        /// The test get linked entities with empty value.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange            
            var parser = new RteGridEditorParser();

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
        [Category("CoreGridEditorParsers")]
        public void TestGetLinkedEntities()
        {
            // arrange            
            var parser = new RteGridEditorParser();

            var contentId = 1068;

            string value = $@"<p>Test rich text editor with <a data-id=\""{contentId}\"" href=\""/{{localLink:{contentId}}}\"" title=\""Explore\"">links</a></p>\n<p> </p>";

            // act
            var result = parser.GetLinkedEntities(value).ToList();

            // verify
            Assert.IsNotNull(result);

            Assert.AreEqual(1, result.Count());

            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentId));
        }
    }
}
