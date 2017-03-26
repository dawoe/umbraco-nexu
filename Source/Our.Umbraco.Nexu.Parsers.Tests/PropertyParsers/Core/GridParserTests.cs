namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System.Linq;

    using global::Umbraco.Core.Models;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The grid parser tests.
    /// </summary>
    [TestFixture]
    public class GridParserTests : BaseParserTest
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
            var dataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.GridAlias);

            var parser = new GridParser();

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

            var parser = new GridParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Test getting linked entities with a value
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithValue()
        {
            // arrange
            var parser = new GridParser();

            var mediaId = 1086;
            var contentId = 1068;

            var value = $@"{{
  ""name"": ""1 column layout"",
  ""sections"": [
    {{
      ""grid"": 12,
      ""rows"": [        
        {{
          ""name"": ""Article"",
          ""areas"": [
            {{
              ""grid"": 4,
              ""controls"": [
                {{
                  ""value"": {{
                    ""focalPoint"": {{
                      ""left"": 0.5,
                      ""top"": 0.5
                    }},
                    ""id"": {mediaId},
                    ""image"": ""/media/1050/costa-rican-frog.jpg""
                  }},
                  ""editor"": {{
                    ""alias"": ""media_text_right""
                  }}
                }}
              ]
            }},
            {{
              ""grid"": 8,
              ""controls"": [
                {{
                  ""value"": ""<p>Test rich text editor with <a data-id=\""{contentId}\"" href=\""/{{localLink:{contentId}}}\"" title=\""Explore\"">links</a></p>\n<p> </p>"",
                  ""editor"": {{
                    ""alias"": ""rte""
                  }}
                }}
              ]
            }}
          ],
          ""id"": ""1c03d717-41ff-a495-5abb-4cd6abf71d7c""
        }}
      ]
    }}
  ]
}}";

            // act
            var result = parser.GetLinkedEntities(value);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count());

            Assert.IsTrue(entities.Any(x => x.Id == mediaId && x.LinkedEntityType == LinkedEntityType.Media));
            Assert.IsTrue(entities.Any(x => x.Id == contentId && x.LinkedEntityType == LinkedEntityType.Document));
        }

        /// <summary>
        /// Test getting linked entities with a empty value
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange
            var parser = new GridParser();
            
            // act
            var result = parser.GetLinkedEntities(null);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(0, entities.Count());
        }
    }
}
