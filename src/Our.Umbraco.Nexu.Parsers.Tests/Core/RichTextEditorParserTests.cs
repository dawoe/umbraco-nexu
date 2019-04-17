namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using global::Umbraco.Core;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Constants;
    using Our.Umbraco.Nexu.Parsers.Core;

    /// <summary>
    /// The rich text editor parser tests.
    /// </summary>
    [TestFixture]
    public class RichTextEditorParserTests : BaseParserTest
    {
        [Test]
        public void When_EditorAlias_Is_Not_Correct_IsParserFor_Should_Return_False()
        {
            // arrange
            var prop = this.CreateProperty(Constants.PropertyEditors.Aliases.Boolean);

            var parser = new RichTextEditorParser();

            // act
            var result = parser.IsParserFor(prop);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void When_EditorAlias_Is_Correct_IsParserFor_Should_Return_True()
        {
            // arrange
            var prop = this.CreateProperty(Constants.PropertyEditors.Aliases.TinyMce);

            var parser = new RichTextEditorParser();

            // act
            var result = parser.IsParserFor(prop);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void When_Value_Is_Not_Set_GetRelatedEntities_Return_Empty_List()
        {
            // arrange
            var prop = this.CreatePropertyWithValues(Constants.PropertyEditors.Aliases.Boolean, new Dictionary<string, object>());

            var parser = new RichTextEditorParser();

            // act
            var result = parser.GetRelatedEntities(prop);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void When_Value_Is_Set_GetRelatedEntities_Return_List_With_Related_Entities()
        {
            // arrange
            var cultureValues = new Dictionary<string, object>();

            var nlValue = @"<p>Hier komt de tekst voor de contact pagina</p>
                            <p>Hier kunnen links in gezet <a data-udi=""umb://document/ca4249ed2b234337b52263cabe5587d1"" href=""/{localLink:umb://document/ca4249ed2b234337b52263cabe5587d1}"" title=""Home"">worden </a></p>
                            <p>En afbeeldingen geplaats</p>
                            <p><img style=""width: 500px; height:333.4375px;"" src=""/media/34371d0892c84015912ebaacd002c5d0/00000006000000000000000000000000/18530280048_459b8b61b2_h.jpg?width=500&amp;height=333.4375"" alt="""" data-udi=""umb://media/34371d0892c84015912ebaacd002c5d0"" /></p>
                            <p> </p>
                            <p>Maar we kunnen ook bestanden <a data-udi=""umb://media/a7e62beab9834049aaf765f5f95f2263"" href=""/media/bxlfm1pv/715010_lr.pdf"" title=""715010 LR"">opladen</a></p>";

            var enValue =
                @"<p>Lorem <a data-udi=""umb://document/ca4249ed2b234337b52263cabe5587d1"" href=""/{localLink:umb://document/ca4249ed2b234337b52263cabe5587d1}"" title=""Home"">ipsum</a> dolor sit amet, consectetur adipiscing elit. Nullam eget lacinia nisl. Aenean sollicitudin diam vitae enim ultrices, semper euismod magna efficitur.</p>";

            cultureValues.Add("nl-NL", nlValue);
            cultureValues.Add("en-US", enValue);

            var prop = this.CreatePropertyWithValues(Constants.PropertyEditors.Aliases.Boolean, cultureValues);

            var parser = new RichTextEditorParser();

            // act
            var result = parser.GetRelatedEntities(prop).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(3, result.Count(x => x.Culture == "nl-nl"));
            Assert.AreEqual(1, result.Count(x => x.Culture == "en-us"));

            Assert.AreEqual(2, result.Count(x => x.RelationType == RelationTypes.DocumentToDocument));
            Assert.AreEqual(2, result.Count(x => x.RelationType == RelationTypes.DocumentToMedia));

            Assert.IsTrue(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://document/ca4249ed2b234337b52263cabe5587d1" && x.Culture == "nl-nl" && x.RelationType == RelationTypes.DocumentToDocument));
            Assert.IsTrue(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://document/ca4249ed2b234337b52263cabe5587d1" && x.Culture == "en-us" && x.RelationType == RelationTypes.DocumentToDocument));
            Assert.IsTrue(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/a7e62beab9834049aaf765f5f95f2263" && x.Culture == "nl-nl" && x.RelationType == RelationTypes.DocumentToMedia));
            Assert.IsTrue(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/34371d0892c84015912ebaacd002c5d0" && x.Culture == "nl-nl" && x.RelationType == RelationTypes.DocumentToMedia));
        }
    }
}
