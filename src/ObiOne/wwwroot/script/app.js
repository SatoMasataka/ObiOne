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
myApp.directive('fileModel', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            element.bind('change', function () {
                scope.$apply(function () {
                    model.assign(scope.$parent.$parent.$parent, element[0].files[0]);
                });
            });
        }
    };
});

//スピナーテキストボックス
myApp.directive('spinner', function () {
    return {
        restrict: 'E',
        require: '^ngModel',
        scope: {
            max: '@',
            min: '@',
            step: '@'
        },
        template: '<span><input type="text" ng-model="model" size="5" style="width:50px"/>'+
                  '<input type="button" value="▲" ng-mousedown="change(1)" ng-mouseup="stopChange()" ng-mouseleave="stopChange()"/>' +
                  '<input type="button" value="▼" ng-mousedown="change(-1)" ng-mouseup="stopChange()" ng-mouseleave="stopChange()"/></span>',
        replace: true,
        link: function (scope, element, attrs, ctrl) {
            scope.$watch('model', function (value) {
                ctrl.$setViewValue(value);
            });

            scope.$parent.$watch(attrs.ngModel, function (value) {
                scope.model = value;
            });

            scope.change = function (i) {
                if (isNaN(scope.model)) {
                    scope.model = scope.min;
                    return;
                };
                $intervalID = setInterval(
                function () {
                    var newVal = parseFloat(scope.model) + scope.step * i;
                    if (newVal <= scope.max && scope.min <= newVal)
                        scope.model = newVal;
                    ctrl.$setViewValue(scope.model);
                },100);                
            }

            scope.stopChange = function () {
                if ($intervalID)
                    clearInterval($intervalID);
            }

        }
    }
});


//画面共通サービス
myApp.service('commonService', ['$location', '$rootScope', 'cpService', function ($location, $rootScope,  cpService) {
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

    //ログオフ処理
    this.logOff = function () {
        var tkn = $rootScope.loginInfo.accessToken;
        $rootScope.is_auth = false;
        $rootScope.loginInfo = null;
        $location.path("/top");

        //トークン破棄
       var revokeUrl = 'https://accounts.google.com/o/oauth2/revoke?token=' + tkn;
       $.ajax({
           type: 'GET',
           url: revokeUrl,
           async: false,
           contentType: "application/json",
           dataType: 'jsonp'
       });
    }

    this.commonBind = function () {
        cpService._cPickerInit();//cpick.jsの準備
    }
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
        .when('/editShop', {
            templateUrl: 'editShop.html',
            controller: 'editShopCtrl'
        })
        .when('/editShop/:shopId', {
            templateUrl: 'editShop.html',
            controller: 'editShopCtrl'
        })
        .when('/shopPage/:shopId', {
            templateUrl: 'shopPage.html',
            controller: 'shopPageCtrl'
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
}]);



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

            //アクセストークン保持
            $rootScope.loginInfo.accessToken=authResult.hg.access_token;
        }); 
    });

    //認証失敗イベント
    $rootScope.$on('event:google-plus-signin-failure', function (event, authResult) {
         alert("Googleアカウントでの認証できませんでした。");
        console.log(event, authResult);
    });

    //ng-repeatをforでまわす
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
    $scope.ToEditShop = function () {
        $location.path("/editShop");
    };
    $scope.LogOff = function () {
        if(confirm("ログオフします。よろしいですか？"))
            commonService.logOff();
    }
    $scope.ToTop = function () {
        $location.path("/top");
        //commonService.logOff();
    }
    $scope.IsTop = function () {
       return  $location.path() == "/top";
    }
}]);