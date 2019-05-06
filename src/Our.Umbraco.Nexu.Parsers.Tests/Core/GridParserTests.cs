namespace Our.Umbraco.Nexu.Parsers.Tests.Core
{
    using System.Linq;

    using global::Umbraco.Core;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Constants;
    using Our.Umbraco.Nexu.Parsers.Core;

    [TestFixture]
    public class GridParserTests
    {
        [Test]
        public void When_EditorAlias_Is_Not_Correct_IsParserFor_Should_Return_False()
        {
            // arrange
            var parser = new GridParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.Boolean);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void When_EditorAlias_Is_Correct_IsParserFor_Should_Return_True()
        {
            // arrange
            var parser = new GridParser();

            // act
            var result = parser.IsParserFor(Constants.PropertyEditors.Aliases.Grid);

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void When_Value_Is_Not_Set_GetRelatedEntities_Return_Empty_List()
        {
            // arrange
            var parser = new GridParser();

            // act
            var result = parser.GetRelatedEntities(null);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void When_Value_Is_Set_GetRelatedEntities_Return_List_With_Related_Entities()
        {
            // arrange
            var gridJson = @"{
  ""name"": ""1 column layout"",
  ""sections"": [
    {
      ""grid"": 12,
      ""allowAll"": true,
      ""rows"": [
        {
          ""name"": ""FullWidth"",
          ""areas"": [
            {
              ""grid"": 12,
              ""allowAll"": true,
              ""hasConfig"": false,
              ""controls"": [
                {
                  ""value"": ""Oooh la la"",
                  ""editor"": {
                    ""name"": ""Headline"",
                    ""alias"": ""headline"",
                    ""view"": ""textstring"",
                    ""render"": null,
                    ""icon"": ""icon-coin"",
                    ""config"": {
                      ""style"": ""font-size: 36px; line-height: 45px; font-weight: bold"",
                      ""markup"": ""<h1>#value#</h1>""
                    }
                  },
                  ""active"": false
                }
              ]
            }
          ],
          ""hasConfig"": false,
          ""id"": ""f10995f1-918d-3e50-e50d-8c41bbe297ce""
        },
        {
          ""label"": ""Article"",
          ""name"": ""Article"",
          ""areas"": [
            {
              ""grid"": 4,
              ""hasConfig"": false,
              ""controls"": [
                {
                  ""value"": {
                    ""udi"": ""umb://media/c0969cab13ab4de9819a848619ac2b5d"",
                    ""image"": ""/media/c0969cab13ab4de9819a848619ac2b5d/00000006000000000000000000000000/18095416144_44a566a5f4_h.jpg""
                  },
                  ""editor"": {
                    ""name"": ""Image"",
                    ""alias"": ""media"",
                    ""view"": ""media"",
                    ""render"": null,
                    ""icon"": ""icon-picture"",
                    ""config"": {}
                  },
                  ""active"": false
                }
              ]
            },
            {
              ""grid"": 8,
              ""hasConfig"": false,
              ""controls"": [
                {
                  ""value"": ""<p>Vestibulum ac diam sit <a data-udi=\""umb://document/e8ad9b65cff64952ac5befe56a60db62\"" href=\""/{localLink:umb://document/e8ad9b65cff64952ac5befe56a60db62}\"" title=\""People\"">amet</a> quam vehicula elementum sed sit amet dui. Curabitur aliquet quam id dui posuere blandit. Vivamus suscipit tortor eget felis porttitor volutpat. Proin eget tortor risus. Sed porttitor lectus nibh. Cras ultricies ligula sed magna dictum porta. Pellentesque in ipsum id orci porta dapibus. Pellentesque in ipsum id orci porta dapibus. Nulla porttitor accumsan tincidunt. Mauris blandit aliquet elit, eget tincidunt nibh pulvinar a.</p>\n<p>Vestibulum ac diam sit amet quam vehicula elementum sed sit amet dui. Curabitur aliquet quam id dui posuere blandit. Vivamus suscipit tortor eget felis porttitor volutpat. Proin eget tortor risus. Sed porttitor lectus nibh. Cras ultricies ligula sed magna dictum porta. Pellentesque in ipsum id orci porta dapibus. Pellentesque in ipsum id orci porta dapibus. Nulla porttitor accumsan tincidunt. Mauris blandit aliquet elit, eget tincidunt nibh pulvinar a.</p>"",
                  ""editor"": {
                    ""name"": ""Rich text editor"",
                    ""alias"": ""rte"",
                    ""view"": ""rte"",
                    ""render"": null,
                    ""icon"": ""icon-article"",
                    ""config"": {}
                  },
                  ""active"": false
                },
                {
                  ""value"": ""<iframe width=\""360\"" height=\""203\"" src=\""https://www.youtube.com/embed/HPgKSCp_Y_U?feature=oembed\"" frameborder=\""0\"" allow=\""accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture\"" allowfullscreen></iframe>"",
                  ""editor"": {
                    ""name"": ""Embed"",
                    ""alias"": ""embed"",
                    ""view"": ""embed"",
                    ""render"": null,
                    ""icon"": ""icon-movie-alt"",
                    ""config"": {}
                  },
                  ""active"": false
                }
              ]
            }
          ],
          ""hasConfig"": false,
          ""id"": ""4d6e2221-e2d9-95bc-8ceb-624bc8df8e3f""
        }
      ]
    }
  ]
}";

            var parser = new GridParser();

            // act
            var result = parser.GetRelatedEntities(gridJson).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count == 2);

            Assert.That(result.Count(x => x.RelationType == RelationTypes.DocumentToDocument) == 1);          

            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://document/e8ad9b65cff64952ac5befe56a60db62" && x.RelationType == RelationTypes.DocumentToDocument));
            Assert.That(result.Exists(x => x.RelatedEntityUdi.ToString() == "umb://media/c0969cab13ab4de9819a848619ac2b5d" && x.RelationType == RelationTypes.DocumentToMedia));            
        }
    }
}
