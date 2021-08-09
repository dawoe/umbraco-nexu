using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.Nexu.Common.Interfaces.Models;
using Our.Umbraco.Nexu.Common.Models;
using Umbraco.Core;

namespace Our.Umbraco.Nexu.Parsers.Core
{
    public class MediaPicker3 : IPropertyValueParser
    {
        public bool IsParserFor(string propertyEditorAlias) =>
            propertyEditorAlias.Equals("Umbraco.MediaPicker3");

        public IEnumerable<IRelatedEntity> GetRelatedEntities(string value)
        {
            var entities = new List<IRelatedEntity>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    var jsonValues = JsonConvert.DeserializeObject<JArray>(value);

                    foreach (var item in jsonValues)
                    {
                        if (item["mediaKey"] != null)
                        {
                            var mediaKey = item.Value<string>("mediaKey");

                            var udi = Udi.Create("media", Guid.Parse(mediaKey));

                            entities.Add(new RelatedMediaEntity
                            {
                                RelatedEntityUdi = udi,
                            });
                        }
                    }
                }
                catch
                {
                    // swallow it
                }
            }

            return entities;
        }
    }
}
