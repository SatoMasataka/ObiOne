myApp.controller('shopPageCtrl', ['$scope', '$resource', '$routeParams','$modal',
function ($scope, $resource, $routeParams, $modal) {
    var bg = ["img/shopBg/bg0.jpg", "img/shopBg/bg1.jpg", "img/shopBg/bg2.jpg"];


    /*画面表示時*/
    $scope.initDisp = function () {
        //クリップボードへコピー
        var clipboard = new Clipboard('.cpbtn');
        $scope.thisUrl = window.location.href;

        $scope.noData = false;//データ取得失敗フラグ

        //IDが無ければリターン
        var sid = $routeParams.shopId;
        if (!sid) {
            $scope.noData = true;
            return;
        }

        var api_Shop = $resource("ObiOne/Shop/?shopId="+sid);

        //店舗情報を取得
        api_Shop.get(
           //クエリストリング
           { },
           //成功時
           function (p) {
               //何も取れてなかったらエラー
               if (!p) {
                   $scope.noData = true;
                   return;
               }

               $scope.ShopInfo = p;
               $scope.bgPath = bg[p.BackImg] ? bg[p.BackImg] : bg[0];
           },
           //失敗時
           function () {
               $scope.noData = true;
           }
       );
    }

    /*帯ダイアログ表示*/
    $scope.DispWiondowOpen = function (o) {
        $scope.Detail = o;

        //ダイアログを出す
        modalInstance = $modal.open({
            templateUrl: "W_Disp",
            scope: $scope
        });
    }

    //楽天商品ページ表示
    $scope.ToRakutenPage = function (url) {
        window.open(url, "window_name", "scrollbars=yes");
    }
}]);


