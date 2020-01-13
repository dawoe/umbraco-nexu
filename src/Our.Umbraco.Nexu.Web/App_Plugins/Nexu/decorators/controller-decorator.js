(function () {

    'use strict';

    // Umbraco overrides
    function nexuUmbracoOverrides($provide) {

        $provide.decorator('$controller', function ($delegate) {

           

            return function (constructor, locals, later, ident) {
                var ctrl = $delegate(constructor, locals, later, ident);

                // Check for an ngController attribute
                if (locals.$attrs && locals.$attrs.ngController) {

                    // Override content edit controller method
                    if (locals.$attrs.ngController.match(/^Umbraco\.Editors\.Content\.EditController\b/i)) {
                        //var controller = $delegate.apply(null, arguments);

                        return angular.extend(function () {
                            
                            var ctrlInstance = ctrl();

                            var infiniteMode = locals.$scope.model && locals.$scope.model.infiniteMode;

                            if (infiniteMode && locals.$scope.model.nexuCulture) {
                                locals.$scope.culture = locals.$scope.model.nexuCulture;
                            }
                                                                             
                            return ctrlInstance;
                        }, ctrl);
                    }                   
                }

                return ctrl;
            }

        });

    }

    angular.module("umbraco").config(['$provide', nexuUmbracoOverrides]);

})();