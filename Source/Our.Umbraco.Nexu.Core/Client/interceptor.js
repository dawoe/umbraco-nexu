angular.module('umbraco.services').config([
   '$httpProvider',
   function ($httpProvider) {

       $httpProvider.interceptors.push(function ($q) {
           return {
               'request': function (request) {

                   // Redirect any requests the built in content delete to our custom delete
                   if (request.url === "views/content/delete.html") {
                       request.url = '/App_Plugins/Nexu/views/content-delete.html';
                   }
                       

                   return request || $q.when(request);
               }
           };
       });

   }]);