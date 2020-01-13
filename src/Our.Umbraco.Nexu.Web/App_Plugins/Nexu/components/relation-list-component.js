(function () {
	'use strict';

	function RelationListComponentController($scope, $location, editorService) {
		var vm = this;
		
		vm.ungrouped = [];

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

            //$location.url(item);
            editorService.contentEditor(editor);
        }

        this.$onInit= function() {
            for (var i = 0; i < this.relations.length; i++) {
                var relation = this.relations[i];

                for (var j = 0; j < relation.Properties.length; j++) {
                    var status = 'Unpublished';

                    if (relation.IsTrashed) {
                        status = 'Recycle bin';
                    }
                    else if (relation.IsPublished) {
                        status = 'Published';
                    }

                    vm.ungrouped.push({
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
            vm.showLanguageColumn = this.showLanguage;
        }

    }

	angular.module('umbraco').component('nexuRelationList',
        {
            controller: RelationListComponentController,
        	controllerAs: "vm",
            bindings: {
                relations: '<',
                showLanguage : '<'
            },
            templateUrl: '/App_Plugins/Nexu/views/relation-list-component.html'
        });

})();