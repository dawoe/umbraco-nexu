angular.module('umbraco').controller('Our.Umbraco.Nexu.DashboardController',
    ['$scope', 'Our.Umbraco.Nexu.Resource', function ($scope, nexuResource) {
        $scope.isLoading = true;
        $scope.RebuildStatus = {
            IsProcessing: true,
            ItemName: '',
            ItemsProcessed : 0
        };

        $scope.getRebuildStatus = function() {
            nexuResource.getRebuildStatus()
                .then(function(result) {
                    $scope.isLoading = false;
                    $scope.RebuildStatus = result.data;
                });
        };

        $scope.rebuild = function() {
            $scope.RebuildStatus.IsProcessing = true;


            nexuResource.rebuild(-1)
                .then(function(result) {
                    $scope.getRebuildStatus();
                });
        };

        $scope.getRebuildStatus();

    }]);