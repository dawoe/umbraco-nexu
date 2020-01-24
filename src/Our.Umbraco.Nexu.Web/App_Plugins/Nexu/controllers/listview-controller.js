angular.module('umbraco').controller('Our.Umbraco.Nexu.ListViewController',
    ['$scope', '$controller', 'Our.Umbraco.Nexu.Services.RelationCheckServive',
        function ($scope, $controller, service) {
            // inherit core delete controller
            angular.extend(this, $controller('Umbraco.PropertyEditors.ListViewController', { $scope: $scope }));


            
        }]);