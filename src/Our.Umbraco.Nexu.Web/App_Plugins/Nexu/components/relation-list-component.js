(function () {
	'use strict';

    function RelationListComponentController($scope, editorService, languageResource, navigationService, overlayService, localizationService) {
		var vm = this;
		
        vm.listitems = [];
        vm.languages = [];
        vm.pageSize = 10;
        vm.showLanguageColumn = false;
        vm.pagedListItems = [];
        vm.currentPage = 1;
        vm.totalPages = 1;
       
        vm.openContent = function (item) {
            var editor = {
                id: item.id,
                nexuCulture: item.language,
                submit: function(model) {
                    editorService.close();
                },
                close: function() {
                    editorService.close();
                }
            };

            navigationService.hideDialog();
            overlayService.close();
            editorService.contentEditor(editor);
        }

        vm.getLanguageLabel = function (culture) {

            if (vm.languages.length > 0) {
               
                var lang = _.find(vm.languages,
                    function(l) {
                        return l.culture.toLowerCase() === culture.toLowerCase();
                    });

                if (lang) {
                    return lang.name;
                }
            }

            return culture;
        }

        vm.nextPage = function() {
            vm.goToPage(vm.currentPage + 1);
        };

        vm.prevPage = function() {
            vm.goToPage(vm.currentPage - 1);
        };
        vm.goToPage = function(pageNumber) {
            vm.currentPage = pageNumber;
            setPagedList();
        };

        function setPagedList() {
            vm.pagedListItems = vm.listitems.slice((vm.currentPage - 1) * vm.pageSize, vm.currentPage * vm.pageSize);
            
        }

        this.$onInit = function () {

            if (this.showLanguage) {
                vm.showLanguageColumn = this.showLanguage;
            }
            
            if (this.itemsPerPage) {
                vm.pageSize = this.itemsPerPage;
            }

            if (vm.showLanguageColumn) {
                languageResource.getAll().then(function (data) {
                    vm.languages = data;
                });
            }

            for (var i = 0; i < this.relations.length; i++) {
                var relation = this.relations[i];

                for (var j = 0; j < relation.Properties.length; j++) {
                    var status = 'content_unpublished';

                    if (relation.IsTrashed) {
                        status = 'contentpicker_trashed';
                    }
                    else if (relation.IsPublished) {
                        status = 'content_published';
                    }

                    vm.listitems.push({
                        name: relation.Name,
                        propertyname: relation.Properties[j].PropertyName,
                        tabname: relation.Properties[j].TabName,
                        status: status,
                        editlink: '/content/content/edit/' + relation.Id + '?mculture=' + relation.Culture,
                        language: relation.Culture,
                        id : relation.Id
                    });


                }
            }

            vm.totalPages = Math.ceil(vm.listitems.length / vm.pageSize);

            setPagedList();
        }



    }

	angular.module('umbraco').component('nexuRelationList',
        {
            controller: RelationListComponentController,
        	controllerAs: "vm",
            bindings: {
                relations: '<',
                showLanguage: '<',
                itemsPerPage : '<'
            },
            templateUrl: '/App_Plugins/Nexu/views/relation-list-component.html'
        });

})();