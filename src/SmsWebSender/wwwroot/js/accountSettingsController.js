(function() {
    "use strict";
    // Getting the existing module
    angular.module("app-sms")
        .controller("accountSettingsController", accountSettingsController);

    function accountSettingsController($http) {
        var vm = this;

        vm.accountSettingsViewModel = {};

        vm.initData = function(model) {
            vm.accountSettingsViewModel = model;
        };

        vm.save = function() {
            $http.post('/notandi/stillingar', vm.accountSettingsViewModel)
                .then(function (response) {
                    window.location.href = '/smsApp';
                }, function (error) {
                    vm.pageModified = true;
                    alert("Ekki tókst að vista. Villa: " + error);
                });
        };

    };
})()