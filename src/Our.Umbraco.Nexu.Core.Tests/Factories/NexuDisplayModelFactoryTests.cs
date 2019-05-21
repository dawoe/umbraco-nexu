namespace Our.Umbraco.Nexu.Core.Tests.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Core.Factories;

    /// <summary>
    /// The nexu display model factory tests.
    /// </summary>
    [TestFixture]
    public class NexuDisplayModelFactoryTests
    {
        private Mock<IContentService> contentServiceMock;

        private Mock<IContentTypeService> contentTypeServiceMock;

        private Mock<ILocalizationService> localizationServiceMock;

        private NexuDisplayModelFactory factory;

        [SetUp]
        public void SetUp()
        {
            this.contentServiceMock = new Mock<IContentService>();
            this.contentTypeServiceMock = new Mock<IContentTypeService>();
            this.localizationServiceMock = new Mock<ILocalizationService>();

            this.factory = new NexuDisplayModelFactory(this.contentServiceMock.Object, this.contentTypeServiceMock.Object, this.localizationServiceMock.Object);
        }

        [Test]
        public void When_No_Relations_Passed_Content_Service_Should_Not_Be_Called()
        {
            // arrange
            var relations = new List<NexuRelation>();

            this.contentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()));

            // act
            var result = this.factory.ConvertRelationsToDisplayModels(relations).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Any() == false);

            this.contentServiceMock.Verify(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()), Times.Never);
        }

        [Test]
        public void When_Relations_Passed_Content_Service_Should_Only_Get_Unique_Content_Items()
        {
            // arrange
            var relations = new List<NexuRelation>();

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3"
            });

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3"
                              });

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db2"
                              });


            IEnumerable<Guid> actualguids = null;

            this.contentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>())).Callback((IEnumerable<Guid> guids) =>
            {
                actualguids = guids;
            }).Returns(new List<IContent>());

            // act
            var result = this.factory.ConvertRelationsToDisplayModels(relations).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Any() == false);

            Assert.That(actualguids.Count() == 2);

            this.contentServiceMock.Verify(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }

        [Test]
        public void When_Relations_Passed_Content_Service_Should_Only_Return_Display_Models_For_Existing_Content()
        {
            // arrange
            var relations = new List<NexuRelation>();

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3",
                                  Culture = "nl"
                              });           

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db2",
                                  Culture = "invariant"
                              });

            var contentId = 1234;
            
            var content = new Mock<IContent>();
            content.SetupGet(x => x.Key).Returns(Guid.Parse("3cce2545e3ac44ecbf55a52cc5965db3"));
            content.SetupGet(x => x.Id).Returns(contentId);
            content.Setup(x => x.GetCultureName(It.IsAny<string>())).Returns((string c) => "Name " + c);
            


            IEnumerable<Guid> actualguids = null;

            this.contentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>())).Callback((IEnumerable<Guid> guids) =>
                {
                    actualguids = guids;
                }).Returns(new List<IContent>() { content.Object});

            // act
            var result = this.factory.ConvertRelationsToDisplayModels(relations).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(actualguids.Count() == 2);
            Assert.That(result.Count() == 1);

            var model = result.First();

            Assert.That(model.Id == contentId);
            Assert.That(model.Name == "Name nl");
            Assert.That(model.Culture == "nl");

            this.contentServiceMock.Verify(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }

        [Test]
        public void When_Relations_Passed_Content_Service_Should_Only_Return_Display_Models_Per_Culture_For_Existing_Content()
        {
            // arrange
            var relations = new List<NexuRelation>();

            relations.Add(new NexuRelation
            {
                ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3",
                Culture = "nl"
            });

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3",
                                  Culture = "en"
                              });

            relations.Add(new NexuRelation
            {
                ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db2",
                Culture = "invariant"
            });

            var contentId = 1234;

            var content = new Mock<IContent>();
            content.SetupGet(x => x.Key).Returns(Guid.Parse("3cce2545e3ac44ecbf55a52cc5965db3"));
            content.SetupGet(x => x.Id).Returns(contentId);
            content.Setup(x => x.GetCultureName(It.IsAny<string>())).Returns((string c) => "Name " + c);



            IEnumerable<Guid> actualguids = null;

            this.contentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>())).Callback((IEnumerable<Guid> guids) =>
            {
                actualguids = guids;
            }).Returns(new List<IContent>() { content.Object });

            // act
            var result = this.factory.ConvertRelationsToDisplayModels(relations).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(actualguids.Count() == 2);
            Assert.That(result.Count() == 2);

            var model = result.First();

            Assert.That(model.Id == contentId);
            Assert.That(model.Name == "Name nl");
            Assert.That(model.Culture == "nl");

            var second = result.Last();

            Assert.That(second.Id == contentId);
            Assert.That(second.Name == "Name en");
            Assert.That(second.Culture == "en");

            this.contentServiceMock.Verify(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }

        [Test]
        public void When_Relations_Passed_Properties_Should_Be_Filled()
        {
            // arrange
            var relations = new List<NexuRelation>();

            relations.Add(new NexuRelation
            {
                ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3",
                Culture = "nl",
                PropertyAlias = "alias1"
            });

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3",
                                  Culture = "nl",
                                    PropertyAlias = "alias2"
            });

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3",
                                  Culture = "nl",
                                  PropertyAlias = "alias3"
            });

            relations.Add(new NexuRelation
            {
                ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db2",
                Culture = "invariant"
            });

            var contentId = 1234;

            var content = new Mock<IContent>();
            content.SetupGet(x => x.Key).Returns(Guid.Parse("3cce2545e3ac44ecbf55a52cc5965db3"));
            content.SetupGet(x => x.Id).Returns(contentId);
            content.Setup(x => x.GetCultureName(It.IsAny<string>())).Returns((string c) => "Name " + c);



            IEnumerable<Guid> actualguids = null;

            this.contentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>())).Callback((IEnumerable<Guid> guids) =>
            {
                actualguids = guids;
            }).Returns(new List<IContent>() { content.Object });

            // act
            var result = this.factory.ConvertRelationsToDisplayModels(relations).ToList();

            // assert
            Assert.IsNotNull(result);
            Assert.That(result.Count() == 1);

            var model = result.First();

            Assert.That(model.Id == contentId);
            Assert.That(model.Name == "Name nl");
            Assert.That(model.Culture == "nl");

            this.contentServiceMock.Verify(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }
    }
}
