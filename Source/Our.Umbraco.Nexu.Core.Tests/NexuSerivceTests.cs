namespace Our.Umbraco.Nexu.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Profiling;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The nexu serivce tests.
    /// </summary>
    [TestFixture]
    public class NexuSerivceTests
    {
        private INexuService service;

        private Mock<IRelationService> relationService;

        /// <summary>
        /// Test setup
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ProfilingLogger logger = new ProfilingLogger(Mock.Of<ILogger>(), Mock.Of<IProfiler>());
            this.relationService = new Mock<IRelationService>();
            this.service = new NexuService(logger, this.relationService.Object);
        }

        /// <summary>
        /// Test teardown
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.relationService = null;
            this.service = null;
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
                x => x.GetRelationTypeByAlias(Constants.RelationTypes.DocumentToDocumentAlias));

            this.relationService.Setup(
                x => x.GetRelationTypeByAlias(Constants.RelationTypes.DocumentToMediaAlias));

            this.relationService.Setup(x => x.Save(It.IsAny<IRelationType>())).Callback(
                (IRelationType x) =>
                    {
                        actualRelationTypes.Add(x);
                    });

            // act
            this.service.SetupRelationTypes();

            // verify
            this.relationService.Verify(
                x => x.GetRelationTypeByAlias(Constants.RelationTypes.DocumentToDocumentAlias), Times.Once);

            this.relationService.Verify(
                x => x.GetRelationTypeByAlias(Constants.RelationTypes.DocumentToMediaAlias), Times.Once);

            this.relationService.Verify(x => x.Save(It.IsAny<IRelationType>()), Times.Exactly(2));

            Assert.IsTrue(actualRelationTypes.Any(x => x.Alias.Equals(Constants.RelationTypes.DocumentToDocumentAlias)));
            Assert.IsTrue(actualRelationTypes.Any(x => x.Alias.Equals(Constants.RelationTypes.DocumentToMediaAlias)));

            var docRelType = actualRelationTypes.First(x => x.Alias.Equals(Constants.RelationTypes.DocumentToDocumentAlias));

            Assert.AreEqual(Constants.RelationTypes.DocumentToDocumentName, docRelType.Name);
            Assert.IsFalse(docRelType.IsBidirectional);
            Assert.AreEqual(new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document), docRelType.ChildObjectType);
            Assert.AreEqual(new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document), docRelType.ParentObjectType);

            var mediaRelType = actualRelationTypes.First(x => x.Alias.Equals(Constants.RelationTypes.DocumentToMediaAlias));

            Assert.AreEqual(Constants.RelationTypes.DocumentToMediaName, mediaRelType.Name);
            Assert.IsFalse(mediaRelType.IsBidirectional);
            Assert.AreEqual(new Guid(global::Umbraco.Core.Constants.ObjectTypes.Media), mediaRelType.ChildObjectType);
            Assert.AreEqual(new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document), mediaRelType.ParentObjectType);

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
                    x => x.GetRelationTypeByAlias(Constants.RelationTypes.DocumentToDocumentAlias))
                .Returns(new RelationType(Guid.Empty, Guid.Empty, Constants.RelationTypes.DocumentToDocumentAlias));

            this.relationService.Setup(
                x => x.GetRelationTypeByAlias(Constants.RelationTypes.DocumentToMediaAlias))
                .Returns(new RelationType(Guid.Empty, Guid.Empty, Constants.RelationTypes.DocumentToMediaAlias));

            // act
            this.service.SetupRelationTypes();

            // verify
            this.relationService.Verify(
                x => x.GetRelationTypeByAlias(Constants.RelationTypes.DocumentToDocumentAlias), Times.Once);

            this.relationService.Verify(
                x => x.GetRelationTypeByAlias(Constants.RelationTypes.DocumentToMediaAlias), Times.Once);
        }
    }
}
