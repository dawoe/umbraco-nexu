angular.module('umbraco').controller('Our.Umbraco.Nexu.ListViewController',
    ['$scope', '$controller', 'localizationService', 'overlayService', 'Our.Umbraco.Nexu.Resources.RelationCheckResource',
        function ($scope, $controller, localizationService, overlayService, relationCheckResource) {
            // inherit core delete controller
            angular.extend(this, $controller('Umbraco.PropertyEditors.ListViewController', { $scope: $scope }));

            var oldDelete = $scope.delete;

            var oldUnpublish = $scope.unpublish;

            var dialog = {
                view: "/App_Plugins/Nexu/views/listview-dialog.html",               
                close: function () {
                    overlayService.close();
                }
            };

            $scope.delete = function () {
                openDialog("general_delete", oldDelete);
            };

            $scope.unpublish = function() {
                openDialog("content_unpublish", oldUnpublish);
            }

            function openDialog(titleKey, callBack) {
                relationCheckResource.checkLinkedItems($scope.selection).then(function (data) {
                    if (data.length > 0) {
                        localizationService.localize(titleKey).then(value => {
                            dialog.title = value;
                            dialog.items = data;
                            overlayService.open(dialog);
                        });
                    } else {
                        callBack();
                    }
                });  
            }
            
        }]);