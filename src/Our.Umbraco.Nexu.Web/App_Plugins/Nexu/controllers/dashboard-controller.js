(function () {
    'use strict';

    function DashboardController($scope, $timeout, localizationService, resource) {
        var vm = this;

        var idleLabel = '';
        
        vm.buttonState = 'init';
        vm.status = {
            IsRunning: true,
            Status: idleLabel
        }

        function getStatus() {
            resource.getStatus().then(function (result) {
                vm.status.IsRunning = result.IsProcessing;
                
                if (vm.status.IsRunning) {
                    localizationService
                        .localize('nexuDashboard_rebuildProcessingStatusLabelFormat', [result.ItemName, result.ItemsProcessed])
                        .then(function(data) {
                            vm.status.Status = data;
                        });
                    $timeout(function() { getStatus() }, 1000, true);
                } else {
                    vm.status.Status = idleLabel;
                }
            });
        }


        function startRebuild() {
            vm.buttonState = 'busy';
            resource.startRebuild().then(function () {
                    vm.buttonState = 'success';
                    vm.status.IsRunning = true;
                    getStatus();
                },
                function () {
                    vm.buttonState = 'error';
                });

        };

        vm.startRebuild = startRebuild;

        function init() {
            localizationService.localize('nexuDashboard_rebuildIdleStatusLabel').then(function(data) {
                idleLabel = data;
            });
            getStatus();
        }

        init();

    }

    angular.module('umbraco').controller('Our.Umbraco.Nexu.Controllers.DashboardController',
        [
            '$scope',
            '$timeout',
            'localizationService',
            'Our.Umbraco.Nexu.Resources.RebuildResource',
            DashboardController
        ]);

})();