namespace Our.Umbraco.Nexu.Resolvers
{
    using System.Collections.Generic;

    using Our.Umbraco.Nexu.Core.Interfaces;

    using global::Umbraco.Core.ObjectResolution;

    /// <summary>
    /// The property parser resolver.
    /// </summary>
    internal class PropertyParserResolver : ManyObjectsResolverBase<PropertyParserResolver, IPropertyParser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyParserResolver"/> class.
        /// </summary>
        /// <param name="parsers">
        /// The parsers.
        /// </param>
        internal PropertyParserResolver(IEnumerable<System.Type> parsers)
            : base(parsers, ObjectLifetimeScope.Application)
        {
        }

        /// <summary>
        /// Gets the property parsers
        /// </summary>
        public IEnumerable<IPropertyParser> Parsers => this.Values;
    }
}
