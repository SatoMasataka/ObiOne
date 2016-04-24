var myApp = angular.module('myApp', ["ngRoute", "ngResource"]);

myApp.service('editObiObject', function () {
    var service = {
        bookData: null
    };
    return service;
});

myApp.config(['$routeProvider', '$locationProvider',
    function($routeProvider, $locationProvider) {
        //$locationProvider.html5Mode(true);
        $routeProvider
        .when('/searchBook', {
            templateUrl: 'searchBook.html',
            controller: 'searchBookCtrl'
        })
        .when('/editObi', {
            templateUrl: 'editObi.html',
            controller: 'editObiCtrl'
        })
        .otherwise({
            redirectTo: '/searchBook'
        });
    }
]);