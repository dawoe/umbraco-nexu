(function () {
    'use strict';

    function RelationCheckResource($http, umbRequestHelper) {

        var apiUrl = Umbraco.Sys.ServerVariables.Nexu.RelationCheckApi;

        var resource = {
            getIncomingLinks: getIncomingLinks
        };

        return resource;

        function getIncomingLinks(udi) {

            return umbRequestHelper.resourcePromise(
                $http.get(apiUrl + 'GetIncomingLinks?udi=' + udi),
                'Failed to get incoming links'
            );
        };

    }

    angular.module('umbraco.resources').factory('Our.Umbraco.Nexu.Resources.RelationCheckResource', RelationCheckResource);

})();