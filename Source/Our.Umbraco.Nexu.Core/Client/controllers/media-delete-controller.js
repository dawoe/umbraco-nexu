angular.module('umbraco').controller('Our.Umbraco.Nexu.MediaDeleteController',
    ['$scope', '$controller', 'Our.Umbraco.Nexu.Resource',
    function ($scope, $controller, nexuResource) {
        // inherit core delete controller
        angular.extend(this, $controller('Umbraco.Editors.Media.DeleteController', { $scope: $scope }));

        $scope.links = {};
        $scope.isLoading = true;

        nexuResource.getIncomingLinks($scope.currentNode.id).then(function (result) {
            $scope.links = result.data;
            $scope.isLoading = false;
        });
    }]);