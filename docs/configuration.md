# Configuration #

## Prevent deleting (v2.2.0+) ##

To prevent that editors can delete a content or media item that is linked to from other items you can add the following key to your appSettings in the web.config

    <add key="nexu:PreventDelete" value="true" />

Removing this setting or setting it to false allows editors to delete the used item

## Prevent unpublishing (v2.2.0+) ##

To prevent that editors can unpublish a content  item that is linked to from other items you can add the following key to your appSettings in the web.config

    <add key="nexu:PreventUnpublish" value="true" />

Removing this setting or setting it to false allows editors to unpublish the used item