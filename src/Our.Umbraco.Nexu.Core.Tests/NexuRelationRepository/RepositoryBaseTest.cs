namespace Our.Umbraco.Nexu.Core.Tests.Repositories
{
    using System;
    using System.Data;

    using global::Umbraco.Core.Persistence;
    using global::Umbraco.Core.Persistence.Mappers;
    using global::Umbraco.Core.Persistence.SqlSyntax;
    using global::Umbraco.Core.Scoping;

    using Moq;

    using NPoco;

    using NUnit.Framework;

    using Our.Umbraco.Nexu.Common.Models;
    using Our.Umbraco.Nexu.Core.Repositories;

    /// <summary>
    /// Represents a base class for repository tests
    /// </summary>
    public abstract class RepositoryBaseTest
    {
        /// <summary>
        /// Gets or sets the scope provider mock.
        /// </summary>
        internal Mock<IScopeProvider> ScopeProviderMock { get; set; }

        /// <summary>
        /// Gets or sets the umbraco database mock.
        /// </summary>
        internal Mock<IUmbracoDatabase> UmbracoDatabaseMock { get; set; }

        /// <summary>
        /// Gets or sets the scope mock.
        /// </summary>
        internal Mock<IScope> ScopeMock { get; set; }

        /// <summary>
        /// Gets or sets the transaction mock.
        /// </summary>
        internal Mock<ITransaction> TransactionMock { get; set; }

        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        internal NexuRelationRepository Repository { get; set; }

        /// <summary>
        /// Sets up mocks used in derived tests
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
            this.UmbracoDatabaseMock = new Mock<IUmbracoDatabase>();

            var sqlSyntaxMock = new Mock<ISqlSyntaxProvider>();
            sqlSyntaxMock.Setup(x => x.GetQuotedTableName(It.IsAny<string>())).Returns((string x) => x);
            sqlSyntaxMock.Setup(x => x.GetQuotedColumnName(It.IsAny<string>())).Returns((string x) => x);

            var pocoMappers = new NPoco.MapperCollection { new PocoMapper() };
            var pocoDataFactory = new FluentPocoDataFactory((type, iPocoDataFactory) => new PocoDataBuilder(type, pocoMappers).Init());

            this.TransactionMock = new Mock<ITransaction>();

            this.UmbracoDatabaseMock.Setup(x => x.GetTransaction()).Returns(this.TransactionMock.Object);

            var sqlContextMock = new Mock<ISqlContext>();
            sqlContextMock.SetupGet(x => x.SqlSyntax).Returns(sqlSyntaxMock.Object);
            sqlContextMock.SetupGet(x => x.PocoDataFactory).Returns(pocoDataFactory);
          
            this.ScopeMock = new Mock<IScope>();
            this.ScopeMock.SetupGet(x => x.Database).Returns(this.UmbracoDatabaseMock.Object);
            this.ScopeMock.Setup(x => x.Complete());
            this.ScopeMock.SetupGet(x => x.SqlContext).Returns(sqlContextMock.Object);
            
            this.ScopeProviderMock = new Mock<IScopeProvider>();

         

            this.ScopeProviderMock
                .Setup(
                    x => x.CreateScope(
                        IsolationLevel.Unspecified,
                        RepositoryCacheMode.Unspecified,
                        null,
                        null,
                        false,
                        true)).Returns(this.ScopeMock.Object);

            

            this.Repository = new NexuRelationRepository(this.ScopeProviderMock.Object);
        }       
    }
}
