namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System.Collections.Generic;

    using global::Umbraco.Core;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    /// <summary>
    /// Represents the tests for GetParserForPropertyEditor method on the NexuEntityParsingService
    /// </summary>
    public class GetParserForPropertyEditor_Tests : NexuEntityParsingServiceBaseTest
    {
        public override List<IPropertyValueParser> GetParsers()
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

            return parsers;
        }
       

        [Test]
        [TestCase(Constants.PropertyEditors.Aliases.ContentPicker)]
        [TestCase(Constants.PropertyEditors.Aliases.TinyMce)]
        public void When_Parser_Exists_In_Collection_GetParserForPropertyEditor_Should_Return_It(string editorAlias)
        {
            // act
            var parser = this.Service.GetParserForPropertyEditor(editorAlias);

            // assert
            Assert.IsNotNull(parser);
            Assert.IsTrue(parser.IsParserFor(editorAlias));

        }

        [Test]
        public void When_Parser_Does_Not_Exists_In_Collection_GetParserForPropertyEditor_Should_Return_Null()
        {
            // arrange 
            var editorAlias = Constants.PropertyEditors.Aliases.MediaPicker;

            // act
            var parser = this.Service.GetParserForPropertyEditor(editorAlias);

            // assert
            Assert.IsNull(parser);
        }
    }
}
