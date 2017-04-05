angular.module("umbraco.directives")
    .directive('nexuLinks', function () {
        return {
            restrict: "E",    // restrict to an element
            replace: true,   // replace the html element with the template
            transclude : true,
            templateUrl: '/App_Plugins/Nexu/views/nexu-links.html',
            scope: {
                links: '=',
                linkClicked : '=',
                title : '@'
            },
            link: function (scope, element, attrs, ctrl) {

                scope.currentIndex = -1;

                scope.getEditLink = function(id) {
                    return '#/content/content/edit/' + id;
                };

                scope.handleLinkClick = function(id) {
                    scope.linkClicked();
                };

                scope.showProperties = function(index) {
                    scope.currentIndex = index;
                };

                scope.hideProperties = function() {
                    scope.currentIndex = -1;
                };
            }
        };
    });