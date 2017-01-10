namespace Our.Umbraco.Nexu.Core.ObjectResolution
{
    using System.Collections.Generic;    

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
            return pluginmanager.ResolveTypes<IPropertyParser>();
        }
    }
}
