namespace Our.Umbraco.Nexu.Core.Tests.Services
{
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Moq;

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
        [Test]
        public void When_Content_Is_Blue_Print_No_Parsing_Should_Happen()
        {
            // arrange
            var parser = new Mock<IPropertyValueParser>();
            parser.Setup(x => x.IsParserFor(It.IsAny<string>()));
            parser.Setup(x => x.GetRelatedEntities(It.IsAny<string>()));

            var parserCollection = new PropertyValueParserCollection(new[] { parser.Object });

            var service = new NexuEntityParsingService(parserCollection);

            var content = Mock.Of<IContent>();
            content.Blueprint = true;

            // act
            service.ParseContent(content);

            // assert
            parser.Verify(x => x.IsParserFor(It.IsAny<string>()), Times.Never);
            parser.Verify(x => x.GetRelatedEntities(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void When_Content_Is_Not_Blue_Print_Parsing_Should_Happen()
        {
            // arrange
            var cultureValues = new Dictionary<string, object>();

            var nlValue = "umb://document/ca4249ed2b234337b52263cabe5587d1";

            var enValue = "umb://document/ec4aafcc0c254f25a8fe705bfae1d324";

            cultureValues.Add("nl-NL", nlValue);
            cultureValues.Add("en-US", enValue);

            var contentPickerProperty = this.CreatePropertyWithValues(
                Constants.PropertyEditors.Aliases.ContentPicker,
                cultureValues);

            var parser = new Mock<IPropertyValueParser>();
            parser.Setup(x => x.IsParserFor(contentPickerProperty.PropertyType.PropertyEditorAlias)).Returns(true);
            parser.Setup(x => x.GetRelatedEntities(nlValue));
            parser.Setup(x => x.GetRelatedEntities(enValue));

            var parserCollection = new PropertyValueParserCollection(new[] { parser.Object });

            var service = new NexuEntityParsingService(parserCollection);

            var content = Mock.Of<IContent>();
            content.Edited = false;

            content.Properties = new PropertyCollection(new []{contentPickerProperty});

            // act
            service.ParseContent(content);

            // assert
            parser.Verify(x => x.IsParserFor(contentPickerProperty.PropertyType.PropertyEditorAlias), Times.AtLeastOnce);
            parser.Verify(x => x.GetRelatedEntities(nlValue), Times.AtLeastOnce);
            parser.Verify(x => x.GetRelatedEntities(enValue), Times.AtLeastOnce);
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
    }
}
