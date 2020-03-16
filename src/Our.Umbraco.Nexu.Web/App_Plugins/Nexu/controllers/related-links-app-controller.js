(function () {
    'use strict';

    function RelatedLinksAppController($scope, appState) {
        var vm = this;

        vm.showLanguage = appState.getSectionState("currentSection") === 'media';

        vm.relations = $scope.model.viewModel;     
        var currentVariant = _.find($scope.content.variants, function (v) { return v.active });

        if (currentVariant && currentVariant.language) {
            vm.culture = currentVariant.language.culture;
            vm.cultureRelations = _.filter(vm.relations,
                function(r) { return r.Culture.toLowerCase() === vm.culture.toLowerCase() });
        } else {
            vm.cultureRelations = vm.relations;
        }

    }

    angular.module('umbraco').controller('Our.Umbraco.Nexu.Controllers.RelatedLinksAppController',
        [
            '$scope',            
            'appState',
            RelatedLinksAppController
        ]);

})();