angular.module('umbraco').controller('Our.Umbraco.Nexu.ContentDeleteController',
    ['$scope', '$controller', 'Our.Umbraco.Nexu.Resource',
    function ($scope, $controller, nexuResource) {
        // inherit core delete controller
        angular.extend(this, $controller('Umbraco.Editors.Content.DeleteController', { $scope: $scope }));

        $scope.links = {};
        $scope.isLoading = true;

        nexuResource.getIncomingLinks($scope.currentNode.id).then(function (result) {
            $scope.links = result.data;
            $scope.isLoading = false;
        });
    }]);