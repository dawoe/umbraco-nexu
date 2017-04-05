namespace Our.Umbraco.Nexu.Core.ObjectResolution
{
    using System.Collections.Generic;
    using System.Reflection;

    using Interfaces;

    using global::Umbraco.Core;

    /// <summary>
    /// The plugin manager extensions methods
    /// </summary>
    internal static class PluginManagerExtensions
    {
        /// <summary>
        /// Resolves the property parsers
        /// </summary>
        /// <param name="pluginmanager">
        /// The plugin manager.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        internal static IEnumerable<System.Type> ResolvePropertyParsers(this PluginManager pluginmanager)
        {
            var assembly = Assembly.Load("Our.Umbraco.Nexu.Parsers");
            return pluginmanager.ResolveTypes<IPropertyParser>(specificAssemblies: new List<Assembly> { assembly });
        }

        /// <summary>
        /// Gets all grid editor parsers
        /// </summary>
        /// <param name="pluginmanager">
        /// The pluginmanager.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        internal static IEnumerable<System.Type> ResolveGridEditorParsers(this PluginManager pluginmanager)
        {
            var assembly = Assembly.Load("Our.Umbraco.Nexu.Parsers");
            return pluginmanager.ResolveTypes<IGridEditorParser>(specificAssemblies: new List<Assembly> { assembly });
        }
    }
}
