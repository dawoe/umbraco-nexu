namespace Our.Umbraco.Nexu.Core.Composing.Composers
{
    using System.Collections.Generic;
    using System.Reflection;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Composing;

    using Our.Umbraco.Nexu.Common.Interfaces.Models;

    /// <summary>
    /// Represents the collections composer.
    /// </summary>
    [ComposeAfter(typeof(NexuComposer))]
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class CollectionsComposer : IUserComposer
    {
        /// <inheritdoc />
        public void Compose(Composition composition)
        {
            var assembly = Assembly.Load("Our.Umbraco.Nexu.Parsers");
            composition.PropertyValueParsers().Append(
                composition.TypeLoader.GetTypes<IPropertyValueParser>(
                    specificAssemblies: new List<Assembly> { assembly }));
        }
    }
}
