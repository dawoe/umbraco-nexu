(function () {
    'use strict';

    function RelationCheckServive($q, resource) {

        var service = {
            checkRelations: getIncheckRelationscomingLinks
        };

        return service;

        function getIncheckRelationscomingLinks(udi) {

            var deferred = $q.defer();

            var result = {
                relations  :[],
                descendantsUsed : false
            };

            resource.getIncomingLinks(udi).then(function (data) {

                if (data.length > 0) {
                    result.relations = data;
                    deferred.resolve(result);
                }
                else {
                    resource.checkDescendants(udi).then(function (data) {
                        result.descendantsUsed = data;
                        deferred.resolve(result);
                    })
                }
                    
                   
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