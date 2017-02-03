namespace Our.Umbraco.Nexu.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    /// <summary>
    /// The activator service provider.
    /// </summary>
    internal class ActivatorServiceProvider : IServiceProvider
    {
        /// <summary>
        /// The get service.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }        
    }
}
