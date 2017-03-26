namespace Our.Umbraco.Nexu.Parsers.Tests.PropertyParsers.Community
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Parsers.PropertyParsers.Community;

    /// <summary>
    /// The vorto parser tests.
    /// </summary>
    [TestFixture]
    public class VortoParserTests : BaseParserTest
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
            var dataTypeDefinition = new DataTypeDefinition("Our.Umbraco.Vorto");

            var parser = new VortoParser();

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

            var parser = new VortoParser();

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
            var parser = new VortoParser();

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
            var dataTypeServiceMock = new Mock<IDataTypeService>();

            var vortoGuid = new Guid("a49b995e-6ef7-4d2a-beab-0d545b42aaf0");

            var rteGuid = new Guid("ca90c950-0aff-4e72-b976-a30b1ac57dad");

            var vortoDataType = new DataTypeDefinition("Our.Umbraco.Vorto") { Id = 1 };
            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(vortoGuid)).Returns(vortoDataType);

            var prevalue = new JObject(new JProperty("guid", rteGuid.ToString("D")));

            dataTypeServiceMock.Setup(x => x.GetPreValuesCollectionByDataTypeId(vortoDataType.Id))
                .Returns(
                    new PreValueCollection(new Dictionary<string, PreValue>
                                               {
                        { "dataType", new PreValue(prevalue.ToString()) }
                                               }));

            dataTypeServiceMock.Setup(x => x.GetDataTypeDefinitionById(rteGuid)).Returns(new DataTypeDefinition(global::Umbraco.Core.Constants.PropertyEditors.TinyMCEAlias));

            var parser = new VortoParser(dataTypeServiceMock.Object);

            string propValue =
                new JObject(
                    new JProperty(
                        "values",
                        new JObject(
                            new JProperty(
                                "en-US",
                                @"<p>English content <a data-id=\""1068\"" href=\""/{localLink:1068}\"" title=\""Explore\"">here</a></p>"),
                            new JProperty(
                                "nl-NL",
                                @"<p>Dutch content <a data-id=\""1076\"" href=\""/{localLink:1076}\"" title=\""Explore\"">hier</a></p>"))),
                    new JProperty("dtdGuid", vortoGuid.ToString("D"))).ToString();

            // act
            var result = parser.GetLinkedEntities(propValue);

            // verify           
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(vortoGuid), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetPreValuesCollectionByDataTypeId(vortoDataType.Id), Times.Once);
            dataTypeServiceMock.Verify(x => x.GetDataTypeDefinitionById(rteGuid), Times.Once);



            Assert.IsNotNull(result);
            var entities = result.ToList();
            Assert.AreEqual(2, entities.Count());

            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 1068));
            Assert.IsTrue(entities.Exists(x => x.LinkedEntityType == LinkedEntityType.Document && x.Id == 1076));
        }
    }
}
