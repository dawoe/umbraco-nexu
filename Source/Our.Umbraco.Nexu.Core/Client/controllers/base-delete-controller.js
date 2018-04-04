angular.module('umbraco').controller('Our.Umbraco.Nexu.BaseDeleteController',
    ['$scope', '$controller', 'Our.Umbraco.Nexu.Resource',
        function ($scope, $controller, nexuResource) {
            // inherit core delete controller
            if ($scope.isMedia) {
                angular.extend(this, $controller('Umbraco.Editors.Media.DeleteController', { $scope: $scope }));
            } else {
                angular.extend(this, $controller('Umbraco.Editors.Content.DeleteController', { $scope: $scope }));
            }

            $scope.preventDelete = Umbraco.Sys.ServerVariables.Nexu.PreventDelete;
            $scope.allowDelete = true;
           

            $scope.links = {};
            $scope.descendantsHaveLinks = false;
            $scope.isLoading = true;

            nexuResource.getIncomingLinks($scope.currentNode.id).then(function (result) {
                $scope.links = result.data;

                if (result.data.length == 0) {
                    nexuResource.checkDescendants($scope.currentNode.id, $scope.isMedia).then(function (result) {
                        $scope.descendantsHaveLinks = result.data;
                        if ($scope.descendantsHaveLinks === "true" && $scope.preventDelete) {
                            $scope.allowDelete = false;
                        }
                        $scope.isLoading = false;
                    });
                } else {
                    // we found links, so prevent deleting if set in config
                    if ($scope.preventDelete) {
                        $scope.allowDelete = false;
                    }
                    $scope.isLoading = false;
                }

            });
        }]);