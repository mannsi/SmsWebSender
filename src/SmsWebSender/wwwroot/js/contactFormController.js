(function() {
    "use strict";
    // Getting the existing module
    angular.module("app-sms")
        .controller("contactFormController", contactFormController);

    function contactFormController($http) {
        var vm = this;

        vm.name = "";
        vm.email = "";
        vm.message = "";

        vm.send = function () {
            $http({
                url: "/contact",
                method: "POST",
                params: {
                    name: vm.name,
                    email: vm.email,
                    message: vm.message
                }
            }).then(function (response) {
                vm.name = "";
                vm.email = "";
                vm.message = "";
            }, function (error) {

            });

            //$http.post('/contact',
            //    {
            //        name: vm.name,
            //        email: vm.email,
            //        message: vm.message
            //    })
            //    .then(function (response) {
            //        vm.name = "";
            //        vm.email = "";
            //        vm.message = "";
            //    }, function (error) {
                    
            //    });
        };
    };
})()