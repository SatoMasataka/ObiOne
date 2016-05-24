myApp.controller('topCtrl', ['$scope', '$location', 'commonService',
function ($scope, $location, commonService) {
    //ページ表示時
    $scope.initDisp = function () {
        //ログインチェック
        commonService.checkAuth_regist();

    }

    //帯を作る　リンククリック
    $scope.toSearchBook = function () {
        $location.path("/searchBook");
    };

}]);


