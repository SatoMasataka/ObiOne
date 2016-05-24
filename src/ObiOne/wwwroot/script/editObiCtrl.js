myApp.service('editCanvasService', function () {
    var stage;//canvas用
    var CanvasW;
    var CanvasH;
    
    var startPoint = { x:0, y:0};//表紙画像始点
    var bookSize = { w:0, h:0};//表紙画像サイズ
    
    var Obi;//Canvas上の帯分要素格納用

    //// 帯の状態を最新に ////
    this.UpdateObi = function (obiInfo) {
        updateObi_Core(obiInfo);
    }

    //// ページ表示直後の処理 ////
    this.PageInitCanvas = function (imgPath,obiInfo) {

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
            var scalePoint;//縮尺率

            if(bookImg.image.height > bookImg.image.width){
                scalePoint = CanvasH / bookImg.image.height
            }else{
                scalePoint = CanvasW / bookImg.image.width;
            }

            bookImg.scaleX = scalePoint;
            bookImg.scaleY = scalePoint;
            bookSize = { w:bookImg.image.width* scalePoint, h:bookImg.image.height * scalePoint};

            //中央寄せになるように始点を定める
            bookImg.x = (CanvasW - bookSize.w ) / 2;
            bookImg.y = (CanvasH - bookSize.h ) / 2;
            startPoint = { x: bookImg.x, y: bookImg.y};

            stage.addChild(bookImg);
            stage.update();

            updateObi_Core(obiInfo);
        }
        loader.loadManifest(loadManifest);
    }

    //// マウスカーソルの位置取得 ////
    this.GetMousePosition = function ()
    {
        var mx = Math.round(stage.mouseX);
        var my = Math.round(stage.mouseY);

        return { X: mx, Y: my };
    }

    //canvasを画像化
    this.GenerateImage=function(){
        return stage.toDataURL(); //todoこれをするには一旦サーバーに画像を落とさないとダメ
    }

    /***ここからローカル関数***/
    //キャンバス初期化
    function canvasReady() {
        stage = new createjs.Stage("main-canvas");
        createjs.Ticker.setFPS(6);
        createjs.Ticker.addEventListener('tick', function () { stage.update(); });
        CanvasW = stage.canvas.clientWidth;
        CanvasH = stage.canvas.clientHeight;

        //背景描画
        //var shape = new createjs.Shape();
        //shape.graphics.beginFill("#000").drawRect(0, 0, CanvasW, CanvasH);
        //shape.x = 0;
        //shape.y = 0;
        //stage.addChild(shape);
        stage.update();
    }

    //帯の最新化コア処理
    function updateObi_Core(obiInfo) {
        stage.removeChild(Obi);//一旦全てリセット

        var obiH = obiInfo.ObiHeightPercentage * 0.01 * bookSize.h;
        var obiW = bookSize.w;

        Obi = new createjs.Container();

        //帯のベース　表紙画像下寄りに表示
        var base = new createjs.Shape();
        base.graphics.beginFill(obiInfo.ObiColor).drawRect(0, 0, obiW, obiH);
        base.x = startPoint.x + bookSize.w - obiW;
        base.y = startPoint.y + bookSize.h - obiH;
        Obi.addChild(base);

        //帯の各Contents描画
        for (var i = 0; i < obiInfo.Contents.length; i++) {
            try {
                loopContainer = new createjs.Container(); //このループで追加する分のコンテナ
                var contentInfo = obiInfo.Contents[i];

                switch (contentInfo.PartsType) {
                    case "0":
                        _GenerateObiContent_Tp0(contentInfo);
                        break;
                    case "1":
                        _GenerateObiContent_Tp1(contentInfo);
                        break;
                    case "2":
                        _GenerateObiContent_Tp2(contentInfo);
                        break;
                }

                Obi.addChild(loopContainer);
            } catch (e) { continue;}
        }
        stage.addChild(Obi);
        stage.update();
    }

    //種別：テキスト
    function _GenerateObiContent_Tp0(contentInfo) {
        //////  テキストの場合 ////////
        if (contentInfo.Tp0.Text == "") return;

        // Text情報(String)生成
        var txtStr = contentInfo.Tp0.Size + "px ";
        if (contentInfo.Tp0.Font)
            txtStr += contentInfo.Tp0.Font + " ";
        if (contentInfo.Tp0.Bold)
            txtStr += "bold ";
        //if (contentInfo.Tp0.Italic)
        //    txtcontentInfo.Tp0o += "italic ";
        txtStr = txtStr.trim();

        //Text描画
        var txt = new createjs.Text(contentInfo.Tp0.Text, txtStr, contentInfo.Tp0.Color);
        txt.textAlign = "left";
        txt.textBaseline = "middle";
        txt.lineHeight = new createjs.Text("あ", txtStr, contentInfo.Tp0.Color).getMeasuredHeight() *1.2;//日本語フォント基準で調整しないと重なることがある
        txt.x = contentInfo.Position.X;
        txt.y = contentInfo.Position.Y;
        txt.rotation = contentInfo.Rotation;

        //文字の行単位配列
        var lines = contentInfo.Tp0.Text.replace('\\n', 'nn').split('\n');

        //最長の文字行を求める
        var longest = lines.sort(function (a, b) {
            var aTxt = new createjs.Text(a, txtStr, contentInfo.Tp0.Color);
            var bTxt = new createjs.Text(b, txtStr, contentInfo.Tp0.Color);

            if (aTxt.getMeasuredWidth() < bTxt.getMeasuredWidth()) return 1;
            if (aTxt.getMeasuredWidth() > bTxt.getMeasuredWidth()) return -1;
            return 0;
        })[0];
        var _longesttxt = new createjs.Text(longest, txtStr, contentInfo.Tp0.Color);

        //囲い図形描画
        var shape = new createjs.Shape();
        shape.x = txt.x;
        shape.y = txt.y;
        shape.rotation = contentInfo.Rotation;

        var sidePadding = 20;//ボーダーと文字がかぶらないようにpaddingを設ける
        var s = shape.graphics.setStrokeStyle(contentInfo.Tp0.LineSize).beginStroke(contentInfo.Tp0.Kakoi.LineColor)
                      .beginFill(contentInfo.Tp0.Kakoi.Color);
        switch (contentInfo.Tp0.Kakoi.Type) {
            case "1":
                //円の場合
                s.drawEllipse(0,0, _longesttxt.getMeasuredWidth() + sidePadding * 2, txt.getMeasuredHeight());
                break;
            case "2":
                //四角
                s.drawRect(0, 0, _longesttxt.getMeasuredWidth() + sidePadding * 2, txt.getMeasuredHeight());
                break;
            case "3":
                //角丸
                s.drawRoundRect(0, 0, _longesttxt.getMeasuredWidth() + sidePadding * 2, txt.getMeasuredHeight(), 20, 20);
                break;
        }
        //基準点をleft middleとあわせる
        shape.regX = sidePadding;
        shape.regY = txt.getMeasuredHeight() / lines.length / 2;

        loopContainer.addChild(shape);

        loopContainer.addChild(txt);
    }

    //種別：図形
    function _GenerateObiContent_Tp1(contentInfo) {
        if (!contentInfo.Tp1.Type || contentInfo.Tp1.Type == "0" 
            || !contentInfo.Tp1.SizeX || contentInfo.Tp1.SizeX <= 0) return;
        
        var shape = new createjs.Shape();

        //回転
        shape.x = contentInfo.Position.X;
        shape.y = contentInfo.Position.Y;
        shape.rotation = contentInfo.Rotation;

        var s = shape.graphics.setStrokeStyle(contentInfo.Tp1.LineSize).beginStroke(contentInfo.Tp1.LineColor)
                     .beginFill(contentInfo.Tp1.Color);

        switch (contentInfo.Tp1.Type) {
            case "1":   //円
                s.drawCircle(0, 0, contentInfo.Tp1.SizeX);
                break;
            case "2":   //四角  
                s.drawRect( contentInfo.Tp1.SizeX / -2, contentInfo.Tp1.SizeY / -2, contentInfo.Tp1.SizeX, contentInfo.Tp1.SizeY);
                break;
            case "3":   //角丸
                s.drawRoundRect(0,0, contentInfo.Tp1.SizeX, contentInfo.Tp1.SizeY, 0, 0);
                break;
            case "4":   //星
                s.drawPolyStar(0,0, contentInfo.Tp1.SizeX, contentInfo.Tp1.KadoNum, 0.6, -90);
                break;
            case "5":   //多角形
                s.drawPolyStar(0, 0, contentInfo.Tp1.SizeX, contentInfo.Tp1.KadoNum, 0, -90);
                break;
            case "6"://直線
                shape.graphics.moveTo(0, 0); 
                shape.graphics.lineTo(contentInfo.Tp1.SizeX, 0);
                shape.graphics.endStroke(); //描画終了
                break;
        } 
        loopContainer.addChild(shape);
    }

    //種別：画像
    function _GenerateObiContent_Tp2(contentInfo) {
        if (!contentInfo.Tp2.ImgPath
           || !contentInfo.Tp2.Size) return;

        var img = new createjs.Bitmap(contentInfo.Tp2.ImgPath);

        img.scaleX = contentInfo.Tp2.Size;
        img.scaleY = contentInfo.Tp2.Size;

        img.x = contentInfo.Position.X;
        img.y = contentInfo.Position.Y;

        //軸はセンターで
        img.regX = img.image.width / 2;
        img.regY = img.image.height / 2;

        img.rotation = contentInfo.Rotation;

        loopContainer.addChild(img);
    }
});

