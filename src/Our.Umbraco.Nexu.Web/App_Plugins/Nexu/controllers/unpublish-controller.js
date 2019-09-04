(function () {
    "use strict";

    function UnpublishController($scope, $controller, editorState, service) {
        var vm = this;
        angular.extend(this, $controller('Umbraco.Overlays.UnpublishController', { $scope: $scope }));

        vm.loading = true;
        vm.relations = [];
        vm.showLanguageColumn = false;
        $scope.model.disableSubmitButton = true;

        function init() {
            service.checkRelations(editorState.current.udi).then(function (data) {
                $scope.model.disableSubmitButton = false;
                vm.relations = data;
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
