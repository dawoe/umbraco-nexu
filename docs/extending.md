# Extending #

You can extend Nexu by writing custom property editor or grid editor parsers

## Custom property editor parsers ##

You can write your own property editor parser by creating a class that implements the interface ` Our.Umbraco.Nexu.Core.Interfaces.IPropertyParser`

This interface requires you to implement 2 methods

`bool IsParserFor(IDataTypeDefinition dataTypeDefinition) `: this method will receive the datatype defintion of the property bein parsed. You need to return a boolean indicating that this is a parser for the datatype definition.

`IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)` : this method will receive the property value that you need to parse for internal links. There are 2 different types of implementations of ILinkedEntity that you can use : `Our.Umbraco.Nexu.Core.Models.LinkedDocumentEntity` and `Our.Umbraco.Nexu.Core.Models.LinkedMediaEntity`

### Example property property parser ###

    public class RelatedLinksParser : IPropertyParser
    {
	    public bool IsParserFor(IDataTypeDefinition dataTypeDefinition)
	    {
		    return
		    dataTypeDefinition.PropertyEditorAlias.Equals(
		    global::Umbraco.Core.Constants.PropertyEditors.RelatedLinksAlias);
	    }
    
    
	    public IEnumerable<ILinkedEntity> GetLinkedEntities(object propertyValue)
	    {
		    if (string.IsNullOrEmpty(propertyValue?.ToString()))
		    {
		    	return Enumerable.Empty<ILinkedEntity>();
		    }
	    
		    var entities = new List<ILinkedEntity>();
		    
		    var jsonValue = JsonConvert.DeserializeObject<List<object>>(propertyValue.ToString());
		    
		    foreach (JObject item in jsonValue)
		    {
			    if (this.IsInternalLink(item))
			    {
			    	var attemptId = this.GetInternalId(item).TryConvertTo<int>();
			    
				    if (attemptId.Success)
				    {
				    	entities.Add(new LinkedDocumentEntity(attemptId.Result));
				    }
		    	}
		    }
		    
		    return entities;
	    }
	       
	    private bool IsInternalLink(JObject item)
	    {
		    bool isInternal = false;
		    
		    var isInternalProperty = item["isInternal"];
		    
		    if (isInternalProperty != null)
		    {
		    	isInternal = isInternalProperty.ToObject<bool>();
		    }
		    
		    return isInternal;
	    }
	    
	    private string GetInternalId(JObject item)
	    {
		    var internalIdProperty = item["internal"];
		    
		    return internalIdProperty?.ToObject<string>();
	    }
    }
    

## Custom grid editor parsers ##

You can create your own grid editor parser by creating a class that implements the interface `Our.Umbraco.Nexu.Core.Interfaces.IGridEditorParser`

This interface requires you to implement 2 methods

`bool IsParserFor(string editorview)` : this will pass in the name of the editor view as found in the grid.editors.config.js file located in the config editor. It needs to return a boolean indicating if this is a parser for the grid editor view.

`IEnumerable<ILinkedEntity> GetLinkedEntities(string value)` :  this method will receive the grid editor value that you need to parse for internal links. There are 2 different types of implementations of ILinkedEntity that you can use : `Our.Umbraco.Nexu.Core.Models.LinkedDocumentEntity` and `Our.Umbraco.Nexu.Core.Models.LinkedMediaEntity`

### Example grid editor parser ###

	public class MediaGridEditorParser : IGridEditorParser
    {
        
        public bool IsParserFor(string editorview)
        {
            return editorview.Equals("media");
        }
        
        public IEnumerable<ILinkedEntity> GetLinkedEntities(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Enumerable.Empty<ILinkedEntity>();
            }

            var linkedEntities = new List<ILinkedEntity>();

            var jsonValue = JsonConvert.DeserializeObject<JObject>(value);

            var attemptId = jsonValue["id"].ToString().TryConvertTo<int>();

            if (attemptId.Success)
            {
                linkedEntities.Add(new LinkedMediaEntity(attemptId.Result));
            }

            return linkedEntities;
        }
    }


## Registering custom parsers ##

Registering your custom parsers is must be done in the `ApplicationStarting` event of a class inheriting from `Umbraco.Core.ApplicationEventHandler`

	public class UmbracoStartup : ApplicationEventHandler
    {
		protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{	
			// register custom nexu property parsers
            Our.Umbraco.Nexu.Core.ObjectResolution.PropertyParserResolver.Current.AddType<CustomPropertyParser>();
			
			// register custom nexu grid editor parsers
			Our.Umbraco.Nexu.Core.ObjectResolution.GridEditorParserResolver.Current.AddType<CustomGridEditorParser>();
		}
    }


## Removing built-in parsers ##

Removing your buil-int parsers is must be done in the `ApplicationStarting` event of a class inheriting from `Umbraco.Core.ApplicationEventHandler`

	public class UmbracoStartup : ApplicationEventHandler
    {
		protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{	
			// removing nexu property parsers
            Our.Umbraco.Nexu.Core.ObjectResolution.PropertyParserResolver.Current.RemoveType<CustomPropertyParser>();
			
			// removing nexu grid editor parsers
			Our.Umbraco.Nexu.Core.ObjectResolution.GridEditorParserResolver.Current.RemoveType<CustomGridEditorParser>();
		}
    }
