namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Community
{
    using System;
    using System.Linq;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.GridEditorParsers.Community;

    /// <summary>
    /// The media grid editor parser tests.
    /// </summary>
    [TestFixture]
    public class LeBlenderGridEditorParserTests : BaseParserTest
    {
        /// <summary>
        /// The test is parser for valid view
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CommunityGridEditorParsers")]
        public void TestIsParserForValidView()
        {
            // arrange            
            var parser = new LeBlenderGridEditorParser();

            // act
            var result = parser.IsParserFor("/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html");

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for in valid data type.
        /// </summary>
        [Test]
        [Category("GridEditorParsers")]
        [Category("CommunityGridEditorParsers")]
        public void TestIsParserForInValidView()
        {
            // arrange            
            var parser = new LeBlenderGridEditorParser();

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
        [Category("CommunityGridEditorParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange            
            var parser = new LeBlenderGridEditorParser();

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
        [Category("CommunityGridEditorParsers")]
        public void TestGetLinkedEntities()
        {
            // arrange            
            var dataTypeServiceMock = new Mock<IDataTypeService>();
            
            var contentPickerDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias)
                                                      {
                                                          Key = Guid.NewGuid()
                                                      };

            var mediaPickerDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.MediaPickerAlias)
                                                    {
                Key = Guid.NewGuid()
            };

            var rteDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias)
                                            {
                Key = Guid.NewGuid()
            };

            var stringDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TextboxAlias)
                                               {
                Key = Guid.NewGuid()
            };
           
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(contentPickerDataTypeDefinition.Key))
                .Returns(contentPickerDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(mediaPickerDataTypeDefinition.Key))
                .Returns(mediaPickerDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(rteDataTypeDefinition.Key))
                .Returns(rteDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(stringDataTypeDefinition.Key))
                .Returns(stringDataTypeDefinition);

            var parser = new LeBlenderGridEditorParser(dataTypeServiceMock.Object);

            var rteLinkId = 1079;
            var rteMediaId = 1080;
            var mediaPickerId = 1068;
            var contentPickerId = 1086;

            string value = $@"[
   {{
      ""title"":{{
         ""value"":""Test"",
         ""dataTypeGuid"":""{stringDataTypeDefinition.Key}"",
         ""editorAlias"":""title"",
         ""editorName"":""Title""
      }},
      ""text"":{{
         ""value"":""<p>Content with <a data-id=\""{rteLinkId}\"" href=\""/{{localLink:{rteLinkId}}}\"" title=\""Contact\"">links</a></p>\n<p> </p>\n<p>And media</p>\n<p><img style=\""width: 500px; height:333.49609375px;\"" src=\""/media/1052/boston-city-flow.jpg?width=500&amp;height=333.49609375\"" alt=\""\"" rel=\""{rteMediaId}\"" data-id=\""{rteMediaId}\"" /></p>"",
         ""dataTypeGuid"":""{rteDataTypeDefinition.Key}"",
         ""editorAlias"":""text"",
         ""editorName"":""Text""
      }},
      ""media"":{{
         ""value"":""{mediaPickerId}"",
         ""dataTypeGuid"":""{mediaPickerDataTypeDefinition.Key}"",
         ""editorAlias"":""media"",
         ""editorName"":""Media""
      }},
      ""content"":{{
         ""value"":""{contentPickerId}"",
         ""dataTypeGuid"":""{contentPickerDataTypeDefinition.Key}"",
         ""editorAlias"":""content"",
         ""editorName"":""Content""
      }}
   }}
]";

            // act
            var result = parser.GetLinkedEntities(value).ToList();

            // assert
            

            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(contentPickerDataTypeDefinition.Key), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(mediaPickerDataTypeDefinition.Key), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(rteDataTypeDefinition.Key), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(stringDataTypeDefinition.Key), Times.Once);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(2, result.Count(x => x.LinkedEntityType == LinkedEntityType.Media));          
            Assert.AreEqual(2, result.Count(x => x.LinkedEntityType == LinkedEntityType.Document));

            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == mediaPickerId));
            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == rteMediaId));
            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == rteLinkId));
            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentPickerId));
        }
    }
}
