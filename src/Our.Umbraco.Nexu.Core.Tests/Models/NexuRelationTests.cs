namespace Our.Umbraco.Nexu.Core.Tests.Models
{
    using System;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// Represents nexu relation model tests.
    /// </summary>
    [TestFixture]
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
            var result = new NexuRelation();

            // assert
            Assert.That(result.Udi.EntityType == "nexurelation");
            Assert.That(result.Udi.ToString() == $"umb://nexurelation/{result.Id.ToString("N")}");
        }
    }
}
