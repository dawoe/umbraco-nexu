namespace Our.Umbraco.Nexu.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;
    using global::Umbraco.Tests.TestHelpers;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Constants;
    using Our.Umbraco.Nexu.Core.Enums;
    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;
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
        /// The service mock.
        /// </summary>
        private Mock<NexuService> serviceMock;

        /// <summary>
        /// The relation service mock
        /// </summary>
        private Mock<IRelationService> relationService;

        /// <summary>
        /// The data type service.
        /// </summary>
        private Mock<IDataTypeService> dataTypeService;

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
            this.dataTypeService = new Mock<IDataTypeService>();

            this.serviceMock = new Mock<NexuService>(
                                   this.ProfilingLogger,
                                   this.relationService.Object,
                                   PropertyParserResolver.Current,
                                   this.dataTypeService.Object) {
                                                                    CallBase = true 
                                                                };
            this.service = this.serviceMock.Object;
        }

        /// <summary>
        /// Override freeze resolution, so we can init our resolverC:\Users\des\Documents\My Projects\umbraco-nexu\Source\Our.Umbraco.Nexu.Core\app.config
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
            this.dataTypeService = null;
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

            var dataTypeContentPicker = new DataTypeDefinition(Constants.PropertyEditors.ContentPickerAlias) { Id = 15 };
            var dataTypeTextBox = new DataTypeDefinition(Constants.PropertyEditors.TextboxAlias) { Id = 16 };

            var propTypeCp1 =
                new PropertyType(
                    dataTypeContentPicker,
                    "contentPicker");

            var propTypeCp2 =
                new PropertyType(
                    dataTypeContentPicker,
                    "contentPicker2");

            var propTypeText =
                new PropertyType(dataTypeTextBox, "textbox");

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

            this.dataTypeService.Setup(x => x.GetDataTypeDefinitionById(15)).Returns(dataTypeContentPicker);
            this.dataTypeService.Setup(x => x.GetDataTypeDefinitionById(16)).Returns(dataTypeTextBox);

            var contetPickerParser = new Mock<IPropertyParser>();
            contetPickerParser.Setup(x => x.IsParserFor(dataTypeContentPicker)).Returns(true);
            contetPickerParser.Setup(x => x.IsParserFor(dataTypeTextBox)).Returns(false);

            this.serviceMock.Setup(x => x.GetAllPropertyParsers())
                .Returns(new List<IPropertyParser> { contetPickerParser.Object });

            // act
            var result = this.service.GetParsablePropertiesForContent(content.Object);

            // verify
            this.serviceMock.Verify(x => x.GetAllPropertyParsers(), Times.Once);
            this.dataTypeService.Verify(x => x.GetDataTypeDefinitionById(It.IsAny<int>()), Times.Exactly(3));

            Assert.IsNotNull(result);

            Assert.AreEqual(2, result.Count());

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

            var prop1 = new Property(propTypeCp1, 1500);
            var prop2 = new Property(propTypeCp2, 1500);
            content.SetupGet(x => x.Properties)
                .Returns(
                    new PropertyCollection(
                        new List<Property>()
                            {
                                prop1,
                                new Property(propTypeText, "Foo"),
                                prop2
                            }));

            content.SetupGet(x => x.Id).Returns(1234);
            content.SetupGet(x => x.Name).Returns("Foo content");

            var parser = new Mock<IPropertyParser>();
            parser.Setup(x => x.GetLinkedEntities(prop1))
                .Returns(new List<ILinkedEntity> { new LinkedDocumentEntity(1500) });

            parser.Setup(x => x.GetLinkedEntities(prop2))
               .Returns(new List<ILinkedEntity> { new LinkedDocumentEntity(1500) });

            this.serviceMock.Setup(x => x.GetParsablePropertiesForContent(content.Object))
                .Returns(
                    new List<PropertyWithParser>
                        {
                            new PropertyWithParser(prop1, parser.Object),
                            new PropertyWithParser(prop2, parser.Object)
                        });

            // act
            var result = this.service.GetLinkedEntitesForContent(content.Object);

            // verify
            this.serviceMock.Verify(x => x.GetParsablePropertiesForContent(content.Object), Times.Once);
            Assert.IsNotNull(result);

            var linkedEntities = result.ToList();
            Assert.AreEqual(1, linkedEntities.Count);

            var linkedDoc = linkedEntities.First();

            Assert.AreEqual(1500, linkedDoc.Id);
            Assert.AreEqual(LinkedEntityType.Document, linkedDoc.LinkedEntityType);
        }

        /// <summary>
        /// Tests getting nexu relations for content when content is parent
        /// </summary>
        [Test]
        [Category("Service")]
        [Category("Parsing")]
        public void TestGetNexuRelationsForContentIsParent()
        {
            // arrange
            var contentId = 1;

            this.relationService.Setup(x => x.GetByParentId(contentId))
                .Returns(
                    new List<IRelation>
                        {
                            new Relation(
                                contentId,
                                123,
                                new RelationType(
                                    Guid.NewGuid(),
                                    Guid.NewGuid(),
                                    Core.Constants.RelationTypes.DocumentToDocumentAlias)),
                            new Relation(
                                contentId,
                                456,
                                new RelationType(
                                    Guid.NewGuid(),
                                    Guid.NewGuid(),
                                    Core.Constants.RelationTypes.DocumentToMediaAlias)),
                            new Relation(contentId, 789, new RelationType(Guid.NewGuid(), Guid.NewGuid(), "foo"))
                        });

            // act
            var result = this.service.GetNexuRelationsForContent(contentId);

            // verify
            this.relationService.Verify(x => x.GetByParentId(contentId), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// Test get nexu relations for content when content is child.
        /// </summary>
        [Test]
        [Category("Service")]
        [Category("Parsing")]
        public void TestGetNexuRelationsForContentIsChild()
        {
            // arrange
            var contentId = 1;

            this.relationService.Setup(x => x.GetByChildId(contentId))
                .Returns(
                    new List<IRelation>
                        {
                            new Relation(
                                1213,
                                contentId,
                                new RelationType(
                                    Guid.NewGuid(),
                                    Guid.NewGuid(),
                                    Core.Constants.RelationTypes.DocumentToDocumentAlias)),
                            new Relation(
                                456,
                                contentId,
                                new RelationType(
                                    Guid.NewGuid(),
                                    Guid.NewGuid(),
                                    Core.Constants.RelationTypes.DocumentToMediaAlias)),                           
                            new Relation(789, contentId, new RelationType(Guid.NewGuid(), Guid.NewGuid(), "foo"))
                        });

            // act
            var result = this.service.GetNexuRelationsForContent(contentId, false);

            // verify
            this.relationService.Verify(x => x.GetByChildId(contentId), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        /// <summary>
        /// Test deleting of nexu relations for content.
        /// </summary>
        [Test]
        [Category("Service")]
        [Category("Parsing")]
        public void TestDeleteRelationsForContent()
        {
            // arrange   
            var contentId = 1;

            this.serviceMock.Setup(x => x.GetNexuRelationsForContent(contentId, true))
                .Returns(
                    new List<IRelation>()
                        {
                            new Relation(
                                contentId,
                                123,
                                new RelationType(
                                    Guid.NewGuid(),
                                    Guid.NewGuid(),
                                    Core.Constants.RelationTypes.DocumentToDocumentAlias)),
                            new Relation(
                                contentId,
                                456,
                                new RelationType(
                                    Guid.NewGuid(),
                                    Guid.NewGuid(),
                                    Core.Constants.RelationTypes.DocumentToMediaAlias))
                        });

            this.relationService.Setup(x => x.Delete(It.IsAny<IRelation>()));

            // act
            this.service.DeleteRelationsForContent(contentId);

            // verify
            this.serviceMock.Verify(x => x.GetNexuRelationsForContent(contentId, true), Times.Once);

            this.relationService.Verify(x => x.Delete(It.IsAny<IRelation>()), Times.Exactly(2));
        }

        /// <summary>
        /// Test saving of linked entities as relations.
        /// </summary>
        [Test]
        [Category("Service")]
        [Category("Parsing")]
        public void TestSaveLinkedEntitiesAsRelations()
        {
            // arrange
            var contentId = 1;
            var entities = new List<ILinkedEntity>
                               {
                                   new LinkedDocumentEntity(1500),
                                   new LinkedMediaEntity(2500)
                               };

            var docToDocRelationType = new RelationType(
                                   new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document),
                                   new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document),
                                   RelationTypes.DocumentToDocumentAlias,
                                   RelationTypes.DocumentToDocumentName)
                                   {
                                       IsBidirectional = false,
                                   };
            this.relationService.Setup(x => x.GetRelationTypeByAlias(RelationTypes.DocumentToDocumentAlias))
                .Returns(
                    docToDocRelationType);

            var docToMediaRelationType = new RelationType(
                                   new Guid(global::Umbraco.Core.Constants.ObjectTypes.Media),
                                   new Guid(global::Umbraco.Core.Constants.ObjectTypes.Document),
                                   RelationTypes.DocumentToMediaAlias,
                                   RelationTypes.DocumentToMediaName)
                                   {
                                       IsBidirectional = false,
                                   };
            this.relationService.Setup(x => x.GetRelationTypeByAlias(RelationTypes.DocumentToMediaAlias))
                .Returns(
                    docToMediaRelationType);


            var docToDocRelation = new Relation(contentId,1500, docToDocRelationType);

            this.relationService.Setup(x => x.Save(docToDocRelation));

            var docToMediaRelation = new Relation(contentId, 2500, docToMediaRelationType);

            this.relationService.Setup(x => x.Save(docToMediaRelation));


            // act
            this.service.SaveLinkedEntitiesAsRelations(contentId, entities);

            // verify
            this.relationService.Verify(x => x.GetRelationTypeByAlias(It.IsAny<string>()), Times.Exactly(2));
            this.relationService.Verify(x => x.Save(It.IsAny<Relation>()), Times.Exactly(2));
            //this.relationService.Verify(x => x.Save(docToMediaRelation), Times.Once);
        }

        /// <summary>
        /// Test parsing content
        /// </summary>
        [Test]
        [Category("Service")]
        [Category("Parsing")]
        public void TestParseContent()
        {
            // arrange
            var content = new Mock<IContent>();
            content.SetupGet(x => x.Id).Returns(1234);
            content.SetupGet(x => x.Name).Returns("Test content");

            var linkedEntities = new List<ILinkedEntity>
                                     {
                                         new LinkedDocumentEntity(456),
                                         new LinkedMediaEntity(123)
                                     };

            this.serviceMock.Setup(x => x.GetLinkedEntitesForContent(content.Object))
                .Returns(linkedEntities);

            this.serviceMock.Setup(x => x.DeleteRelationsForContent(content.Object.Id));

            this.serviceMock.Setup(x => x.SaveLinkedEntitiesAsRelations(content.Object.Id, linkedEntities));

            // act
            this.service.ParseContent(content.Object);

            // verify            
            this.serviceMock.Verify(x => x.GetLinkedEntitesForContent(content.Object), Times.Once);
            this.serviceMock.Verify(x => x.DeleteRelationsForContent(content.Object.Id), Times.Once);
            this.serviceMock.Verify(x => x.SaveLinkedEntitiesAsRelations(content.Object.Id, linkedEntities), Times.Once);
        }
    }
}
