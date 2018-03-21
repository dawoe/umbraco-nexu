angular.module('umbraco').controller('Our.Umbraco.Nexu.PropertyEditorController',
    ['$scope', '$routeParams', 'Our.Umbraco.Nexu.Resource',
        function ($scope, $routeParams, nexuResource) {

            $scope.isCreate = false;

            if ($routeParams.create) {
                $scope.isCreate = true;
            }

            function init() {

                if ($scope.isCreate === false) {
                    nexuResource.getIncomingLinks($routeParams.id).then(function (result) {
                        $scope.links = result.data;
                    });
                }
                

            }

            init();
        }]);