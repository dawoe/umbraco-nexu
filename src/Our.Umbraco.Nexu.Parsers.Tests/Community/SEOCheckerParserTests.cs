using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.Nexu.Parsers.Tests.Community
{
    using System.Linq;

    using global::Umbraco.Core;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Constants;
    using Our.Umbraco.Nexu.Parsers.Core;

    [TestFixture]
    public class SEOCheckerParserTests
    {
        [Test]
        public void When_EditorAlias_Is_Not_Correct_IsParserFor_Should_Return_False()
        {
            // arrange       
            var parser = new SEOCheckerParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.Boolean);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void When_EditorAlias_Is_Correct_IsParserFor_Should_Return_True()
        {
            // arrange
            var parser = new SEOCheckerParser();

            // act
            var result = parser.IsParserFor("SEOChecker.SEOCheckerSocialPropertyEditor");

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void When_Value_Is_Not_Set_GetRelatedEntities_Return_Empty_List()
        {
            // arrange
            var parser = new SEOCheckerParser();

            // act
            var result = parser.GetRelatedEntities(null).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count == 0);
        }

        [Test]
        public void When_Value_Is_Set_GetRelatedEntities_Return_List_With_Related_Entities()
        {
            // arrange
            var seoCheckerXML =
                "<SEOCheckerSocial><socialImage>umb://media/6f2d6d1d13a8438789b3cf0cced47344</socialImage><ogTitle></ogTitle><ogDescription></ogDescription><twitterTitle></twitterTitle> <twitterDescription></twitterDescription></SEOCheckerSocial>";

            var parser = new SEOCheckerParser();

            // act
            var result = parser.GetRelatedEntities(seoCheckerXML).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count == 1);
            Assert.That(result.Count(x => x.RelationType == RelationTypes.DocumentToMedia) == 1);

            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/6f2d6d1d13a8438789b3cf0cced47344"));
        }
    }
}
