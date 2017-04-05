namespace Our.Umbraco.Nexu.Core.ObjectResolution
{
    using System;
    using System.Collections.Generic;

    using global::Umbraco.Core.Logging;
    using global::Umbraco.Core.ObjectResolution;

    using Our.Umbraco.Nexu.Core.Interfaces;

    /// <summary>
    /// The property parser resolver.
    /// </summary>
    public class PropertyParserResolver : ManyObjectsResolverBase<PropertyParserResolver, IPropertyParser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyParserResolver"/> class.
        /// </summary>
        /// <param name="parsers">
        /// The parsers.
        /// </param>
        public PropertyParserResolver(IEnumerable<System.Type> parsers)
            : base(parsers, ObjectLifetimeScope.Application)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyParserResolver"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="parsers">
        /// The parsers.
        /// </param>
        public PropertyParserResolver(
          IServiceProvider serviceProvider,
          ILogger logger,
          List<Type> parsers)
           : base(serviceProvider, logger, parsers, ObjectLifetimeScope.Application)
        {            
        }

        /// <summary>
        /// Gets the property parsers
        /// </summary>
        public IEnumerable<IPropertyParser> Parsers => this.Values;
    }
}
