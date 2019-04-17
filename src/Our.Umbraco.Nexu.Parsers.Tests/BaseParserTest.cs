namespace Our.Umbraco.Nexu.Parsers.Tests
{
    using System.Collections.Generic;

    using global::Umbraco.Core.Models;

    /// <summary>
    /// The base parser test.
    /// </summary>
    public abstract class BaseParserTest
    {
        /// <summary>
        /// Creates a property for the editor alias
        /// </summary>
        /// <param name="editorAlias">
        /// The editor alias.
        /// </param>
        /// <returns>
        /// The <see cref="Property"/>.
        /// </returns>
        public Property CreateProperty(string editorAlias)
        {
            var propertyType =
                new PropertyType(editorAlias, ValueStorageType.Ntext)
                    {
                        Variations = ContentVariation.Culture
                    };

            return new Property(propertyType);
        }

        /// <summary>
        /// Creates create property with values.
        /// </summary>
        /// <param name="editorAlias">
        /// The editor alias.
        /// </param>
        /// <param name="cultureValues">
        /// The culture values dictionary. the key is the culture
        /// </param>
        /// <returns>
        /// The <see cref="Property"/>.
        /// </returns>
        public Property CreatePropertyWithValues(string editorAlias, Dictionary<string, object> cultureValues)
        {
            var propertyWithValues = this.CreateProperty(editorAlias);

            foreach (var key in cultureValues.Keys)
            {
                propertyWithValues.SetValue(cultureValues[key], key);
            }

            return propertyWithValues;
        }
    }
}
