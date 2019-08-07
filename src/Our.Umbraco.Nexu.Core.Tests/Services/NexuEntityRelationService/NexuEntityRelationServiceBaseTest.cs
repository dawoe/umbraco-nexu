namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityRelationService
{
    using System;
    using System.Collections.Generic;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Models;
    using global::Umbraco.Core.Services;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Repositories;
    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    /// Represents nexu entity relation service base test.
    /// </summary>
    public abstract class NexuEntityRelationServiceBaseTest
    {
        /// <summary>
        /// Gets or sets the relation repository mock.
        /// </summary>
        internal Mock<IRelationRepository> RelationRepositoryMock { get; set; }

        /// <summary>
        /// Gets or sets the localization service mock.
        /// </summary>
        internal Mock<ILocalizationService> LocalizationServiceMock { get; set; }

        /// <summary>
        /// Gets or sets the content service mock.
        /// </summary>
        internal Mock<IContentService> ContentServiceMock { get; set; }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        internal NexuEntityRelationService Service { get; set; }

        /// <summary>
        /// Sets up the test
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
            this.RelationRepositoryMock = new Mock<IRelationRepository>();
            this.LocalizationServiceMock = new Mock<ILocalizationService>();
            this.ContentServiceMock = new Mock<IContentService>();

            this.LocalizationServiceMock.Setup(x => x.GetDefaultLanguageIsoCode()).Returns("en-US");

            var relations = this.GetRelations();

            this.RelationRepositoryMock.Setup(x => x.GetIncomingRelationsForItem(It.IsAny<Udi>()))
                .Returns(relations);
            this.ContentServiceMock.Setup(x => x.GetByIds(It.IsAny<IEnumerable<Guid>>())).Returns(this.GetContentItems);

            this.Service = new NexuEntityRelationService(this.RelationRepositoryMock.Object, this.LocalizationServiceMock.Object, this.ContentServiceMock.Object);
        }

        /// <summary>
        /// Tears down the test
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
            this.RelationRepositoryMock = null;
            this.ContentServiceMock = null;
            this.LocalizationServiceMock = null;
            this.Service = null;
        }

        /// <summary>
        /// Gets the relations
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<NexuRelation> GetRelations()
        {
            return new List<NexuRelation>();
        }

        /// <summary>
        /// Gets the content items
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<IContent> GetContentItems()
        {
            return new List<IContent>();
        }
    }
}
