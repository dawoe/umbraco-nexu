(function () {
    'use strict';

    function RelationCheckResource($http, umbRequestHelper) {

        var apiUrl = Umbraco.Sys.ServerVariables.Nexu.RelationCheckApi;

        var resource = {
            getIncomingLinks: getIncomingLinks,
            checkLinkedItems: checkLinkedItems
        };

        return resource;

        function getIncomingLinks(udi) {

            return umbRequestHelper.resourcePromise(
                $http.get(apiUrl + 'GetIncomingLinks?udi=' + udi),
                'Failed to get incoming links'
            );
        };

        function checkLinkedItems(udis, isMedia) {

            var udiList = _.map(udis,
                function (u) {
                    return 'umb://' + (isMedia ? 'media' : 'document') + '/' + u.key.replace(/-/g,'');
                });

            return umbRequestHelper.resourcePromise(
                $http.post(apiUrl + 'CheckLinkedItems', JSON.stringify(udiList)),
                'Failed to checked linked items'
            );
        }

    }

    angular.module('umbraco.resources').factory('Our.Umbraco.Nexu.Resources.RelationCheckResource', RelationCheckResource);

})();