namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Core
{
    using System.Linq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
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

        /// <summary>
        /// The test get linked entities with empty value.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CoreGridEditorParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange            
            var parser = new MediaGridEditorParser();

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
            var parser = new MediaGridEditorParser();

            string value = @" {
                    ""focalPoint"": {
                      ""left"": 0.4825,
                      ""top"": 0.6067415730337079
                    },
                    ""id"": 1119,
                    ""image"": ""/media/1037/9015601712_72e44263e4_b.jpg""
                  }";

            // act
            var result = parser.GetLinkedEntities(value).ToList();

            // verify
            Assert.IsNotNull(result);

            Assert.AreEqual(1, result.Count());

            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 1119));
        }
    }
}
