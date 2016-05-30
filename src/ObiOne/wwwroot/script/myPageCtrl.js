myApp.controller('myPageCtrl', ['$scope', '$resource', '$location','$rootScope','$modal', 'commonService', 
function ($scope, $resource, $location,$rootScope,$modal, commonService) {
    var apiObi = $resource("ObiOne/RegistObi/?ID=:id");
    var api_Shop = $resource("ObiOne/Shop/?ID=:id");

    //ページ表示時
    $scope.init = function () {
        //認証＋登録チェック
        commonService.checkAuth_regist();

        //ログインユーザー作成の帯を取得
        apiObi.query(
           //クエリストリング
           { id: $rootScope.loginInfo.Id },
           //成功時
           function (p) {
               $scope.ObiList = p;
           },
           //失敗時
           function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
       );

        //ログインユーザーの店舗を取得
        api_Shop.query(
           //クエリストリング
           { id: $rootScope.loginInfo.Id },
           //成功時
           function (p) {
               $scope.MyShopList = p;
           },
           //失敗時
           function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
       );
    };

    /*帯ダイアログ表示*/
    $scope.DispWiondowOpen = function (o) {
        $scope.Detail = o;

        //ダイアログを出す
        modalInstance = $modal.open({
            templateUrl: "W_Disp",
            scope: $scope
        });

    }

    //ダイアログ内：帯削除
    $scope.DeleteObi = function (obiId) {
        if (!confirm("この帯を削除しますか？")) return;

        apiObi.delete({ obiId: obiId , AccessToken: $rootScope.loginInfo.accessToken},
            //成功時
           function () {
               alert("削除しました。");
               modalInstance.close();
               $scope.init();
           },
           //失敗時
           function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
        );
    }

    //楽天商品ページ表示
    $scope.ToRakutenPage = function (url) {
        window.open(url, "window_name", "scrollbars=yes");
    }

    $scope.toEditShop = function (shopId) {
        $location.path("/editShop/" + shopId);
    };


    //店舗へ遷移
    $scope.toShop = function (shopId) {
        $location.path("/shopPage/"+shopId);
    };
}]);


