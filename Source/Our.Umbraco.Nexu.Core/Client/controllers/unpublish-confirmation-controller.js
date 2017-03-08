angular.module('umbraco').controller('Our.Umbraco.Nexu.UnPublishConfirmationController',
    ['$scope','notificationsService', 
    function ($scope, notificationsService) {

        $scope.links = $scope.notification.args.links;

        $scope.publish = function (notification) {
            // resolve the deferred request
            notificationsService.remove(notification);
            notification.args.deferredPromise.resolve(notification.args.originalRequest);            
        };

        $scope.cancel = function (notification) {            
            notificationsService.remove(notification);
        };
    }]);