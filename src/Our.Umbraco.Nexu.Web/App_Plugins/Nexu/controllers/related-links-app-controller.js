(function () {
    'use strict';

    function RelatedLinksAppController($scope, $location) {
        var vm = this;

        vm.relations = $scope.model.viewModel;     
        var currentVariant = _.find($scope.content.variants, function (v) { return v.active });

        if (currentVariant && currentVariant.language) {
            vm.culture = currentVariant.language.culture;
            vm.cultureRelations = _.filter(vm.relations,
                function(r) { return r.Culture.toLowerCase() === vm.culture.toLowerCase() });
        } else {
            vm.cultureRelations = vm.relations;
        }
      
        

        vm.ungrouped = [];

        for (var i = 0; i < vm.cultureRelations.length; i++) {
            var relation = vm.cultureRelations[i];
            
            for (var j = 0; j < relation.Properties.length; j++) {
                vm.ungrouped.push({
                    name: relation.Name,
                    propertyname: relation.Properties[j].PropertyName,
                    tabname: relation.Properties[j].TabName,
                    published: relation.IsPublished,
                    trashed: relation.IsTrashed,
                    editlink: '/content/content/edit/' + relation.Id + '?mculture=' + relation.Culture
                });
            }
        }

        vm.openContent = function(link) {
            $location.url(link);
        }
    }

    angular.module('umbraco').controller('Our.Umbraco.Nexu.Controllers.RelatedLinksAppController',
        [
            '$scope',
            '$location',
            RelatedLinksAppController
        ]);

})();