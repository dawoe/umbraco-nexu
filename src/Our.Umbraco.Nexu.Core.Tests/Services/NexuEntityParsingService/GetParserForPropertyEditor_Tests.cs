namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Core.Composing.Collections;
    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    /// Represents the tests for GetParserForPropertyEditor method on the NexuEntityParsingService
    /// </summary>
    public class GetParserForPropertyEditor_Tests
    {
        /// <summary>
        /// The property value parser collection used in all tests
        /// </summary>
        private PropertyValueParserCollection propertyValueParserCollection;

        /// <summary>
        /// The service instance used in all tests
        /// </summary>
        private NexuEntityParsingService service;

        private Mock<ILogger> loggerMock;

        /// <summary>
        /// The setup that is run for all tests
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var parsers = new List<IPropertyValueParser>();

            var contentPickerParser = new Mock<IPropertyValueParser>();
            contentPickerParser.Setup(x => x.IsParserFor(Constants.PropertyEditors.Aliases.ContentPicker))
                .Returns(true);

            parsers.Add(contentPickerParser.Object);

            var rteParser = new Mock<IPropertyValueParser>();
            rteParser.Setup(x => x.IsParserFor(Constants.PropertyEditors.Aliases.TinyMce))
                .Returns(true);

            parsers.Add(rteParser.Object);

            this.propertyValueParserCollection = new PropertyValueParserCollection(parsers);

            this.loggerMock = new Mock<ILogger>();

            this.service = new NexuEntityParsingService(this.propertyValueParserCollection, this.loggerMock.Object);
        }

        [Test]
        [TestCase(Constants.PropertyEditors.Aliases.ContentPicker)]
        [TestCase(Constants.PropertyEditors.Aliases.TinyMce)]
        public void When_Parser_Exists_In_Collection_GetParserForPropertyEditor_Should_Return_It(string editorAlias)
        {
            // arrange
            Assume.That(this.propertyValueParserCollection.Count > 0);

            // act
            var parser = this.service.GetParserForPropertyEditor(editorAlias);

            // assert
            Assert.IsNotNull(parser);
            Assert.IsTrue(parser.IsParserFor(editorAlias));

        }

        [Test]
        public void When_Parser_Does_Not_Exists_In_Collection_GetParserForPropertyEditor_Should_Return_Null()
        {
            // arrange
            Assume.That(this.propertyValueParserCollection.Count > 0);

            var editorAlias = Constants.PropertyEditors.Aliases.MediaPicker;

            // act
            var parser = this.service.GetParserForPropertyEditor(editorAlias);

            // assert
            Assert.IsNull(parser);
        }
    }
}
