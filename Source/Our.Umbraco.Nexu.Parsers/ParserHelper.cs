namespace Our.Umbraco.Nexu.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The parser helper.
    /// </summary>
    public static class ParserHelper
    {
        /// <summary>
        /// The get linked entities from csv string.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public static IEnumerable<T> GetLinkedEntitiesFromCsvString<T>(string input) where T : ILinkedEntity
        {
            var idlist = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var entities = new List<T>();

            idlist.ForEach(
                x =>
                {
                    var attemptId = x.TryConvertTo<int>();

                    if (attemptId.Success)
                    {
                        entities.Add((T)Activator.CreateInstance(typeof(T), attemptId.Result));
                    }
                });

            return entities;
        }
    }
}
