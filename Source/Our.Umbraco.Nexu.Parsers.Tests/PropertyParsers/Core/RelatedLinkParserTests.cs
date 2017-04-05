namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System.Linq;

    using global::Umbraco.Core.Models;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The related link parser tests.
    /// </summary>
    [TestFixture]
    public class RelatedLinkParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid data type.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestIsParserForValidDataType()
        {
            // arrange
            var dataTypeDefinition =
                new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.RelatedLinksAlias);

            var parser = new RelatedLinksParser();

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
        [Category("CorePropertyParsers")]
        public void TestIsParserForInValidDataType()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("foo");

            var parser = new RelatedLinksParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The test get linked entities with empty value.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange
            var parser = new RelatedLinksParser();

            object propValue = null;

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(0, entities.Count());
        }

        /// <summary>
        /// The test get linked entities with value.
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithValue()
        {
            // arrange
            var parser = new RelatedLinksParser();

            string propValue = @"[
                      {
                        ""caption"": ""External"",
                        ""link"": ""http://www.google.be"",
                        ""newWindow"": false,
                        ""edit"": false,
                        ""isInternal"": false,
                        ""type"": ""external"",
                        ""title"": ""External""
                      },
                      {
                        ""caption"": ""Internal page empty"",
                        ""link"": null,
                        ""newWindow"": false,
                        ""internal"": null,
                        ""edit"": false,
                        ""isInternal"": true,
                        ""internalName"": """",
                        ""type"": ""internal"",
                        ""title"": ""Internal page empty""
                      },
                      {
                        ""caption"": ""Internal page"",
                        ""link"": 1079,
                        ""newWindow"": false,
                        ""internal"": 1079,
                        ""edit"": false,
                        ""isInternal"": true,
                        ""internalName"": ""Contact"",
                        ""type"": ""internal"",
                        ""title"": ""Internal page""
                      },
                      {
                        ""caption"": ""Empty external"",
                        ""link"": ""http://"",
                        ""newWindow"": false,
                        ""edit"": false,
                        ""isInternal"": false,
                        ""type"": ""external"",
                        ""title"": ""Empty external""
                      }
                    ]";

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(1, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 1079));
        }
    }
}
