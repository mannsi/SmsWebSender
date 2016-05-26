(function() {
    "use strict";
    // Getting the existing module
    angular.module("app-sms")
        .controller("smsController", smsController);

    function smsController($http, $timeout) {
        var vm = this;

        vm.name = "mannsi";
        vm.recipients = [];
        vm.errorMessage = "";
        vm.sendingSuccessMessage = "";
        vm.isFetching = true;
        vm.isSending = false;
        vm.startFade = false;
        vm.fadeIn = false;

        function fetchFunc(date) {
            var dateString = date.toUTCString();
            $http(
                {
                url:"/sms/recipients",  
                method: 'GET',
                params: { date: dateString }
                }
                )
                .then(function(response) {
                        // Success
                        angular.copy(response.data, vm.recipients);
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
            $http.post("/sms/send", vm.recipients)
                .then(function (response) {
                    vm.fadeIn = true;
                    vm.sendingSuccessMessage = "Sending tókst !";
                    
                    $timeout(function () {
                        vm.fadeIn = false;
                        vm.startFade = true;
                    }, 2000);
                    },
                    function() {
                        vm.errorMessage = "Gat ekki sent sms";
                    })
                .finally(function() {
                    vm.isSending = false;
                });
        }
        vm.addRecipient = function() {
            vm.recipients.push({
                Name: "",
                Number: "",
                AppointmentStartTime: "",
                Text: ""
            });
        }

        vm.fetchToday = function () {
            var today = new Date();
            fetchFunc(today);
        };

        vm.fetchTomorrow = function () {
            var tomorrow = addDays(new Date(), 1);
            fetchFunc(tomorrow);
        };

        vm.fetchToday();
    };
})()