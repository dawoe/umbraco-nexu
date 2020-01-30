angular.module('umbraco').controller('Our.Umbraco.Nexu.ListViewController',
    ['$scope', '$controller', 'localizationService', 'overlayService', 'Our.Umbraco.Nexu.Resources.RelationCheckResource',
        function ($scope, $controller, localizationService, overlayService, relationCheckResource) {
            // inherit core delete controller
            angular.extend(this, $controller('Umbraco.PropertyEditors.ListViewController', { $scope: $scope }));

            var oldDelete = $scope.delete;

            var dialog = {
                view: "/App_Plugins/Nexu/views/listview-dialog.html",               
                close: function () {
                    overlayService.close();
                }
            };

            $scope.delete = function () {

                
                relationCheckResource.checkLinkedItems($scope.selection).then(function(data) {
                    if (data.length > 0) {
                        localizationService.localize("general_delete").then(value => {
                            dialog.title = value;
                            dialog.items = data;
                            overlayService.open(dialog);
                        });
                    } else {
                        oldDelete();
                    }
                });                             

            };

            
            
        }]);