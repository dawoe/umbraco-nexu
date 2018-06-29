angular.module('umbraco').controller('Our.Umbraco.Nexu.ConfirmUnpublishController',
    [
        '$scope', '$controller', 'Our.Umbraco.Nexu.Resource',
        function($scope, $controller, nexuResource) {
            angular.extend(this, $controller('Umbraco.Notifications.ConfirmUnpublishController', { $scope: $scope }));           
        }
    ]);