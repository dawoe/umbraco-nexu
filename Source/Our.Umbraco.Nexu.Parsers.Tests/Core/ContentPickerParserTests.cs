namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using global::Umbraco.Core.Models;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Parsers.Core;

    /// <summary>
    /// The content picker parser tests.
    /// </summary>
    [TestFixture]
    public class ContentPickerParserTests : BaseParserTest
    {        
        /// <summary>
        /// Test IsParserFor of parser with correct proptype
        /// </summary>       
        [Test]   
        [Category("Parsers")]    
        [Category("CoreParsers")]
        public void TestIsParserForValidProptype()
        {
            // arrange
            var property = new PropertyType(
                               global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias,
                               DataTypeDatabaseType.Nvarchar,
                               "cp1");

            var parser = new ContentPickerParser();

            // act
            var result = parser.IsParserFor(property);

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test IsParserFor of parser with incorrect proptype
        /// </summary>
        [Test]
        [Category("Parsers")]
        [Category("CoreParsers")]
        public void TestIsParserForInValidProptype()
        {
            // arrange
            var property = new PropertyType(
                               "foo",
                               DataTypeDatabaseType.Nvarchar,
                               "cp1");

            var parser = new ContentPickerParser();

            // act
            var result = parser.IsParserFor(property);

            // verify
            Assert.IsFalse(result);
        }
    }
}