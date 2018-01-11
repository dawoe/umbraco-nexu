namespace Our.Umbraco.Nexu.Parsers.GridEditorParsers.Community
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Umbraco.Core;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Our.Umbraco.Nexu.Core.Interfaces;
    using Our.Umbraco.Nexu.Core.Models;

    /// <summary>
    /// The le blender grid editor parser.
    /// </summary>
    public class LeBlenderGridEditorParser : IGridEditorParser
    {
        /// <summary>
        /// Check if it is a parser for a speficic editor
        /// </summary>
        /// <param name="editorview">
        /// The editor view alias as defined in grid config
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsParserFor(string editorview)
        {
            return editorview.Equals("/App_Plugins/LeBlender/editors/leblendereditor/LeBlendereditor.html");
        }

        /// <summary>
        /// Gets the linked entities for this editor
        /// </summary>
        /// <param name="value">
        /// The grid editor value
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<ILinkedEntity> GetLinkedEntities(string value)
        {            
            if (string.IsNullOrEmpty(value))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            return Enumerable.Empty<ILinkedEntity>();

            //var linkedEntities = new List<ILinkedEntity>();

            //try
            //{
            //    var jsonValue = JsonConvert.DeserializeObject<JArray>(value);

            //    ///get any value that's a udi
            //    var props = jsonValue.Descendants().OfType<JProperty>().Where(p => p.Name == "value" && p.Value.ToString().StartsWith("umb://")).ToList();

            //    var contentService = ApplicationContext.Current.Services.ContentService;
            //    var mediaService = ApplicationContext.Current.Services.MediaService;

            //    foreach (JProperty property in props)
            //    {
            //        var propvalue = property.Value.ToString();
            //        var guidUdi = Udi.Parse(propvalue) as GuidUdi;

            //        if (guidUdi != null)
            //        {
            //            if (propvalue.StartsWith("umb://document/"))
            //            {
            //                var node = contentService.GetById(guidUdi.Guid);
            //                if (node != null)
            //                {
            //                    linkedEntities.Add(new LinkedDocumentEntity(node.Id));                                
            //                }
            //            }
            //            else if (propvalue.StartsWith("umb://media/"))
            //            {
            //                var media = mediaService.GetById(guidUdi.Guid);
            //                if (media != null)
            //                {
            //                    linkedEntities.Add(new LinkedMediaEntity(media.Id));                                
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{

            //}

            //return linkedEntities;
        }
    }
}
