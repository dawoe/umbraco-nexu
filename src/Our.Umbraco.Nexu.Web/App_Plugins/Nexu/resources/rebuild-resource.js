(function () {
    'use strict';

    function RebuildResource($http, umbRequestHelper) {

        var apiUrl = Umbraco.Sys.ServerVariables.Nexu.RebuildApi;

        var resource = {
            getStatus: getStatus,
            startRebuild: startRebuild
        };

        return resource;

        function startRebuild() {

            return umbRequestHelper.resourcePromise(
                $http.get(apiUrl + 'Rebuild'),
                'Failed starting rebuild'
            );
        };

        function getStatus() {

            return umbRequestHelper.resourcePromise(
                $http.get(apiUrl + 'GetRebuildStatus'),
                'Failed getting rebuild status'
            );
        };

    }

    angular.module('umbraco.resources').factory('Our.Umbraco.Nexu.Resources.RebuildResource', RebuildResource);

})();