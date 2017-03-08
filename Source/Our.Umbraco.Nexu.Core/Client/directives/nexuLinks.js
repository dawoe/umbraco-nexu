angular.module("umbraco.directives")
    .directive('nexuLinks', function () {
        return {
            restrict: "E",    // restrict to an element
            replace: true,   // replace the html element with the template
            transclude : true,
            templateUrl: '/App_Plugins/Nexu/views/nexu-links.html',
            scope: {
                links: '=',
                title : '@'
            },
            link: function (scope, element, attrs, ctrl) {
                
            }
        };
    });