(function() {
    "use strict";
    // Getting the existing module
    angular.module("app-sms")
        .controller("smsController", smsController);

    function smsController($http, $timeout) {
        var vm = this;

        vm.messageLinesBlocks = [];
        vm.calendarEntries = new Array();
        vm.errorMessage = "";
        vm.sendingSuccessMessage = "";
        vm.isFetching = true;
        vm.isSending = false;
        vm.startFade = false;
        vm.fadeIn = false;
        vm.senderName = "";
        vm.allowedSenderNameRegex = '[A-Za-z0-9]+';

        vm.initData =  function(senderName) {
            vm.senderName = senderName;
        }

        function stringIsNumeric(n) {
            return !isNaN(parseFloat(n)) && isFinite(n);
        }

        function fetchFunc(date) {
            vm.isFetching = true;
            var dateString = date.toUTCString();
            $http(
                {
                    url: "/smsApp/messageLinesBlocks",
                    method: 'GET',
                    params: { date: dateString }
                }
                )
                .then(function(response) {
                        // Success
                        angular.copy(response.data, vm.messageLinesBlocks);
                    },
                    function(error) {
                        // Failure
                        vm.errorMessage = "Gat ekki náð í gögn";
                    })
                .finally(function() {
                    vm.isFetching = false;
                });
        }

        function addDays(date, days) {
            var result = new Date(date);
            result.setDate(result.getDate() + days);
            return result;
        }

        vm.sendSms = function() {
            vm.isSending = true;
            vm.errorMessage = "";
            $http.post('/smsApp/send', vm.messageLinesBlocks)
                .then(function (response) {
                    vm.fadeIn = true;
                    vm.sendingSuccessMessage = "Sending tókst !";
                    
                    $timeout(function () {
                        vm.fadeIn = false;
                        vm.startFade = true;
                    }, 3000);
                    },
                    function() {
                        vm.errorMessage = "Gat ekki sent sms";
                    })
                .finally(function() {
                    vm.isSending = false;
                });
        }

        vm.fetchToday = function () {
            vm.messageLinesBlocks = [];
            var today = new Date();
            fetchFunc(today);
        };

        vm.fetchTomorrow = function () {
            vm.messageLinesBlocks = [];
            var tomorrow = addDays(new Date(), 1);
            fetchFunc(tomorrow);
        };

        vm.fetchAfter3Days = function () {
            vm.messageLinesBlocks = [];
            var tomorrow = addDays(new Date(), 3);
            fetchFunc(tomorrow);
        };

        vm.validGsmNumber = function (numberString) {
            return !numberString.startsWith('5') && numberString.length === 7 && stringIsNumeric(numberString);
        };

        vm.fetchToday();
    };
})()