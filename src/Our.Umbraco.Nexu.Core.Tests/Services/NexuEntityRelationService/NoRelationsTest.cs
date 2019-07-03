namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityRelationService
{
    using System;

    using ApprovalTests;
    using ApprovalTests.Reporters;
    using ApprovalTests.Reporters.ContinuousIntegration;

    using global::Umbraco.Core;

    using Moq;

    using Newtonsoft.Json;

    using NUnit.Framework;

    /// <summary>
    /// The no relations test.
    /// </summary>
    [TestFixture]
    [UseReporter(typeof(DiffReporter), typeof(AppVeyorReporter))]
    public class NoRelationsTest : NexuEntityRelationServiceBaseTest
    {
        [Test]
        public void NoRelationsShouldReturnEmptyResult()
        {
            // arrange
            var udi = new GuidUdi("foo", Guid.NewGuid());

            // act
            var result = this.Service.GetRelationsForItem(udi);

            // assert
            Approvals.VerifyJson(JsonConvert.SerializeObject(result));

            // verify
            this.RelationRepositoryMock.Verify(x => x.GetIncomingRelationsForItem(It.IsAny<Udi>()), Times.Once);

            this.LocalizationServiceMock.VerifyNoOtherCalls();
            this.ContentServiceMock.VerifyNoOtherCalls();
        }
    }
}
