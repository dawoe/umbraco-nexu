angular.module('umbraco').controller('Our.Umbraco.Nexu.ContentDeleteController',
    ['$scope', '$controller', 
    function ($scope, $controller) {
        $scope.isMedia = false;

        // inherit base delete controller        
        angular.extend(this, $controller('Our.Umbraco.Nexu.BaseDeleteController', { $scope: $scope }));
    }]);