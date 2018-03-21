angular.module('umbraco').controller('Our.Umbraco.Nexu.PropertyEditorController',
    ['$scope', '$routeParams','appState', 'Our.Umbraco.Nexu.Resource',
        function ($scope, $routeParams, appState, nexuResource) {

            $scope.isCreate = false;

            $scope.currentSection = appState.getSectionState("currentSection");

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