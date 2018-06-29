angular.module('umbraco.services').config([
   '$httpProvider',
   function ($httpProvider) {

       $httpProvider.interceptors.push(['$q','$injector', 'notificationsService', function ($q,$injector, notificationsService) {
           return {
               'request': function (request) {
                   
                   if (Umbraco.Sys.ServerVariables.application.version) {

                       var versionArray = Umbraco.Sys.ServerVariables.application.version.split('.');

                       var version = {
                           "major": parseInt(versionArray[0]),
                           "minor": parseInt(versionArray[1]),
                           "patch": parseInt(versionArray[2])
                       };

                       // Redirect any requests to built in content delete to our custom delete
                       if (request.url.indexOf("views/content/delete.html") === 0) {
                           request.url = '/App_Plugins/Nexu/views/content-delete.html';
                       }

                       // Redirect any requests to built in media delete to our custom delete
                       if (request.url.indexOf("views/media/delete.html") === 0) {
                           request.url = '/App_Plugins/Nexu/views/media-delete.html';
                       }

                       // Redirect requests to unpublish confirmation to our own (v7.12+)
                       if (request.url.indexOf("views/common/notifications/confirmunpublish.html") === 0) {
                           request.url = '/App_Plugins/Nexu/views/confirmunpublish.html';
                       }

                       if (version.major >= 7 && version.minor < 12) {

                           var unpublishUrlRegex =
                               /^\/umbraco\/backoffice\/UmbracoApi\/Content\/PostUnPublish\?id=(\d*)$/i;

                           // check if unpublished api call is made
                           if (unpublishUrlRegex.test(request.url)) {
                               // get the id from the url
                               var id = unpublishUrlRegex.exec(request.url)[1];

                               // get nexuResource
                               var nexuService = $injector.get('Our.Umbraco.Nexu.Resource');

                               // create deferred request
                               var deferred = $q.defer();

                               // get incoming links
                               nexuService.getIncomingLinks(id)
                                   .then(function(result) {
                                       // if incoming links are found, intercept unpublish and show custom notification
                                       if (result.data.length > 0) {
                                           notificationsService.add({
                                               // the path of our custom notification view
                                               view: "/App_Plugins/Nexu/views/unpublish-confirmation.html",
                                               // arguments object we want to pass to our custom notification
                                               args: {
                                                   links: result.data,
                                                   deferredPromise: deferred,
                                                   originalRequest: request,
                                                   descendantsHaveLinks: false
                                               }
                                           });
                                       } else {
                                           nexuService.checkDescendants(id, false).then(function(result) {
                                               if (result.data === 'true') {
                                                   notificationsService.add({
                                                       // the path of our custom notification view
                                                       view: "/App_Plugins/Nexu/views/unpublish-confirmation.html",
                                                       // arguments object we want to pass to our custom notification
                                                       args: {
                                                           links: [],
                                                           deferredPromise: deferred,
                                                           originalRequest: request,
                                                           descendantsHaveLinks: true
                                                       }
                                                   });
                                               } else {
                                                   // execute request as normal
                                                   deferred.resolve(request);
                                               }
                                           });

                                       }
                                   });

                               // return deferred promise
                               return deferred.promise;

                           }
                       }

                       
                   }

                   return request || $q.when(request);
               }
           };
       }]);

   }]);