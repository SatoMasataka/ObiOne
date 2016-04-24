myApp.service('editCanvasService', function () {
    var stage;//canvas�p
    var CanvasW;
    var CanvasH;
    
    var startPoint = { x:0, y:0};//�\���摜�n�_
    var bookSize = { w:0, h:0};//�\���摜�T�C�Y
    
    var Obi;

    //// �y�[�W�\������̏��� ////
    this.PageInitCanvas = function (imgPath) {

        //������stage�����������Ȃ��Ɠ��ڈȍ~�ɔ��ł����ꍇ��canvas���@�\���Ȃ�
        canvasReady();

        // preload.js�œǂݍ��񂾑f�ނ�ێ�����ϐ��B
        var assets = {};

        // preloadjs���g���ĉ摜��ǂݍ��ށB
        var loadManifest = [{ id: "book", src: imgPath }];
        var loader = new PreloadJS(false);
        loader.onFileLoad = function (event) {
           assets[event.id] = event.result;
        }
        loader.onComplete = function () {
            //�摜�ǂݍ��݂��I������ۂɁA�\���摜�\��
            var bookImg = new createjs.Bitmap(assets["book"]);

            //  �������c�����ŏk�ڗ���ύX
            var scalePoint =(bookImg.image.height > bookImg.image.width)?
                CanvasH / bookImg.image.height : CanvasW / bookImg.image.width;

            bookImg.scaleX = scalePoint;
            bookImg.scaleY = scalePoint;
            bookSize = { w:bookImg.image.width* scalePoint, h:bookImg.image.height * scalePoint};

            //�����񂹂ɂȂ�悤�Ɏn�_���߂�
            bookImg.x = (CanvasW - bookSize.w ) / 2;
            bookImg.y = (CanvasH - bookSize.h ) / 2;
            startPoint = { x: bookImg.x, y: bookImg.y};

            stage.addChild(bookImg);
            stage.update();
        }
        loader.loadManifest(loadManifest);
    }

    //// �т̏�Ԃ��ŐV�� ////
    this.UpdateObi = function (obiInfo) {
        stage.removeChild(Obi);

        var obiH = obiInfo.ObiHeightPercentage * 0.01 * bookSize.h;
        var obiW = bookSize.w;

        Obi= new createjs.Shape();
        Obi.graphics.beginFill(obiInfo.ObiColor).drawRect(0, 0, obiW, obiH);
        Obi.x = startPoint.x + bookSize.w - obiW;
        Obi.y = startPoint.y+bookSize.h - obiH;
        stage.addChild(Obi);
        stage.update();
    }



    //�L�����o�X������
    function canvasReady() {
        stage = new createjs.Stage("main-canvas");
        createjs.Ticker.setFPS(6);
        createjs.Ticker.addEventListener('tick', function () { stage.update(); });
        CanvasW = stage.canvas.clientWidth;
        CanvasH = stage.canvas.clientHeight;

        //�w�i�`��
        var shape = new createjs.Shape();
        shape.graphics.beginFill("#000").drawRect(0, 0, CanvasW, CanvasH);
        shape.x = 0;
        shape.y = 0;
        stage.addChild(shape);
        stage.update();
    }

    // �����_n�ʂ܂ł��c���֐�
    // number=�Ώۂ̐��l
    // n=�c�����������_�ȉ��̌���
    function floatFormat(number, n) {
        var _pow = Math.pow(10, n);

        return Math.round(number * _pow) / _pow;
    }

});

myApp.controller('editObiCtrl', ['$scope', '$resource', 'editObiObject', 'editCanvasService', 'cpService',
    function ($scope, $resource, editObiObject, editCanvasService, cpService) {

    var apiw = $resource("ObiOne/GetBook");
    $scope.ObiInfo = { ObiHeightPercentage: 0, ObiColor: "#000" };

    //�N�G���X�g�����O���͂����Ƒ傫���摜��url�ɂȂ�
    var imgUrl = (editObiObject.bookData.largeImageUrl).split("?")[0];
    $scope.t = imgUrl;

    /*�y�[�W�\����*/
    $scope.initDisp = function () {
        editCanvasService.PageInitCanvas(imgUrl);
        cpService._cPickerInit();
    }
    
    //////////
    //�ҏW����
    //////////
    /*�ѕ��ύX*/
    $scope.changeObiHeightPercentage = function (addPoint) {
        var newHeight = $scope.ObiInfo.ObiHeightPercentage + addPoint;
        if (0 <= newHeight && newHeight <= 100) {
            $scope.ObiInfo.ObiHeightPercentage = newHeight;
            editCanvasService.UpdateObi($scope.ObiInfo); //�эX�V
        }
    }
    $scope.updateObi = function () {
        editCanvasService.UpdateObi($scope.ObiInfo); //�эX�V
    }
}]);