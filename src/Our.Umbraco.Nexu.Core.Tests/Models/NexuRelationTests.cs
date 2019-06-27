namespace Our.Umbraco.Nexu.Core.Tests.Models
{
    using System;

    using ApprovalTests;
    using ApprovalTests.Reporters;
    using ApprovalTests.Reporters.ContinuousIntegration;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Models;

    /// <summary>
    /// Represents nexu relation model tests.
    /// </summary>
    [TestFixture]    
    [UseReporter(typeof(DiffReporter), typeof(AppVeyorReporter))]
    public class NexuRelationTests
    {
        [Test]
        public void When_NexuRelation_Instance_Is_Created_Id_Should_Be_A_Guid()
        {
            // act
            var result = new NexuRelation();

            // assert
            Assert.That(result.Id != Guid.Empty);
        }

        [Test]
        public void Nexu_Relation_Udi_Should_Match_Id()
        {
            // act
            var result = new NexuRelation
                             {
                                 Id= new Guid("9a935375-6c89-4dda-ae83-b9207e19a3ee")
                             };

            // assert
            Approvals.VerifyJson(JsonConvert.SerializeObject(result));           
        }
    }
}
