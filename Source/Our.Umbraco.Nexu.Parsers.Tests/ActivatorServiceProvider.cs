namespace Our.Umbraco.Nexu.Parsers.Tests
{
    using System;

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
