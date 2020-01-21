namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Community
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Community;

    /// <summary>
    /// The inner content parser tests.
    /// </summary>
    [TestFixture]
    public class SEOCheckerParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid data type.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestIsParserForValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("SEOChecker.SEOCheckerSocialPropertyEditor");

            var parser = new SEOCheckerParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for in valid data type.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestIsParserForInValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("foo");

            var parser = new SEOCheckerParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }
    }
}
