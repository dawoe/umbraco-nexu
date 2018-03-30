angular.module('umbraco').controller('Our.Umbraco.Nexu.UnPublishConfirmationController',
    ['$scope','notificationsService',
    function ($scope, notificationsService) {

        $scope.links = $scope.notification.args.links;

        $scope.descendantsHaveLinks = $scope.notification.args.descendantsHaveLinks;

        $scope.preventUnPublish = Umbraco.Sys.ServerVariables.Nexu.PreventUnPublish;
        $scope.allowUnPublish = true;

        if ($scope.preventUnPublish && ($scope.descendantsHaveLinks || ($scope.links && $scope.links.length > 0))) {
            $scope.allowUnPublish = false;
        }

        $scope.publish = function (notification) {
            notificationsService.remove(notification);

            // execute the deferred unpublish request
            notification.args.deferredPromise.resolve(notification.args.originalRequest);
        };

        $scope.cancel = function (notification) {
            notificationsService.remove(notification);
        };

        $scope.hide = function() {
            notificationsService.remove($scope.notification);
        }
    }]);