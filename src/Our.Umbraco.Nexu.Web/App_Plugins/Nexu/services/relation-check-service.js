(function () {
    'use strict';

    function RelationCheckServive($q, resource) {

        var service = {
            checkRelations: checkRelations
        };

        return service;

        function getIncheckRelationscomingLinks(udi) {

            var deferred = $q.defer();

            resource.getIncomingLinks(udi).then(function (data) {
                    deferred.resolve(data);
                },
                function () {
                    deferred.reject();
                });

            return deferred.promise;
        };

    }

    angular.module('umbraco.resources').factory('Our.Umbraco.Nexu.Services.RelationCheckServive',
        ['$q', 'Our.Umbraco.Nexu.Resources.RelationCheckResource', RelationCheckServive]
        );

})();