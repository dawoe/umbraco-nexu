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

    using Our.Umbraco.Nexu.Core.Enums;
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
            SettingsForTests.ConfigureSettings(SettingsForTests.GenerateMockSettings());
            this.relationService = new Mock<IRelationService>();
            this.service = new NexuService(this.ProfilingLogger, this.relationService.Object, PropertyParserResolver.Current);
        }

        /// <summary>
        /// Override freeze resolution, so we can init our resolver
        /// </summary>
        protected override void FreezeResolution()
        {
            var assembly = Assembly.Load("Our.Umbraco.Nexu.Parsers");
            this.parsers =
                TypeFinder.FindClassesOfType<Interfaces.IPropertyParser>(new List<Assembly> { assembly }).ToList();            

            // set up pattern model resolver
            PropertyParserResolver.Current = new PropertyParserResolver(
                new ActivatorServiceProvider(),
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
        [Category("Service")]
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
        [Category("Service")]
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
        [Category("Service")]
        [Category("Parsing")]
        public void TestGetAllPropertyParsers()
        {
            // act
            var result = this.service.GetAllPropertyParsers();

            // verify
            Assert.IsNotNull(result);
            Assert.AreEqual(this.parsers.Count(), result.Count());
        }

        /// <summary>
        /// Test get all property parsers for content item.
        /// </summary>
        [Test]
        [Category("Service")]
        [Category("Parsing")]
        public void TestGetParsablePropertiesForContent()
        {
            // arrange
            var content = new Mock<IContent>();

            content.SetupGet(x => x.PropertyTypes)
                .Returns(
                    new List<PropertyType>()
                        {
                            new PropertyType(
                                Constants.PropertyEditors.ContentPickerAlias,
                                DataTypeDatabaseType.Integer,
                                "contentPicker"),
                            new PropertyType(
                                Constants.PropertyEditors.TextboxAlias,
                                DataTypeDatabaseType.Nvarchar,
                                "textbox")
                        });            

            // act
            var result = this.service.GetParsablePropertiesForContent(content.Object);

            // verify
            Assert.IsNotNull(result);

            Assert.AreEqual(1, result.Count());

        }

        /// <summary>
        /// Test get linked entities for content
        /// </summary>
        [Test]
        [Category("Service")]
        [Category("Parsing")]
        public void TestGetLinkedEntitesForContent()
        {
            // arrange
            var content = new Mock<IContent>();

            var propTypeCp1 = new PropertyType(
                                   Constants.PropertyEditors.ContentPickerAlias,
                                   DataTypeDatabaseType.Integer,
                                   "contentPicker");

            var propTypeCp2 = new PropertyType(
                                   Constants.PropertyEditors.ContentPickerAlias,
                                   DataTypeDatabaseType.Integer,
                                   "contentPicker2");

            var propTypeText = new PropertyType(
                                   Constants.PropertyEditors.TextboxAlias,
                                   DataTypeDatabaseType.Nvarchar,
                                   "textbox");

            content.SetupGet(x => x.PropertyTypes)
                .Returns(
                    new List<PropertyType>()
                        {
                            propTypeCp1,
                            propTypeText,
                            propTypeCp2
                        });

            content.SetupGet(x => x.Properties)
                .Returns(
                    new PropertyCollection(
                        new List<Property>()
                            {
                                new Property(propTypeCp1, 1500),
                                new Property(propTypeText, "Foo"),
                                new Property(propTypeCp2, 1500)
                            }));

            content.SetupGet(x => x.Id).Returns(1234);
            content.SetupGet(x => x.Name).Returns("Foo content");

            // act
            var result = this.service.GetLinkedEntitesForContent(content.Object);

            // verify
            Assert.IsNotNull(result);
            
            var linkedEntities = result.ToList();
            Assert.AreEqual(1, linkedEntities.Count);

            var linkedDoc = linkedEntities.First();

            Assert.AreEqual(1500, linkedDoc.Id);
            Assert.AreEqual(LinkedEntityType.Document, linkedDoc.LinkedEntityType);
        }
    }
}
