namespace Our.Umbraco.Nexu.Core.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// The NexuService interface.
    /// </summary>
    public interface INexuService
    {
        /// <summary>
        /// Sets up the needed the relation types
        /// </summary>
        void SetupRelationTypes();

        /// <summary>
        /// Gets all property parsrs
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<IPropertyParser> GetAllPropertyParsers();
    }
}