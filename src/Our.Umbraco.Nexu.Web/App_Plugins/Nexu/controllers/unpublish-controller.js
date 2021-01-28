(function () {
    "use strict";

    function UnpublishController($scope, $controller, editorState, service) {
        var vm = this;
        angular.extend(this, $controller('Umbraco.Overlays.UnpublishController', { $scope: $scope }));

        vm.loading = true;
        vm.relations = [];
        vm.descendantsHaveLinks = false;
        vm.showLanguageColumn = false;
        $scope.model.disableSubmitButton = true;

        function init() {
            service.checkRelations(editorState.current.udi).then(function (data) {
                vm.relations = data.relations;
                vm.descendantsHaveLinks = data.descendantsUsed;
                $scope.model.disableSubmitButton = false;

                if (Umbraco.Sys.ServerVariables.Nexu.PreventUnPublish) {
                    if (vm.relations.length > 0 || vm.descendantsHaveLinks) {
                        $scope.model.disableSubmitButton = true;
                    }
                }

                vm.loading = false;
            });

        }

        init();
    }

    angular.module("umbraco").controller("Our.Umbraco.Nexu.Controllers.UnpublishController",
        [
            '$scope',
            '$controller',
            'editorState',
            'Our.Umbraco.Nexu.Services.RelationCheckServive',
        UnpublishController]);

})();
