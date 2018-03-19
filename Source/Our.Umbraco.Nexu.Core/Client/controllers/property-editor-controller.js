angular.module('umbraco').controller('Our.Umbraco.Nexu.PropertyEditorController',
    ['$scope', '$routeParams', 'Our.Umbraco.Nexu.Resource',
        function ($scope, $routeParams, nexuResource) {
           

            function init() {
                nexuResource.getIncomingLinks($routeParams.id).then(function (result) {
                    $scope.links = result.data;                    
                });
            }

            init();
        }]);