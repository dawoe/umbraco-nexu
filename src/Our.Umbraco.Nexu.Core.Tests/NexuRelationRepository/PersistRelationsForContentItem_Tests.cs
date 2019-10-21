namespace Our.Umbraco.Nexu.Core.Tests.NexuRelationRepository
{
    using System;
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Persistence;

    using Moq;

    using NPoco;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Core.Tests.Repositories;

    /// <summary>
    /// Represents the tests for the PersistRelationsForContentItem method of the NexuRelationRepository
    /// </summary>
    [TestFixture]
    public class PersistRelationsForContentItem_Tests : RepositoryBaseTest
    {
        [Test]
        public void
            When_PersistRelationsForContentItem_Called_Relations_For_Content_Item_Should_Be_Deleted_And_New_Ones_Inserted()
        {
            // arrange
            var udi = new GuidUdi("foo", Guid.NewGuid());
            var relations = Mock.Of<IEnumerable<NexuRelation>>();            

            this.UmbracoDatabaseMock.Setup(x => x.Delete<NexuRelation>(It.IsAny<Sql<ISqlContext>>()));
            this.UmbracoDatabaseMock.Setup(x => x.InsertBatch<NexuRelation>(relations, null));           

            // act
            this.Repository.PersistRelationsForContentItem(udi, relations);

            // assert
            this.UmbracoDatabaseMock.Verify(x => x.Delete<NexuRelation>(It.IsAny<Sql<ISqlContext>>()), Times.Once);
            this.UmbracoDatabaseMock.Verify(x => x.InsertBatch<NexuRelation>(It.IsAny<IEnumerable<NexuRelation>>(), null), Times.Once);            
        }

        [Test]
        public void
            When_PersistRelationsForContentItem_Called_And_Delete_Throws_Exception_No_New_Records_Should_Be_Inserted_And_Exception_Should_Be_Thrown()
        {
            // arrange
            var udi = new GuidUdi("foo", Guid.NewGuid());
            var relations = Mock.Of<IEnumerable<NexuRelation>>();

            this.UmbracoDatabaseMock.Setup(x => x.Delete<NexuRelation>(It.IsAny<Sql<ISqlContext>>())).Throws(new Exception());
            this.UmbracoDatabaseMock.Setup(x => x.InsertBatch<NexuRelation>(relations, null));

            // act
            Assert.Throws<Exception>(() => this.Repository.PersistRelationsForContentItem(udi, relations)); 

            // assert
            this.UmbracoDatabaseMock.Verify(x => x.Delete<NexuRelation>(It.IsAny<Sql<ISqlContext>>()), Times.Once);
            this.UmbracoDatabaseMock.Verify(x => x.InsertBatch<NexuRelation>(It.IsAny<IEnumerable<NexuRelation>>(), null), Times.Never);           
        }

        [Test]
        public void
            When_PersistRelationsForContentItem_Called_And_Insert_Throws_Exception_Should_Be_Thrown()
        {
            // arrange
            var udi = new GuidUdi("foo", Guid.NewGuid());
            var relations = Mock.Of<IEnumerable<NexuRelation>>();

            this.UmbracoDatabaseMock.Setup(x => x.Delete<NexuRelation>(It.IsAny<Sql<ISqlContext>>()));
            this.UmbracoDatabaseMock.Setup(x => x.InsertBatch<NexuRelation>(relations,null)).Throws(new Exception()); 

            // act
            Assert.Throws<Exception>(() => this.Repository.PersistRelationsForContentItem(udi, relations));

            // assert
            this.UmbracoDatabaseMock.Verify(x => x.Delete<NexuRelation>(It.IsAny<Sql<ISqlContext>>()), Times.Once);
            this.UmbracoDatabaseMock.Verify(x => x.InsertBatch<NexuRelation>(It.IsAny<IEnumerable<NexuRelation>>(), null), Times.Once);            
        }
    }
}
