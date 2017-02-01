namespace Our.Umbraco.Nexu.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Profiling;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Tests.TestHelpers;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Resolvers;

    /// <summary>
    /// The nexu serivce tests.
    /// </summary>
    [TestFixture]
    public class NexuSerivceTests : global::Umbraco.Tests.TestHelpers.BaseDatabaseFactoryTest
    {
        /// <summary>
        /// The service.
        /// </summary>
        private INexuService service;

        /// <summary>
        /// The relation service mock
        /// </summary>
        private Mock<IRelationService> relationService;

        /// <summary>
        /// The parsers.
        /// </summary>
        private List<Type> parsers;

        /// <summary>
        /// Override initialize to set up our own test stuff
        /// </summary>
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            this.relationService = new Mock<IRelationService>();
            this.service = new NexuService(this.ProfilingLogger, this.relationService.Object, PropertyParserResolver.Current);
        }

        /// <summary>
        /// Override freeze resolution, so we can init our resolver
        /// </summary>
        protected override void FreezeResolution()
        {
            var assembly = typeof(Interfaces.IPropertyParser).Assembly;
            this.parsers =
                TypeFinder.FindClassesOfType<Interfaces.IPropertyParser>(new List<Assembly> { assembly }).ToList();

            // set up pattern model resolver
            PropertyParserResolver.Current = new PropertyParserResolver(
                Mock.Of<IServiceProvider>(),
                this.Logger,
                this.parsers);

            base.FreezeResolution();
        }

        /// <summary>
        /// Override teardown so we can clean up our stuff
        /// </summary>
        [TearDown]
        public override void TearDown()
        {
            this.relationService = null;
            this.service = null;
            this.parsers = null;

            base.TearDown();
        }

        /// <summary>
        /// Test setup of relatoin types when they already exist.
        /// </summary>
        [Test]
        [Category("Setup")]
        public void TestSetupRelationTypessWithExistingRelationsTypes()
        {
            // arrange        
            var actualRelationTypes = new List<IRelationType>();

            this.relationService.Setup(
                x => x.GetRelationTypeByAlias(Core.Constants.RelationTypes.DocumentToDocumentAlias));

            this.relationService.Setup(
                x => x.GetRelationTypeByAlias(Core.Constants.RelationTypes.DocumentToMediaAlias));

            this.relationService.Setup(x => x.Save(It.IsAny<IRelationType>())).Callback(
                (IRelationType x) =>
                    {
                        actualRelationTypes.Add(x);
                    });

            // act
            this.service.SetupRelationTypes();

            // verify
            this.relationService.Verify(
                x => x.GetRelationTypeByAlias(Nexu.Core.Constants.RelationTypes.DocumentToDocumentAlias), Times.Once);

            this.relationService.Verify(
                x => x.GetRelationTypeByAlias(Core.Constants.RelationTypes.DocumentToMediaAlias), Times.Once);

            this.relationService.Verify(x => x.Save(It.IsAny<IRelationType>()), Times.Exactly(2));

            Assert.IsTrue(actualRelationTypes.Any(x => x.Alias.Equals(Core.Constants.RelationTypes.DocumentToDocumentAlias)));
            Assert.IsTrue(actualRelationTypes.Any(x => x.Alias.Equals(Core.Constants.RelationTypes.DocumentToMediaAlias)));

            var docRelType = actualRelationTypes.First(x => x.Alias.Equals(Core.Constants.RelationTypes.DocumentToDocumentAlias));

            Assert.AreEqual(Core.Constants.RelationTypes.DocumentToDocumentName, docRelType.Name);
            Assert.IsFalse(docRelType.IsBidirectional);
            Assert.AreEqual(new Guid(Constants.ObjectTypes.Document), docRelType.ChildObjectType);
            Assert.AreEqual(new Guid(Constants.ObjectTypes.Document), docRelType.ParentObjectType);

            var mediaRelType = actualRelationTypes.First(x => x.Alias.Equals(Core.Constants.RelationTypes.DocumentToMediaAlias));

            Assert.AreEqual(Core.Constants.RelationTypes.DocumentToMediaName, mediaRelType.Name);
            Assert.IsFalse(mediaRelType.IsBidirectional);
            Assert.AreEqual(new Guid(Constants.ObjectTypes.Media), mediaRelType.ChildObjectType);
            Assert.AreEqual(new Guid(Constants.ObjectTypes.Document), mediaRelType.ParentObjectType);

        }

        /// <summary>
        /// Test setup relations with non existing relations.
        /// </summary>
        [Test]
        [Category("Setup")]
        public void TestSetupRelationTypessWithNonExistingRelationTypes()
        {
            // arrange
            this.relationService.Setup(
                    x => x.GetRelationTypeByAlias(Core.Constants.RelationTypes.DocumentToDocumentAlias))
                .Returns(new RelationType(Guid.Empty, Guid.Empty, Core.Constants.RelationTypes.DocumentToDocumentAlias));

            this.relationService.Setup(
                x => x.GetRelationTypeByAlias(Core.Constants.RelationTypes.DocumentToMediaAlias))
                .Returns(new RelationType(Guid.Empty, Guid.Empty, Core.Constants.RelationTypes.DocumentToMediaAlias));

            // act
            this.service.SetupRelationTypes();

            // verify
            this.relationService.Verify(
                x => x.GetRelationTypeByAlias(Core.Constants.RelationTypes.DocumentToDocumentAlias), Times.Once);

            this.relationService.Verify(
                x => x.GetRelationTypeByAlias(Core.Constants.RelationTypes.DocumentToMediaAlias), Times.Once);
        }

        /// <summary>
        /// Tests retreiving of all property parsers
        /// </summary>
        [Test]
        public void TestGetAllPropertyParsers()
        {
            // act
            var result = this.service.GetAllPropertyParsers();

            // verify
            Assert.IsNotNull(result);
            Assert.AreEqual(this.parsers.Count(), result.Count());
        }
    }
}
