angular.module('umbraco').controller('Our.Umbraco.Nexu.ListViewController',
    ['$scope', '$controller', 'localizationService', 'overlayService', 'Our.Umbraco.Nexu.Resources.RelationCheckResource',
        function ($scope, $controller, localizationService, overlayService, relationCheckResource) {
            // inherit core delete controller
            angular.extend(this, $controller('Umbraco.PropertyEditors.ListViewController', { $scope: $scope }));

            var oldDelete = $scope.delete;

            var oldUnpublish = $scope.unpublish;

            var dialog = {
                view: '/App_Plugins/Nexu/views/listview-dialog.html',               
                close: function () {
                    overlayService.close();
                }
            };

            $scope.delete = function () {
                openDialog('general_delete','nexu_listviewDeleteIntro', oldDelete);
            };

            $scope.unpublish = function() {
                openDialog('content_unpublish','nexu_listviewUnpublishIntro', oldUnpublish);
            }

            function openDialog(titleKey, introKey, callBack) {
                relationCheckResource.checkLinkedItems($scope.selection).then(function (data) {
                    if (data.length > 0) {
                        localizationService.localizeMany([titleKey, introKey]).then(value => {
                            dialog.title = value[0];
                            dialog.intro = value[1];
                            dialog.items = data;
                            overlayService.open(dialog);
                        });
                    } else {
                        callBack();
                    }
                });  
            }
            
        }]);