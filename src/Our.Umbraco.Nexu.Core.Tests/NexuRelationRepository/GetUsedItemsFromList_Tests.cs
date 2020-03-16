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
    public class GetUsedItemsFromList_Tests : RepositoryBaseTest
    {
        [Test]
        public void
            When_GetGetUsedItemsFromList_Testsm_Called_We_Should_Fetch_All_Items_Where_ChildUdi_Matches_The_Udis_In_TheList()
        {
            // arrange
            var fooUdi = new GuidUdi("foo", Guid.NewGuid());
            var barUdi = new GuidUdi("bar", Guid.NewGuid());
            var listUdis = new List<Udi>
                               {
                                   fooUdi,
                                   barUdi
                               };            
            var relations = Mock.Of<List<NexuRelation>>();

            Sql actualSql = null;

            this.UmbracoDatabaseMock.Setup(x => x.Fetch<NexuRelation>(It.IsAny<Sql<ISqlContext>>())).Callback((Sql sql) =>
                {
                    actualSql = sql;
                }).Returns(relations);


            // act
            var result = this.Repository.GetUsedItemsFromList(listUdis);

            // assert 
            Assert.Multiple(
                () =>
                    {
                        Assert.IsNotNull(result);
                        Assert.That(actualSql.SQL == "WHERE (Nexu_Relations.child_udi IN (@0,@1))");
                        Assert.That(actualSql.Arguments[0].ToString() == fooUdi.ToString());
                        Assert.That(actualSql.Arguments[1].ToString() == barUdi.ToString());
                    });
          

            this.UmbracoDatabaseMock.Verify(x => x.Fetch<NexuRelation>(It.IsAny<Sql<ISqlContext>>()), Times.Once);
        }
    }
}
