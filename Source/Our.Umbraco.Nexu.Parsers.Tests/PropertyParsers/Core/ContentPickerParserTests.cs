namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System;
    using System.Linq;

    using global::Umbraco.Core.Cache;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The content picker parser tests.
    /// </summary>
    [TestFixture]
    public class ContentPickerParserTests : BaseParserTest
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
            var dataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias);

            var parser = new ContentPickerParser();

            // act
            var result = parser.IsParserFor(dataTypeDefinition);

            // verify
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The test is parser for valid data type for Umbraco 7.6
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestIsParserForValidDataTypeV76()
        {
            // arrange
            var dataTypeDefinition = new DataTypeDefinition("Umbraco.ContentPicker2");

            var parser = new ContentPickerParser();

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

            var parser = new ContentPickerParser();

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
            var parser = new ContentPickerParser();

            var propertyType = new PropertyType(
                              global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias,
                              DataTypeDatabaseType.Integer,
                              "cp1");

            var property = new Property(propertyType, 1500);

            // act
            var result = parser.GetLinkedEntities(property.Value);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(1, entities.Count());

            var entity = entities.First();

            Assert.AreEqual(LinkedEntityType.Document, entity.LinkedEntityType);
            Assert.AreEqual(1500, entity.Id);
        }

        /// <summary>
        /// The test get linked entities with value for umbraco 76
        /// </summary>
        [Test]
        [Category("PropertyParsers")]
        [Category("CorePropertyParsers")]
        public void TestGetLinkedEntitiesWithValueV76()
        {
            // arrange
            var contentServiceMock = new Mock<IContentService>();

            var cacheProviderMock = new Mock<ICacheProvider>();
            cacheProviderMock.Setup(x => x.GetCacheItem(It.IsAny<string>(), It.IsAny<Func<object>>()))
                .Returns((string k, Func<object> action) => action());

            var key = "84ccc854d4bf47d8a2d6833c9fd5fed7";
            var guid = Guid.Parse(key);

            var contentMock = new Mock<IContent>();
            contentMock.SetupGet(x => x.Id).Returns(1500);

            contentServiceMock.Setup(x => x.GetById(guid)).Returns(contentMock.Object);

            var parser = new ContentPickerParser(contentServiceMock.Object, cacheProviderMock.Object);

            var propertyType = new PropertyType(
                              "Umbraco.ContentPicker2",
                              DataTypeDatabaseType.Nvarchar,
                              "cp1");

            var property = new Property(propertyType, $"umb://document/{key}");

            // act
            var result = parser.GetLinkedEntities(property.Value);

            // verify
            contentServiceMock.Verify(x => x.GetById(guid), Times.Once);

            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(1, entities.Count());

            var entity = entities.First();

            Assert.AreEqual(LinkedEntityType.Document, entity.LinkedEntityType);
            Assert.AreEqual(1500, entity.Id);
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
            var parser = new ContentPickerParser();

            var propertyType = new PropertyType(
                              global::Umbraco.Core.Constants.PropertyEditors.ContentPickerAlias,
                              DataTypeDatabaseType.Integer,
                              "cp1");

            var property = new Property(propertyType, null);

            // act
            var result = parser.GetLinkedEntities(property.Value);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(0, entities.Count());           
        }
    }
}