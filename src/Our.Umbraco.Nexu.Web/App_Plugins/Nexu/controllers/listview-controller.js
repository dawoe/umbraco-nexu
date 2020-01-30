angular.module('umbraco').controller('Our.Umbraco.Nexu.ListViewController',
    ['$scope', '$controller', 'localizationService', 'overlayService', 'Our.Umbraco.Nexu.Resources.RelationCheckResource',
        function ($scope, $controller, localizationService, overlayService, relationCheckResource) {
            // inherit core delete controller
            angular.extend(this, $controller('Umbraco.PropertyEditors.ListViewController', { $scope: $scope }));

            oldDelete = $scope.delete;

            $scope.delete = function () {

                
                relationCheckResource.checkLinkedItems($scope.selection).then(function(data) {
                    if (data.length > 0) {

                    } else {
                        oldDelete();
                    }
                });

               
                //var dialog = {
                //    view: "views/propertyeditors/listview/overlays/delete.html",
                //    deletesVariants: selectionHasVariants(),
                //    isTrashed: $scope.isTrashed,
                //    submitButtonLabelKey: "contentTypeEditor_yesDelete",
                //    submitButtonStyle: "danger",
                //    submit: function (model) {
                //        performDelete();
                //        overlayService.close();
                //    },
                //    close: function () {
                //        overlayService.close();
                //    }
                //};

                //localizationService.localize("general_delete").then(value => {
                //    dialog.title = value;
                //    overlayService.open(dialog);
                //});

            };

            
            
        }]);