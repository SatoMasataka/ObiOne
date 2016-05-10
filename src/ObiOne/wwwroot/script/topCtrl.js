myApp.controller('topCtrl', ['$scope', '$resource', '$location',
function ($scope, $resource, $location) {
    //todoログイン情報があれば自動でマイページに飛ばす

    //帯を作る　リンククリック
    $scope.toSearchBook = function () {
        $location.path("/searchBook");
    };

}]);


