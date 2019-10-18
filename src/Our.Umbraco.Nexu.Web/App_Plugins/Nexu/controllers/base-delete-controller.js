angular.module('umbraco').controller('Our.Umbraco.Nexu.BaseDeleteController',
    ['$scope', '$controller', 'Our.Umbraco.Nexu.Services.RelationCheckServive',
        function ($scope, $controller, service) {
            // inherit core delete controller
            if ($scope.isMedia) {
                angular.extend(this, $controller('Umbraco.Editors.Media.DeleteController', { $scope: $scope }));
            } else {
                angular.extend(this, $controller('Umbraco.Editors.Content.DeleteController', { $scope: $scope }));
            }

           
            $scope.links = {};
            $scope.descendantsHaveLinks = false;
            $scope.isLoading = true;

            service.checkRelations($scope.currentNode.udi).then(function (result) {
                $scope.links = result;

                $scope.isLoading = false;

            });
        }]);