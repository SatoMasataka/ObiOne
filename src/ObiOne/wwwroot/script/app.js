var myApp = angular.module('myApp', ["ngRoute", "ngResource", "directive.g+signin", "ui.bootstrap"]);

//帯編集画面への引継ぎデータ
myApp.service('editObiObject', function () {
    var service = {
        bookData: null
    };
    return service;
});
//ng-repeat終了でイベント発生させるためのdirectie
myApp.directive("repeatFinished", function($timeout){
    return function(scope, element, attrs){
        if (scope.$last){
            $timeout(function(){
                scope.$emit("repeatFinishedEventFired"); //イベント着火！
            });
        }
    }
});

//画面共通サービス
myApp.service('commonService', ['$location', '$rootScope', '$resource', function ($location, $rootScope, $resource) {
    //認証できているかどうか
    this.checkAuth = function(){
        // 認証できていなければログインへ
        if (!$rootScope.is_auth) {
            $location.path("/top");
        }
    };

    //認証できておりかつ登録済みかどうか
    this.checkAuth_regist = function () {
        // 認証できていなければログインへ
        if (!$rootScope.is_auth  || !$rootScope.loginInfo) {
            $location.path("/top");
        }
    };


    this.logOff = function () {
        //var logoff=$resource(' https://api.yamareco.com/api/v1/oauth/sign_out');
        //logoff.get(null,function(){
            $rootScope.is_auth=false;
            $rootScope.loginInfo=null;
            $location.path("/top");
       // });
      
    };
}]);

myApp.config(['$routeProvider', '$locationProvider', '$httpProvider', function ($routeProvider, $locationProvider, $httpProvider) {
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
         .when('/myPage', {
             templateUrl: 'myPage.html',
             controller: 'myPageCtrl'
         })
        .when('/userSetting', {
            templateUrl: 'userSetting.html',
            controller: 'userSettingCtrl'
        }) 
        .when('/top', {
            templateUrl: 'top.html',
            controller: 'topCtrl'
        })
        .otherwise({
            redirectTo: '/top'
        });

    // Anti IE cache
    if (!$httpProvider.defaults.headers.get)
        $httpProvider.defaults.headers.get = {};
    $httpProvider
        .defaults
        .headers
        .get['If-Modified-Since'] = (new Date(0)).toUTCString();
}
]);



myApp.run(['$rootScope', '$resource', '$location', function ($rootScope, $resource, $location) {
    //認証成功イベント
    $rootScope.$on('event:google-plus-signin-success', function (event, authResult) {
        var api_login = $resource("ObiOne/Login"); //apiパス

        // Googleアカウントによるログインに成功した時にはイベントが発生し accessTokenが得られるのでそれをサーバに教える。
        api_login.get({ accessToken: authResult.hg.access_token }, function (s) {
            $rootScope.is_auth = true;
            if (s.registFlg == "0") {
                //新規登録の場合
                $location.path("/userSetting");
                $rootScope.loginInfo_work = s.loginInfo;
            } else {
                //登録済みの場合
                $location.path("/myPage");
                $rootScope.loginInfo = s.loginInfo;
            }           
        }); 
    });

    //認証失敗イベント
    $rootScope.$on('event:google-plus-signin-failure', function (event, authResult) {
         alert("通信エラー：しばらくしてからもう一度アクセスしてください。");
        console.log(event, authResult);
    });

    $rootScope.range = function (n) {
        var arr = [];
        for (var i = 0; i < n; ++i) arr.push(i);

        return arr;
    };
}]);

//メニュー用コントローラー
myApp.controller('menuCtrl', ['$scope','$location', 'commonService',function ($scope,$location,commonService) {
    $scope.ToSearchBook=function(){
        $location.path("/searchBook");
    };
    $scope.ToMyPage=function(){
        $location.path("/myPage");
    };
    $scope.LogOff=function(){
        commonService.logOff();
    }
    $scope.IsTop = function () {
       return  $location.path() == "/top";
    }
}]);