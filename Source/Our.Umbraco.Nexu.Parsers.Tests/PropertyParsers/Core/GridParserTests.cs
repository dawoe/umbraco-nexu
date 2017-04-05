namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Configuration.Grid;
    using global::Umbraco.Core.Models;

    using Moq;

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

            var gridConfigMock = new Mock<IGridConfig>();

            var parser = new GridParser(gridConfigMock.Object);

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

            var gridConfigMock = new Mock<IGridConfig>();

            var parser = new GridParser(gridConfigMock.Object);

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
            var mediaEditorAlias = "media_text_right";
            var mediaGridEditorConfigMock = new Mock<IGridEditorConfig>();
            mediaGridEditorConfigMock.SetupGet(x => x.Alias).Returns(mediaEditorAlias);
            mediaGridEditorConfigMock.SetupGet(x => x.View).Returns("media");

            var richTextEditorAlias = "rte";
            var richtTextEditorConfigMock = new Mock<IGridEditorConfig>();
            richtTextEditorConfigMock.SetupGet(x => x.Alias).Returns(richTextEditorAlias);
            richtTextEditorConfigMock.SetupGet(x => x.View).Returns("rte");

            var gridEditorsConfigMock = new Mock<IGridEditorsConfig>();
            gridEditorsConfigMock.SetupGet(x => x.Editors)
                .Returns(
                    new List<IGridEditorConfig> { mediaGridEditorConfigMock.Object, richtTextEditorConfigMock.Object });

            var gridConfigMock = new Mock<IGridConfig>();
            gridConfigMock.SetupGet(x => x.EditorsConfig).Returns(gridEditorsConfigMock.Object);

            var parser = new GridParser(gridConfigMock.Object);

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
                    ""alias"": ""{mediaEditorAlias}""
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
                    ""alias"": ""{richTextEditorAlias}""
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
            var gridConfigMock = new Mock<IGridConfig>();

            var parser = new GridParser(gridConfigMock.Object);

            // act
            var result = parser.GetLinkedEntities(null);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(0, entities.Count());
        }
    }
}
