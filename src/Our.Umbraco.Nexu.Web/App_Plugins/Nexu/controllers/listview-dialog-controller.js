(function () {
    'use strict';

    function ListViewDialogController($scope, languageResource, editorService, overlayService) {
        var vm = this;

        vm.languages = [];
        vm.relations = $scope.model.items;
        vm.pageSize = 5;
        vm.pagedListItems = [];
        vm.currentPage = 1;
        vm.totalPages = 1;
        vm.intro = $scope.model.intro;

        vm.openContent = function (item) {
            var editor = {
                id: item.Id,
                nexuCulture: item.Culture,
                submit: function (model) {
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };
           
            overlayService.close();
            editorService.contentEditor(editor);
        }

        vm.getLanguageLabel = function (culture) {

            if (vm.languages.length > 0) {

                var lang = _.find(vm.languages,
                    function (l) {
                        return l.culture.toLowerCase() === culture.toLowerCase();
                    });

                if (lang) {
                    return lang.name;
                }
            }

            return culture;
        }

        vm.nextPage = function () {
            vm.goToPage(vm.currentPage + 1);
        };

        vm.prevPage = function () {
            vm.goToPage(vm.currentPage - 1);
        };
        vm.goToPage = function (pageNumber) {
            vm.currentPage = pageNumber;
            setPagedList();
        };


        function setPagedList() {
            vm.pagedListItems = vm.relations.slice((vm.currentPage - 1) * vm.pageSize, vm.currentPage * vm.pageSize);

        }

        function init() {
            languageResource.getAll().then(function (data) {
                vm.languages = data;
                vm.totalPages = Math.ceil(vm.relations.length / vm.pageSize);
                setPagedList();
            });
        }

        init();      
    }

    angular.module('umbraco').controller('Our.Umbraco.Nexu.Controllers.ListViewDialogController',
        [
            '$scope',           
            'languageResource',
            'editorService',
            'overlayService',
            ListViewDialogController
        ]);

})();