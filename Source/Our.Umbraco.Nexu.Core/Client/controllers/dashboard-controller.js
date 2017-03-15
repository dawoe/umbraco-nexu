angular.module('umbraco').controller('Our.Umbraco.Nexu.DashboardController',
    ['$scope', 'Our.Umbraco.Nexu.Resource', function ($scope, nexuResource) {
        $scope.isLoading = true;
        $scope.isProcessing = false;

        nexuResource.getRebuildStatus()
            .then(function(result) {
                $scope.isLoading = false;
                $scope.isProcessing = result.data;
            });

    }]);