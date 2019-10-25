namespace Our.Umbraco.Nexu.Parsers.Tests.GridEditorParsers.Community
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.GridEditorParsers.Community;
    using Our.Umbraco.Nexu.Parsers.GridEditorParsers.Core;

    /// <summary>
    /// The doc type grid editor parser tests.
    /// </summary>
    [TestFixture]
    public class DocTypeGridEditorParserTests : BaseParserTest
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
            var parser = new DocTypeGridEditorParser();

            // act
            var result = parser.IsParserFor("/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.html");

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
            var parser = new DocTypeGridEditorParser();

            // act
            var result = parser.IsParserFor("foo");

            // verify
            Assert.IsFalse(result);
        }

        [Test]
        [Category("GridEditorParsers")]
        [Category("CommunityGridEditorParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange            
            var parser = new DocTypeGridEditorParser();

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
            var contentType = "DTGEDocType";
            var pickerId = 1079;
            var picker2Id = 1080;
            var mediaPickerId = 1068;
            var contentId = 1086;

            // setup datatype service
            var contentPickerDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias)
            {
                Id = 1
            };

            var mediaPickerDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.MediaPickerAlias)
            {
                Id = 2
            };

            var rteDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias)
                                            {
                                                Id = 3
                                            };

            var stringDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TextboxAlias)
                                               {
                                                   Id = 4
                                               };

            var dataTypeServiceMock = new Mock<IDataTypeService>();
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(contentPickerDataTypeDefinition.Id))
                .Returns(contentPickerDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(mediaPickerDataTypeDefinition.Id))
                .Returns(mediaPickerDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(rteDataTypeDefinition.Id))
                .Returns(rteDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(stringDataTypeDefinition.Id))
                .Returns(stringDataTypeDefinition);

            // setup content type service
            var pickerAlias = "picker";
            var mediaPickerAlias = "mediaPicker";
            var stringAlias = "string";
            var rteAlias = "text";
            var picker2Alias = "picker2";

            var contentTypeMock = new Mock<IContentType>();

            contentTypeMock.SetupGet(x => x.CompositionPropertyTypes)
                .Returns(
                    new List<PropertyType>
                        {
                            new PropertyType(contentPickerDataTypeDefinition, pickerAlias),
                            new PropertyType(contentPickerDataTypeDefinition, picker2Alias),
                            new PropertyType(mediaPickerDataTypeDefinition, mediaPickerAlias),
                            new PropertyType(stringDataTypeDefinition, stringAlias),
                            new PropertyType(rteDataTypeDefinition, rteAlias)
                        });

            var contentTypeServiceMock = new Mock<IContentTypeService>();

            contentTypeServiceMock.Setup(x => x.GetContentType(contentType)).Returns(contentTypeMock.Object);

            // setup parser and value
            var parser = new DocTypeGridEditorParser(contentTypeServiceMock.Object, dataTypeServiceMock.Object);
            
            string value = $@"{{
                      ""dtgeContentTypeAlias"": ""{contentType}"",
                      ""value"": {{
                                    ""name"": ""Doc Type"",
                        ""{pickerAlias}"": ""{pickerId}"",
                        ""{picker2Alias}"": ""{picker2Id}"",
                        ""{mediaPickerAlias}"" : ""{mediaPickerId}"",
                        ""{stringAlias}"" : ""blabla"",
                        ""{rteAlias}"" : ""<p>Test rich text editor with <a data-id=\""{contentId}\"" href=\""/{{localLink:{contentId}}}\"" title=\""Explore\"">links</a></p>\n<p> </p>""
                      }},
                      ""id"": ""d5256865-01a2-adff-d6d9-413a7aa5227c""
                    }}";

            // act
            var result = parser.GetLinkedEntities(value).ToList();

            // verify
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(contentPickerDataTypeDefinition.Id), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(mediaPickerDataTypeDefinition.Id), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(rteDataTypeDefinition.Id), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(stringDataTypeDefinition.Id), Times.Once);

            contentTypeServiceMock.Verify(x => x.GetContentType(contentType), Times.Once);

            Assert.IsNotNull(result);

            Assert.AreEqual(4, result.Count());

            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == mediaPickerId));
            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == pickerId));
            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentId));
            Assert.IsTrue(result.Any(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == picker2Id));
        }
    }
}
