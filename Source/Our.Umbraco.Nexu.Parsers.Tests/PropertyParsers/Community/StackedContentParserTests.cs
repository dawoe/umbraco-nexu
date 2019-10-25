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
    public class StackedContentParserTests : BaseParserTest
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
            var dataTypeDefinition = new DataTypeDefinition("Our.Umbraco.StackedContent");

            var parser = new StackedContentParser();

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

            var parser = new StackedContentParser();

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
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesWithEmptyValue()
        {
            // arrange
            var parser = new StackedContentParser();

            object propValue = null;

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(0, entities.Count());
        }

        [Test]
        [Category("PropertyParsers")]
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesWithValue()
        {
            // arrange
            var contentTypeServiceMock = new Mock<IContentTypeService>();
            var dataTypeServiceMock = new Mock<IDataTypeService>();

            // contentype guids
            var contentType1Guid = "8c161b51-4a69-40f7-8b07-9eadb49ccfad";
            var contentType2Guid = "f8ebe158-e596-44d5-be30-a8b79d8adabf";

            // property aliases
            var text = "text";
            var media = "media";
            var link = "link";
            var related = "related";

            // setup datatypes
            var mediaPickerDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.MediaPickerAlias)
                                                    {
                                                        Id = 2
                                                    };

            var contentPickerDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias)
                                                      {
                                                          Id = 1
                                                      };

            var rteDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias)
                                            {
                                                Id = 3
                                            };

            var textBoxDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TextboxAlias)
                                            {
                                                Id = 4
                                            };

            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(contentPickerDataTypeDefinition.Id))
                .Returns(contentPickerDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(mediaPickerDataTypeDefinition.Id))
                .Returns(mediaPickerDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(rteDataTypeDefinition.Id))
                .Returns(rteDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(textBoxDataTypeDefinition.Id))
                .Returns(textBoxDataTypeDefinition);

            // setup content types and content service calls
            var contentType1 = new Mock<IContentType>();

            contentType1.SetupGet(x => x.CompositionPropertyTypes)
                .Returns(new List<PropertyType>
                             {
                                 new PropertyType(rteDataTypeDefinition, text),
                                 new PropertyType(mediaPickerDataTypeDefinition, media)
                             });

            var contentType2 = new Mock<IContentType>();

            contentType2.SetupGet(x => x.CompositionPropertyTypes)
                .Returns(new List<PropertyType>
                             {
                                 new PropertyType(contentPickerDataTypeDefinition, link),
                                 new PropertyType(textBoxDataTypeDefinition, related)
                             });

            contentTypeServiceMock.Setup(x => x.GetContentType(new Guid(contentType1Guid))).Returns(contentType1.Object);
            contentTypeServiceMock.Setup(x => x.GetContentType(new Guid(contentType2Guid))).Returns(contentType2.Object);

            // setup test content
            var contentId = 1075;
            var mediaId = 1092;

            var propValue = $@"[  
    {{  
        ""key"":""9319498d-5ea4-4589-bdc8-aa7ea5ca39ab"",
        ""name"":""Item 1"",
        ""icon"":""icon-document"",
        ""icContentTypeGuid"":""{contentType1Guid}"",
        ""{text}"":""<p>asdfasdfasdf</p>"",
        ""{media}"":""{mediaId}""
    }},
    {{  
        ""key"":""3b39cc3f-e9f1-40f7-b4d4-a56df9d1f95f"",
        ""name"":""Item 2"",
        ""icon"":""icon-document"",
        ""icContentTypeGuid"":""{contentType2Guid}"",
        ""{link}"":""{contentId}"",
        ""{related}"":""asflkasjf""
    }},
    {{  
        ""key"":""3b39cc3f-e9f1-40f7-b4d4-a56df9d1f95f"",
        ""name"":""Item 2"",
        ""icon"":""icon-document"",
        ""icContentTypeGuid"":""{contentType2Guid}"",
        ""{link}"":"""",
        ""{related}"":""asflkasjf""
    }}
]";

            var parser = new StackedContentParser(contentTypeServiceMock.Object, dataTypeServiceMock.Object);

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify
            contentTypeServiceMock.Verify(x => x.GetContentType(new Guid(contentType1Guid)), Times.Once);
            contentTypeServiceMock.Verify(x => x.GetContentType(new Guid(contentType2Guid)), Times.Once);

            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(contentPickerDataTypeDefinition.Id), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(mediaPickerDataTypeDefinition.Id), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(rteDataTypeDefinition.Id), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(textBoxDataTypeDefinition.Id), Times.Once);

            Assert.IsNotNull(result);

            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == contentId));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == mediaId));
        }
    }
}
