myApp.controller('topCtrl', ['$scope', '$resource', '$location',
function ($scope, $resource, $location) {
    //todo���O�C����񂪂���Ύ����Ń}�C�y�[�W�ɔ�΂�

    //�т����@�����N�N���b�N
    $scope.toSearchBook = function () {
        $location.path("/searchBook");
    };

}]);


