namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Core
{
    using System.Linq;

    using global::Umbraco.Core.Models;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Core;

    /// <summary>
    /// The media picker parser tests.
    /// </summary>
    [TestFixture]
    public class MultipleMediaPickerParserTests : BaseParserTest
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
            var dataTypeDefinition = new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.MultipleMediaPickerAlias);

            var parser = new MultipleMediaPickerParser();

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

            var parser = new MultipleMediaPickerParser();

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
            var parser = new MultipleMediaPickerParser();

            var propertyType = new PropertyType(
                              global::Umbraco.Core.Constants.PropertyEditors.MultipleMediaPickerAlias,
                              DataTypeDatabaseType.Nvarchar,
                              "cp1");

            var property = new Property(propertyType, null);

            // act
            var result = parser.GetLinkedEntities(property.Value);

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
            var parser = new MultipleMediaPickerParser();

            var propertyType = new PropertyType(
                              global::Umbraco.Core.Constants.PropertyEditors.MultipleMediaPickerAlias,
                              DataTypeDatabaseType.Nvarchar,
                              "cp1");

            var property = new Property(propertyType, "100,101,104");

            // act
            var result = parser.GetLinkedEntities(property.Value);

            // verify
            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(3, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 100));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 101));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Media && x.Id == 104));
        }
    }
}
