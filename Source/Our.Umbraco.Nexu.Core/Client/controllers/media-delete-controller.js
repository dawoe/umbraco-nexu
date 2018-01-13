angular.module('umbraco').controller('Our.Umbraco.Nexu.MediaDeleteController',
    ['$scope', '$controller', 'Our.Umbraco.Nexu.Resource',
    function ($scope, $controller, nexuResource) {
        // inherit core delete controller
        angular.extend(this, $controller('Umbraco.Editors.Media.DeleteController', { $scope: $scope }));

        $scope.links = {};
        $scope.descendantsHaveLinks = false;
        $scope.isLoading = true;

        nexuResource.getIncomingLinks($scope.currentNode.id).then(function (result) {
            $scope.links = result.data;

            if (result.data.length == 0) {
                nexuResource.checkDescendants($scope.currentNode.id).then(function(result) {
                    $scope.descendantsHaveLinks = result.data;
                    $scope.isLoading = false;
                });
            } else {
                $scope.isLoading = false;
            }
           
        });
    }]);