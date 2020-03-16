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

    [TestFixture]
    public class GetIncomingRelationsForItem_Tests : RepositoryBaseTest
    {
        [Test]
        public void
            When_GetIncomingRelationsForItem_Called_We_Should_Fetch_All_Items_Where_ChildUdi_Matches_The_Item_Udi()
        {
            // arrange
            var udi = new GuidUdi("foo", Guid.NewGuid());
            var relations = Mock.Of<List<NexuRelation>>();

            Sql actualSql = null;

            this.UmbracoDatabaseMock.Setup(x => x.Fetch<NexuRelation>(It.IsAny<Sql<ISqlContext>>())).Callback((Sql sql) =>
                {
                    actualSql = sql;
                }).Returns(relations);
           

            // act
            var result = this.Repository.GetIncomingRelationsForItem(udi);

            // assert 
            Assert.IsNotNull(result);
            Assert.That(actualSql.SQL == "WHERE ((Nexu_Relations.child_udi = @0))");
            Assert.That(actualSql.Arguments[0].ToString() == udi.ToString());

            this.UmbracoDatabaseMock.Verify(x => x.Fetch<NexuRelation>(It.IsAny<Sql<ISqlContext>>()), Times.Once);
        }

    }
}
