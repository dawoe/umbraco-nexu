angular.module("umbraco.directives")
    .directive('nexuDescendantsWarning', function () {
        return {
            restrict: "E",    // restrict to an element
            replace: true,   // replace the html element with the template
            transclude: true,
            templateUrl: '/App_Plugins/Nexu/views/nexu-descendants-warning.html'            
        };
    });