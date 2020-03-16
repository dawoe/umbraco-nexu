angular.module('umbraco').controller('Our.Umbraco.Nexu.MediaDeleteController',
    ['$scope', '$controller', 
    function ($scope, $controller) {
        $scope.isMedia = true;

        // inherit base delete controller        
        angular.extend(this, $controller('Our.Umbraco.Nexu.BaseDeleteController', { $scope: $scope }));
        
    }]);