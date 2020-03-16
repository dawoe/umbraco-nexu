angular.module('umbraco').controller('Our.Umbraco.Nexu.ListViewController',
    ['$scope', '$controller', 'localizationService', 'overlayService', 'appState', 'Our.Umbraco.Nexu.Resources.RelationCheckResource',
        function ($scope, $controller, localizationService, overlayService, appState, relationCheckResource) {
            // inherit core delete controller
            angular.extend(this, $controller('Umbraco.PropertyEditors.ListViewController', { $scope: $scope }));

            var oldDelete = $scope.delete;

            var oldUnpublish = $scope.unpublish;

            var section = appState.getSectionState("currentSection");

            var dialog = {
                view: '/App_Plugins/Nexu/views/listview-dialog.html',               
                close: function () {
                    overlayService.close();
                }
            };

            $scope.delete = function () {

                if (section === 'content' || section === 'media') {
                    openDialog('general_delete', 'nexu_listviewDeleteIntro', oldDelete);
                } else {
                    oldDelete();
                }
               
            };

            $scope.unpublish = function () {

                if (section === 'content' || section === 'media') {
                    openDialog('content_unpublish', 'nexu_listviewUnpublishIntro', oldUnpublish);
                } else {
                    oldUnpublish();
                }                
            }

            function openDialog(titleKey, introKey, callBack) {
                relationCheckResource.checkLinkedItems($scope.selection, section === 'media').then(function (data) {
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