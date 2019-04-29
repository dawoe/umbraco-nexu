namespace Our.Umbraco.Nexu.Core.Tests.Services
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Moq;

    using NPoco;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Core.Composing.Collections;
    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    /// Represents the nexu content parsing service tests.
    /// </summary>
    [TestFixture]
    public class NexuEntityParsingServiceTests
    {
        /// <summary>
        /// The content used in tests
        /// </summary>
        private IContent content;

        /// <summary>
        /// The property value parse collection used in tests
        /// </summary>
        private PropertyValueParserCollection propertyValueParserCollection;

        /// <summary>
        /// The mocked content picker parser used in tests
        /// </summary>
        private Mock<IPropertyValueParser> contentPickerParser;

        /// <summary>
        /// The parsers used in the test
        /// </summary>
        private Property contentPickerProperty;


        private List<IPropertyValueParser> parsers;

        private NexuEntityParsingService service;

        /// <summary>
        /// Setup up used for all tests
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.SetupTestData();
        }

        /// <summary>
        /// Tears down all tests
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.contentPickerProperty = null;
            this.contentPickerParser = null;
            this.parsers = null;
            this.propertyValueParserCollection = null;
            this.content = null;
        }

        [Test]
        public void When_Content_Is_Blue_Print_No_Parsing_Should_Happen()
        {
            // arrange
            this.content.Blueprint = true;

            // act
            this.service.ParseContent(this.content);

            // assert
            this.contentPickerParser.Verify(x => x.IsParserFor(It.IsAny<string>()), Times.Never);
            this.contentPickerParser.Verify(x => x.GetRelatedEntities(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void When_Content_Is_Not_Blue_Print_Parsing_Should_Happen()
        {
            // arrange
            this.content.Blueprint = false;

            // act
            this.service.ParseContent(this.content);

            // assert
            this.contentPickerParser.Verify(x => x.IsParserFor(this.contentPickerProperty.PropertyType.PropertyEditorAlias), Times.AtLeastOnce);
            this.contentPickerParser.Verify(x => x.GetRelatedEntities(It.IsAny<string>()), Times.Exactly(2));
        }

        /// <summary>
        /// Creates a property for the editor alias
        /// </summary>
        /// <param name="editorAlias">
        /// The editor alias.
        /// </param>
        /// <returns>
        /// The <see cref="Property"/>.
        /// </returns>
        private Property CreateProperty(string editorAlias)
        {
            var propertyType =
                new PropertyType(editorAlias, ValueStorageType.Ntext)
                    {
                        Variations = ContentVariation.Culture
                    };

            return new Property(propertyType);
        }

        /// <summary>
        /// Creates create property with values.
        /// </summary>
        /// <param name="editorAlias">
        /// The editor alias.
        /// </param>
        /// <param name="cultureValues">
        /// The culture values dictionary. the key is the culture
        /// </param>
        /// <returns>
        /// The <see cref="Property"/>.
        /// </returns>
        private Property CreatePropertyWithValues(string editorAlias, Dictionary<string, object> cultureValues)
        {
            var propertyWithValues = this.CreateProperty(editorAlias);

            foreach (var key in cultureValues.Keys)
            {
                propertyWithValues.SetValue(cultureValues[key], key);
            }

            return propertyWithValues;
        }

        private void SetupTestData()
        {
            // arrange
            this.content = Mock.Of<IContent>();

            var cultureValues = new Dictionary<string, object>();

            var nlValue = "umb://document/ca4249ed2b234337b52263cabe5587d1";

            var enValue = "umb://document/ec4aafcc0c254f25a8fe705bfae1d324";

            cultureValues.Add("nl-NL", nlValue);
            cultureValues.Add("en-US", enValue);

            this.contentPickerProperty = this.CreatePropertyWithValues(
                Constants.PropertyEditors.Aliases.ContentPicker,
                cultureValues);

            this.content.Properties = new PropertyCollection(new[] { this.contentPickerProperty });

            this.contentPickerParser = new Mock<IPropertyValueParser>();
            this.contentPickerParser.Setup(x => x.IsParserFor(Constants.PropertyEditors.Aliases.ContentPicker))
                .Returns(true);
            this.contentPickerParser.Setup(x => x.GetRelatedEntities(nlValue));
            this.contentPickerParser.Setup(x => x.GetRelatedEntities(enValue));

            this.parsers = new List<IPropertyValueParser>();

            this.parsers.Add(this.contentPickerParser.Object);

            this.propertyValueParserCollection = new PropertyValueParserCollection(this.parsers);

            this.service = new NexuEntityParsingService(this.propertyValueParserCollection);
        }
    }
}
