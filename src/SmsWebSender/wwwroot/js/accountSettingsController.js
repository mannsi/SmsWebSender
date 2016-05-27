(function() {
    "use strict";
    // Getting the existing module
    angular.module("app-sms")
        .controller("accountSettingsController", accountSettingsController);

    function accountSettingsController() {
        var vm = this;

        vm.senderName = "";
        vm.smsTemplate = "";

        vm.initData = function (senderName, smsTemplate) {
            vm.senderName = senderName;
            vm.smsTemplate = smsTemplate;
        }

    };
})()