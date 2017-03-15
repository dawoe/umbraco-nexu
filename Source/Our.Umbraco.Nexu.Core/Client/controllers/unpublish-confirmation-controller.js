angular.module('umbraco').controller('Our.Umbraco.Nexu.UnPublishConfirmationController',
    ['$scope','notificationsService', 
    function ($scope, notificationsService) {

        $scope.links = $scope.notification.args.links;

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