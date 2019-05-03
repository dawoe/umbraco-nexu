namespace Our.Umbraco.Nexu.Core.Tests.Services.NexuEntityParsingService
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Logging;

    using Moq;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;
    using Our.Umbraco.Nexu.Common.Interfaces.Repositories;
    using Our.Umbraco.Nexu.Core.Composing.Collections;
    using Our.Umbraco.Nexu.Core.Services;

    /// <summary>
    /// Represents nexu entity parsing service base test.
    /// </summary>
    public abstract class NexuEntityParsingServiceBaseTest
    {
        /// <summary>
        /// The service instance used in all tests
        /// </summary>
        internal NexuEntityParsingService Service { get; set; }

        /// <summary>
        /// The logger mock.
        /// </summary>
        internal Mock<ILogger> LoggerMock { get; set; }

        /// <summary>
        /// The relation repository mock.
        /// </summary>
        internal Mock<IRelationRepository> RelationRepositoryMock { get; set; }

        /// <summary>
        /// The setup that is run for all tests
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.LoggerMock = new Mock<ILogger>();
            this.RelationRepositoryMock = new Mock<IRelationRepository>();

            var serviceMock = new Mock<NexuEntityParsingService>(
                                  new PropertyValueParserCollection(this.GetParsers()), this.LoggerMock.Object, this.RelationRepositoryMock.Object)
                                  {
                                      CallBase = true
                                  };


            this.Service = serviceMock.Object;
        }

        public virtual List<IPropertyValueParser> GetParsers()
        {
            return new List<IPropertyValueParser>();
        }
    }
}
