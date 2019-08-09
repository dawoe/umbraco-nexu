(function () {
    'use strict';

    function RelatedLinksAppController($scope) {
        var vm = this;

        vm.relations = $scope.model.viewModel;     
        var currentVariant = _.find($scope.content.variants, function(v) { return v.active });
        vm.culture = currentVariant.language.culture;
        console.log(vm.relations);
        vm.cultureRelations = _.filter(vm.relations, function (r) { return r.Culture.toLowerCase() === vm.culture.toLowerCase() });
        console.log(vm.cultureRelations)
        
    }

    angular.module('umbraco').controller('Our.Umbraco.Nexu.Controllers.RelatedLinksAppController',
        [
            '$scope',
            'editorState',
            'localizationService',
            RelatedLinksAppController
        ]);

})();