myApp.controller('editObiCtrl', ['$scope', '$resource', '$modal', '$rootScope', '$location','$http', 'editObiObject', 'editCanvasService', 'cpService',
function ($scope, $resource, $modal, $rootScope, $location, $http,editObiObject, editCanvasService, cpService) {

    var api_getbook = $resource("ObiOne/GetBook"); //api
    var api_registObi = $resource("ObiOne/RegistObi"); //api
    var api_upload = $resource("ObiOne/Upload", {}, {
        post: {
            method: 'POST',
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined}
        },
    }); 

    //帯生成情報初期値セット
    $scope.ObiInfo = { ObiHeightPercentage: 20, ObiColor: "#3df509", Contents: [] };
    for (var i = 0; i < 5; i++) {
        //帯Contentsデフォルト情報をセット
        $scope.ObiInfo.Contents[i] = {
            PartsType: "0",
            Position: { X: 400, Y: 400 },
            Tp0: {
                Text: "", Size: 50, Color: "", Font: "serif",
                Kakoi: { Type: "0" }
            },
            Tp1: {
                SizeX: 50, SizeY: 50, Color: "", Type: "0"
            },
            Tp2: {
                ImgPath: "", Size: 1
            },
        };
    }


    /*ページ表示時*/
    $scope.initDisp = function () {
        //データが無いときは検索に戻す
        if (!editObiObject.bookData) $scope.btnToSearchClick();

        //クエリストリングをはずすと大きい画像のurlになる
        var imgUrl = (editObiObject.bookData.largeImageUrl).split("?")[0];
        $scope.bookData = editObiObject.bookData;

        //サーバーに表紙画像を保存(canvasの画像出力のため)
        api_getbook.save({ ImageUrl: imgUrl }, function (p) {
            editCanvasService.PageInitCanvas(p.Path, $scope.ObiInfo);
        },
        function () {
            alert("通信エラー：しばらくしてからもう一度アクセスしてください。");
            $scope.btnToSearchClick();
        });
    }

    //ng-repeatが完全に終了したイベント
    $scope.$on('repeatFinishedEventFired', function() {
        cpService._cPickerInit();//cpick.jsの準備
    });

    //////////
    //編集操作
    //////////
    /*帯更新*/
    $scope.updateObi = function () {
        editCanvasService.UpdateObi($scope.ObiInfo); //帯更新
    }

    /*位置変更*/
    $scope.btnChangePosClick = function (contentIdx) {
        //位置変更対象インデックスを更新
        $scope.changePosIdx = contentIdx;
    }
    /*キャンバスクリック*/
    $scope.canvasClick = function () {

        var mPosi = editCanvasService.GetMousePosition();
        $scope.ObiInfo.Contents[$scope.changePosIdx].Position = mPosi;
        editCanvasService.UpdateObi($scope.ObiInfo); //帯更新
    }


    /*帯画像生成ボタンクリック*/
    $scope.btnCommitClick = function () {
        //イメージ画像生成
        imgData = editCanvasService.GenerateImage();
        $scope.t = imgData;

        //ダイアログへ受け渡し用
        $scope.bookData = editObiObject.bookData;
        $scope.loginInfo = $rootScope.loginInfo;

        //ダイアログを出す
        modalInstance = $modal.open({
            templateUrl: "W_Regist",
            scope: $scope
            // controller: 'EditObi_ModalCtrl'
        });
    }

    //楽天商品ページ表示
    $scope.ToRakutenPage = function (url) {
        window.open(url, "window_name", "scrollbars=yes");
    }

    /*検索に戻るボタンクリック*/
    $scope.btnToSearchClick = function () {
        $location.path("/searchBook");
    }
   

    //ダイアログ内：登録ボタン
    $scope.btnRegistObiClick = function () {
        if (!imgData) {
            alert("エラー：画像データがありません。");
            return;
        }
        //生成したデータをDBに登録
        api_registObi.save({ Imgdata: imgData, BookData: $scope.bookData, LoginInfo: $scope.loginInfo }, function () {
            alert("帯を登録しました。");
            modalInstance.close();
            $location.path("/myPage");
        },
            //失敗時
            function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
        );
    }

    /*画像選択ボタンクリック*/
    $scope.btnSelectImgClick = function (tgt) {

        $scope.StmpFolders = [{ Name: "テスト", Num: 2 }, { Name: "乗り物", Num: 1 }];
        $scope.CurrentFolder = $scope.StmpFolders[0];
        $scope.SelectImgTarget = tgt;   //値を返すターゲット

        //ダイアログを出す
        modalInstance_Img = $modal.open({
            templateUrl: "W_SelectImg",
            scope: $scope
        });
    }

    //画像選択ダイアログ内:カテゴリクリック
    $scope.showImgFilesInFolder = function (stmpFol) {
        $scope.CurrentFolder = stmpFol;
    }

    //画像選択ダイアログ内:画像クリック
    $scope.returnImgPath = function (url) {
        $scope.SelectImgTarget.Tp2.ImgPath = url;
        modalInstance_Img.close();
    }

    //画像選択ダイアログ内ファイルアップクリック
    $scope.btnUploadCkick = function () {
        if (!$scope.file) {
            alert("エラー：画像データがありません。");
            return;
        }
        //if ($scope.StmpFolders["MY"] && $scope.StmpFolders["MY"].Num > 20) {
        //    alert("エラー：これ以上アップロードできません。不要なファイルを削除してください。");
        //    return;
        //}

        //生成したデータをDBに登録
        //post
        var fd = new FormData();
        fd.append('logininfo', $scope.loginInfol);
        fd.append('file', $scope.file);
        $http.post('ObiOne/Upload', fd, {
            transformrequest: null,
            headers: { 'content-type': undefined }
        })
        .success(function () {
            alert("aa");
        });

        //pi_upload.save(fd);
        //, function () {
        //    alert("アップ。");
        //    //modalInstance.close();
        //    //$location.path("/myPage");
        //},
        //    //失敗時
        //    function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
        //);

        //var stage = new createjs.Stage("c2");
        // preload.jsで読み込んだ素材を保持する変数。
        //var assets = {};

        // preloadjsを使って画像を読み込む。
        //var imgPath = "https://cdn.teratail.com/img/about/imgAboutLogo.png";
        //var loadManifest = [{ id: "img", src: imgPath }];
        //var loader = new PreloadJS(false);
        //loader.onFileLoad = function (event) {
        //    assets[event.id] = event.result;
        //}
        //loader.onComplete = function () {
        //    var img = new createjs.Bitmap(assets["img"]);
        //    stage.addChild(img);
        //    stage.update();
        //    var fl = stage.toDataURL();

        //    api_upload.save({ File:  fl, LoginInfo: $scope.loginInfo }, function () {
        //        alert("ファイルアップロードしました。");   
        //    },
        //    失敗時
        //    function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
        //);
        //}
        //loader.loadManifest(loadManifest);

        

       
        
    }


}]);