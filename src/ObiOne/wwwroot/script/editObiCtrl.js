myApp.service('editCanvasService', function () {
    var stage;//canvas用
    var CanvasW;
    var CanvasH;
    
    var startPoint = { x:0, y:0};//表紙画像始点
    var bookSize = { w:0, h:0};//表紙画像サイズ
    
    var Obi;

    //// ページ表示直後の処理 ////
    this.PageInitCanvas = function (imgPath) {

        //ここでstageを初期化しないと二回目以降に飛んできた場合にcanvasが機能しない
        canvasReady();

        // preload.jsで読み込んだ素材を保持する変数。
        var assets = {};

        // preloadjsを使って画像を読み込む。
        var loadManifest = [{ id: "book", src: imgPath }];
        var loader = new PreloadJS(false);
        loader.onFileLoad = function (event) {
           assets[event.id] = event.result;
        }
        loader.onComplete = function () {
            //画像読み込みが終わった際に、表紙画像表示
            var bookImg = new createjs.Bitmap(assets["book"]);

            //  横長か縦長かで縮尺率を変更
            var scalePoint =(bookImg.image.height > bookImg.image.width)?
                CanvasH / bookImg.image.height : CanvasW / bookImg.image.width;

            bookImg.scaleX = scalePoint;
            bookImg.scaleY = scalePoint;
            bookSize = { w:bookImg.image.width* scalePoint, h:bookImg.image.height * scalePoint};

            //中央寄せになるように始点を定める
            bookImg.x = (CanvasW - bookSize.w ) / 2;
            bookImg.y = (CanvasH - bookSize.h ) / 2;
            startPoint = { x: bookImg.x, y: bookImg.y};

            stage.addChild(bookImg);
            stage.update();
        }
        loader.loadManifest(loadManifest);
    }

    //// 帯の状態を最新に ////
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



    //キャンバス初期化
    function canvasReady() {
        stage = new createjs.Stage("main-canvas");
        createjs.Ticker.setFPS(6);
        createjs.Ticker.addEventListener('tick', function () { stage.update(); });
        CanvasW = stage.canvas.clientWidth;
        CanvasH = stage.canvas.clientHeight;

        //背景描画
        var shape = new createjs.Shape();
        shape.graphics.beginFill("#000").drawRect(0, 0, CanvasW, CanvasH);
        shape.x = 0;
        shape.y = 0;
        stage.addChild(shape);
        stage.update();
    }

    // 小数点n位までを残す関数
    // number=対象の数値
    // n=残したい小数点以下の桁数
    function floatFormat(number, n) {
        var _pow = Math.pow(10, n);

        return Math.round(number * _pow) / _pow;
    }

});

myApp.controller('editObiCtrl', ['$scope', '$resource', 'editObiObject', 'editCanvasService', 'cpService',
    function ($scope, $resource, editObiObject, editCanvasService, cpService) {

    var apiw = $resource("ObiOne/GetBook");
    $scope.ObiInfo = { ObiHeightPercentage: 0, ObiColor: "#000" };

    //クエリストリングをはずすと大きい画像のurlになる
    var imgUrl = (editObiObject.bookData.largeImageUrl).split("?")[0];
    $scope.t = imgUrl;

    /*ページ表示時*/
    $scope.initDisp = function () {
        editCanvasService.PageInitCanvas(imgUrl);
        cpService._cPickerInit();
    }
    
    //////////
    //編集操作
    //////////
    /*帯幅変更*/
    $scope.changeObiHeightPercentage = function (addPoint) {
        var newHeight = $scope.ObiInfo.ObiHeightPercentage + addPoint;
        if (0 <= newHeight && newHeight <= 100) {
            $scope.ObiInfo.ObiHeightPercentage = newHeight;
            editCanvasService.UpdateObi($scope.ObiInfo); //帯更新
        }
    }
    $scope.updateObi = function () {
        editCanvasService.UpdateObi($scope.ObiInfo); //帯更新
    }
}]);