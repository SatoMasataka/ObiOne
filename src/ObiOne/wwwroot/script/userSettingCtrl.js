myApp.controller('userSettingCtrl', ['$scope', '$resource', '$location', '$rootScope','commonService',
function ($scope, $resource, $location, $rootScope,commonService) {
        
    //ページ表示時
    $scope.init = function () {
        //ログインチェック
        commonService.checkAuth();

        if ($rootScope.loginInfo)
        {
            //設定変更
            $scope.mode = "1";
            $scope.loginInfo = $rootScope.loginInfo;
        } else {
            //新規登録
            $scope.mode = "0";
            $scope.loginInfo = $rootScope.loginInfo_work;
        }
    }

    //登録ボタンクリック
    $scope.btnRegistClick = function () {
        var api_login = $resource("ObiOne/Login"); //apiパス
        if ($scope.mode == "0") {
            //新規登録
            alert("登録しました。");
            api_login.save({ loginInfo: $scope.loginInfo }, function (g) {
                $rootScope.loginInfo = g.loginInfo;
                $location.path("/myPage");
            });
        } else {
            alert("変更しました。");
        }
    }
}]);