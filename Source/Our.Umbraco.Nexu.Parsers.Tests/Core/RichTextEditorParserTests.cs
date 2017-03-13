namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using global::Umbraco.Core.Models;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Parsers.Core;

    /// <summary>
    /// The rich text editor parser tests.
    /// </summary>
    public class RichTextEditorParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid data type.
        /// </summary>
        [Test]
        [Category("Parsers")]
        [Category("CoreParsers")]
        public void TestIsParserForValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias);

            var parser = new RichTextEditorParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for in valid data type.
        /// </summary>
        [Test]
        [Category("Parsers")]
        [Category("CoreParsers")]
        public void TestIsParserForInValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("foo");

            var parser = new RichTextEditorParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }
    }
}
