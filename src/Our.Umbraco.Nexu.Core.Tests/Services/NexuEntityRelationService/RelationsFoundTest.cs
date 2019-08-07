namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityRelationService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ApprovalTests;
    using ApprovalTests.Reporters;
    using ApprovalTests.Reporters.ContinuousIntegration;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Services;

    using Moq;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Models;

    [TestFixture]
    [UseReporter(typeof(DiffReporter), typeof(AppVeyorReporter))]
    public class RelationsFoundTest : NexuEntityRelationServiceBaseTest
    {
        [Test]
        public void WhenRelationsFoundDisplayModelShouldBeReturned()
        {
            // arrange
            var udi = new GuidUdi("foo", Guid.NewGuid());

            var relations = this.GetRelations();

            var distinctUdis = relations.Select(x => new GuidUdi(new Uri(x.ParentUdi))).Distinct().ToList();

            // act
            var result = this.Service.GetRelationsForItem(udi);

            // assert
            Approvals.VerifyJson(JsonConvert.SerializeObject(result));

            // verify
            this.RelationRepositoryMock.Verify(x => x.GetIncomingRelationsForItem(It.IsAny<Udi>()), Times.Once);

            this.LocalizationServiceMock.Verify(x => x.GetDefaultLanguageIsoCode(), Times.Once);

            this.ContentServiceMock.Verify(x => x.GetByIds(distinctUdis.Select(r => r.Guid)), Times.Once);

        }

        /// <summary>
        /// Gets the relations
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public override List<NexuRelation> GetRelations()
        {
            var relations = new List<NexuRelation>();

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3",
                                  Culture = "nl"
            });

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/3cce2545e3ac44ecbf55a52cc5965db3",
                                  Culture = null
                              });

            relations.Add(new NexuRelation
                              {
                                  ParentUdi = "umb://document/616d0f54b8b0450ebe0d2ca4c06672a2",
                                  Culture = "en-US"
            });

            return relations;
        }
    }
}
