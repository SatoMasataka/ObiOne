myApp.controller('editShopCtrl', ['$scope', '$resource', '$location','$modal','$rootScope','$routeParams','commonService',
function ($scope, $resource, $location, $modal, $rootScope,$routeParams,commonService) {
    var api_shop = $resource("ObiOne/Shop"); //api
    var api_Obi = $resource("ObiOne/RegistObi"); //api

    /*ページ表示時*/
    $scope.initDisp = function () {
        //認証＋登録チェック
        commonService.checkAuth_regist();
        
        $(".afterCheck").on("click", function (o) {
            var p = $(o.target).prev();
            p.prop("checked", true);
        });
        
        if ($routeParams.shopId) {
            /*編集時:データDBをから取得  */
            api_shop.get({ shopId: $routeParams.shopId, auth: true }, function (p) {
                $scope.ShopInfo = p;
            },
                //失敗時
                function () {
                    alert("通信エラー：しばらくしてからもう一度アクセスしてください。");
                    $location.path("/myPage");
                }
            );

        } else {
            /*新規登録時 : 空のモデルをセット*/
            $scope.ShopInfo = { ShopName: "", ShopObiList: [] ,BackImg:"0"};

            for (var i = 0; i < 8; i++) {
                $scope.ShopInfo.ShopObiList[i] = {};
            }
        }
    }

    /*帯削除*/
    $scope.removeObiInfo = function (tgtIdx) {
        $scope.ShopInfo.ShopObiList[tgtIdx] = {};
    }

    /*帯選択ウィンドウ表示*/
    $scope.dispSelectObiWindow = function (tgt) {
        $scope.TargetIdx = tgt;

        //ログインユーザー作成の帯を取得
        api_Obi.query(
           //クエリストリング
           { id: $rootScope.loginInfo.Id },
           //成功時
           function (p) {
               $scope.ObiList = p;

           },
           //失敗時
           function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
       );
        //ダイアログを出す
        modalInstance = $modal.open({
            templateUrl: "W_SelectObi",
            scope: $scope
        });
    }

    /*ダイアログ内：クリックされた帯情報を呼び出し元へ返す*/
    $scope.returnObiInfo = function (ret) {
        $scope.ShopInfo.ShopObiList[$scope.TargetIdx].ObiInfo = ret;
        modalInstance.close();
    }



    //登録ボタン
    $scope.btnRegistObiClick = function () {
        //入力チェック

        //OrderSeq採番
        for (var i = 0; i < $scope.ShopInfo.ShopObiList.length; i++) {
            $scope.ShopInfo.ShopObiList[i].OrderSeq = i;
        }

        //生成したデータをDBに登録        
        api_shop.save({ ShopInfo: $scope.ShopInfo, LoginInfo: $scope.loginInfo }, function () {
            var mes = ($scope.ShopInfo)?"仮想店舗を更新しました。":"仮想店舗を登録しました。";
            alert(mes);
            $location.path("/myPage");
        },
            //失敗時
            function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
        );
    }

    $scope.deleteShop = function () {
        if (!confirm("この店舗を削除しますか？")) return;

        api_shop.delete({ ShopId: $scope.ShopInfo.ShopId }, function () {
            alert("仮想店舗を削除しました");
            $location.path("/myPage");
        },
            //失敗時
            function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
        );
    }
}]);


