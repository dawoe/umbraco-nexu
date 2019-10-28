namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The nested content parser tests.
    /// </summary>
    [TestFixture]
    public class NestedContentParserTests : BaseParserTest
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
            var dataTypeDefinition = new DataTypeDefinition("Umbraco.NestedContent");

            var parser = new NestedContentParser();

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
            var dataTypeDefinition = new DataTypeDefinition("Our.Umbraco.NestedContent");

            var parser = new NestedContentParser();

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
            var parser = new NestedContentParser();

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
        [Category("CommunityPropertyParsers")]
        public void TestGetLinkedEntitiesWithValue()
        {
            // arrange
            var contentTypeServiceMock = new Mock<IContentTypeService>();
            var dataTypeServiceMock = new Mock<IDataTypeService>();

            var contentPickerDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias)
            {
                Id = 1
            };

            var mediaPickerDataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.MediaPickerAlias)
                                                    {
                                                        Id = 2
                                                    };

            var ncContentPickerContentType = new Mock<IContentType>();           

            ncContentPickerContentType.SetupGet(x => x.CompositionPropertyTypes)
                .Returns(new List<PropertyType> { new PropertyType(contentPickerDataTypeDefinition,"picker") });

            var ncMediaPickerContentType = new Mock<IContentType>();

            ncMediaPickerContentType.SetupGet(x => x.CompositionPropertyTypes)
                .Returns(new List<PropertyType> { new PropertyType(mediaPickerDataTypeDefinition,"picker") });

            contentTypeServiceMock.Setup(x => x.GetContentType("NCContentPicker")).Returns(ncContentPickerContentType.Object);
            contentTypeServiceMock.Setup(x => x.GetContentType("NCMediaPicker")).Returns(ncMediaPickerContentType.Object);

            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(contentPickerDataTypeDefinition.Id))
                .Returns(contentPickerDataTypeDefinition);
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(mediaPickerDataTypeDefinition.Id))
                .Returns(mediaPickerDataTypeDefinition);


            var parser = new NestedContentParser(contentTypeServiceMock.Object, dataTypeServiceMock.Object);

            string propValue = "[{\"name\":\"Item 1\",\"ncContentTypeAlias\":\"NCContentPicker\",\"picker\":\"1072\"},{\"name\":\"Item 2\",\"ncContentTypeAlias\":\"NCMediaPicker\",\"picker\":\"1081\"},{\"name\":\"Item 1\",\"ncContentTypeAlias\":\"NCContentPicker\",\"picker\":\"1073\"}]";

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify
            contentTypeServiceMock.Verify(x => x.GetContentType(It.IsAny<string>()), Times.Exactly(2));

            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(contentPickerDataTypeDefinition.Id), Times.Once);                
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(mediaPickerDataTypeDefinition.Id), Times.Once);     
               
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(3, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 1072));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 1081));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 1073));
        }
    }
}
