myApp.controller('myPageCtrl', ['$scope', '$resource', '$location','$rootScope','$modal', 'commonService', 
function ($scope, $resource, $location,$rootScope,$modal, commonService) {
    var apiObi = $resource("ObiOne/RegistObi/?ID=:id");


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

    };

    /*帯画像生成ボタンクリック*/
    $scope.DispWiondowOpen = function (o) {
        $scope.Detail = o;

        //ダイアログを出す
        modalInstance = $modal.open({
            templateUrl: "W_Disp",
            scope: $scope
        });

    }

    //帯削除
    $scope.DeleteObi = function (obiId) {
        if (!confirm("この帯を削除しますか？")) return;

        apiObi.delete({ obiId: obiId },
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
   
    //帯を作る　リンククリック
    $scope.toSearchBook = function () {
        $location.path("/searchBook");
    };
}]);


