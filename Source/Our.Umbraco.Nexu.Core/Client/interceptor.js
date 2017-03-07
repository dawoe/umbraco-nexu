angular.module('umbraco.services').config([
   '$httpProvider',
   function ($httpProvider) {

       $httpProvider.interceptors.push(['$q','$injector', 'notificationsService', function ($q,$injector, notificationsService) {
           return {
               'request': function (request) {

                   // Redirect any requests the built in content delete to our custom delete
                   if (request.url === "views/content/delete.html") {
                       request.url = '/App_Plugins/Nexu/views/content-delete.html';                      
                   }

                   var unpublishUrlRegex = /^\/umbraco\/backoffice\/UmbracoApi\/Content\/PostUnPublish\?id=(\d*)$/i;

                   // check if unpublished api call is made
                   if(unpublishUrlRegex.test(request.url)) {                                        
                       // get the id from the url
                       var id = unpublishUrlRegex.exec(request.url)[1];

                       // get nexuResource 
                       var nexuService = $injector.get('Our.Umbraco.Nexu.Resource');

                       // create deferred request
                       var deferred = $q.defer();                     

                       // get incoming links
                       nexuService.getIncomingLinks(id)
                           .then(function(result) {
                               // if incoming links are found, cancel unpublish
                               if (result.data.length > 0) {
                                   notificationsService.error('Intercepted unpublish');

                                   // cancel request
                                   deferred.reject(request);
                               } else {
                                   // execute request as normal
                                   deferred.resolve(request);
                               }
                           });

                       // return deferred promise
                       return deferred.promise;
                      
                   }
                       
                   return request || $q.when(request);
               }
           };
       }]);

   }]);