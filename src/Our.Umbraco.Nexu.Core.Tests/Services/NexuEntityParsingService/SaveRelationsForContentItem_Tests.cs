namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System;
    using System.Collections.Generic;
    using System.Web.WebSockets;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents the tests for SaveRelationsForContentItem method on the NexuEntityParsingService
    /// </summary>
    [TestFixture]
    public class SaveRelationsForContentItem_Tests : NexuEntityParsingServiceBaseTest
    {
        [Test]
        public void When_SaveRelationsForContentItem_It_Should_Call_Repository_Method_With_Correct_Parameters()
        {
            // arrange
            var guid = Guid.NewGuid();

            var content = Mock.Of<IContent>();
            content.Blueprint = false;
            content.Key = guid;

            var udi = content.GetUdi();

            var relations = new List<NexuRelation>();

            this.RelationRepositoryMock.Setup(x => x.PersistRelationsForContentItem(udi, relations));

            // act
            this.Service.SaveRelationsForContentItem(content, relations);

            // assert
            this.RelationRepositoryMock.Verify(x => x.PersistRelationsForContentItem(udi, relations));
        }
    }
}
