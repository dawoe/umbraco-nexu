(function () {
    "use strict";

    function DashboardController($scope, $timeout, resource) {
        var vm = this;
        
        vm.buttonState = "init";
        vm.status = {
            IsRunning: true,
            Status: 'Ready'
        }

        function getStatus() {
            resource.getStatus().then(function (result) {
                vm.status.IsRunning = result.IsProcessing;
                
                if (vm.status.IsRunning) {
                    vm.status.Status = "Currently processing " +
                        result.ItemName +
                        ". Total " +
                        result.ItemsProcessed +
                        " items processed";
                    $timeout(function() { getStatus() }, 1000, true);
                } else {
                    vm.status.Status = 'Ready';
                }
            });
        }


        function startRebuild() {
            vm.buttonState = "busy";
            resource.startRebuild().then(function () {
                    vm.buttonState = "success";
                    vm.status.IsRunning = true;
                    getStatus();
                },
                function () {
                    vm.buttonState = "error";
                });

        };

        vm.startRebuild = startRebuild;

        function init() {
            getStatus();
        }

        init();

    }

    angular.module("umbraco").controller("Our.Umbraco.Nexu.Controllers.DashboardController",
        [
            "$scope",
            "$timeout",            
            "Our.Umbraco.Nexu.Resources.RebuildResource",
            DashboardController
        ]);

})();