namespace Our.Umbraco.Nexu.Parsers.PropertyParsers.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    using global::Umbraco.Core;
    using global::Umbraco.Core.Configuration;
    using global::Umbraco.Core.Configuration.Grid;
    using global::Umbraco.Core.IO;
    using global::Umbraco.Core.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.ObjectResolution;

    /// <summary>
    /// The grid parser.
    /// </summary>
    public class GridParser : IPropertyParser
    {
        /// <summary>
        /// The grid config.
        /// </summary>
        private readonly IGridConfig gridConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridParser"/> class.
        /// </summary>
        public GridParser()
        {
            this.gridConfig = UmbracoConfig.For.GridConfig(
                       ApplicationContext.Current.ProfilingLogger.Logger,
                       ApplicationContext.Current.ApplicationCache.RuntimeCache,
                       new DirectoryInfo(ParserHelper.MapPath(SystemDirectories.AppPlugins)),
                       new DirectoryInfo(ParserHelper.MapPath(SystemDirectories.Config)),
                       HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridParser"/> class.
        /// </summary>
        /// <param name="gridConfig">
        /// The grid config.
        /// </param>
        public GridParser(IGridConfig gridConfig)
        {
            this.gridConfig = gridConfig;
        }

        /// <summary>
        /// Check if it's a parser for a data type definition
        /// </summary>
        /// <param name="dataTypeDefinition">
        /// The data type definition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
        {
            return
                dataTypeDefinition.PropertyEditorAlias.Equals(global::Umbraco.Core.Constants.PropertyEditors.GridAlias);
        }

        /// <summary>
        /// Gets the linked entites from the property value
        /// </summary>
        /// <param name="propertyValue">
        /// The property value.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue?.ToString()))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }            

            var entities = new List<ILinkedEntity>();

            var jsonValue = JsonConvert.DeserializeObject<JObject>(propertyValue.ToString());

            var sections = this.GetArray(jsonValue, "sections");

            foreach (var section in sections.Cast<JObject>())
            {
                var rows = this.GetArray(section, "rows");
                foreach (var row in rows.Cast<JObject>())
                {
                    var areas = this.GetArray(row, "areas");
                    foreach (var area in areas.Cast<JObject>())
                    {
                        var controls = this.GetArray(area, "controls");
                        foreach (var control in controls.Cast<JObject>())
                        {
                            var editor = control.Value<JObject>("editor");
                            if (editor != null)
                            {
                                var alias = editor.Value<string>("alias");
                                if (alias.IsNullOrWhiteSpace() == false)
                                {
                                    //find the alias in config
                                    var found = gridConfig.EditorsConfig.Editors.FirstOrDefault(x => x.Alias == alias);
                                    if (found != null)
                                    {
                                        // get the parser for this editor
                                        var parser =
                                            GridEditorParserResolver.Current.Parsers.FirstOrDefault(
                                                x => x.IsParserFor(found.View));

                                        if (parser != null)
                                        {
                                            // parse the value
                                            var value = control["value"].ToString();
                                            entities.AddRange(parser.GetLinkedEntities(value));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return entities;
        }

        private JArray GetArray(JObject obj, string propertyName)
        {
            JToken token;
            if (obj.TryGetValue(propertyName, out token))
            {
                var asArray = token as JArray;
                return asArray ?? new JArray();
            }
            return new JArray();
        }
    }
}
