(function () {
    'use strict';

    function RelatedLinksAppController($scope, editorState, localizationService) {
        var vm = this;

        console.log($scope.model);
        console.log(editorState.current);
    }

    angular.module('umbraco').controller('Our.Umbraco.Nexu.Controllers.RelatedLinksAppController',
        [
            '$scope',
            'editorState',
            'localizationService',
            RelatedLinksAppController
        ]);

})();