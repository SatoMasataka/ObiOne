myApp.controller('topCtrl', ['$scope', '$location', 'commonService',
function ($scope, $location, commonService) {
    //�y�[�W�\����
    $scope.initDisp = function () {
        //���O�C���`�F�b�N
        commonService.checkAuth_regist();

    }

    //�т����@�����N�N���b�N
    $scope.toSearchBook = function () {
        $location.path("/searchBook");
    };

}]);


