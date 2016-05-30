myApp.controller('searchBookCtrl', ['$scope', '$resource', '$location', 'editObiObject',
    function ($scope, $resource, $location, editObiObject) {

     var apiw = $resource("ObiOne/GetBook/?Title=:title&Author=:author&Page=:page");

    //現在の表示内容の検索条件
    var NowAuthor="";
    var NowTitle = "";

    /*書誌情報検索*/
    $scope.SearchBook = function () {
        apiw.get(
            //クエリストリング
            { title: $scope.Title, author: $scope.Author, page: 1 },
            //成功時
            function (p) {
                $scope.Paging = wrappingPageIndex(p.pageCount);//総ページ数
                $scope.NowPage = p.page;
                //検索条件保持
                NowTitle = $scope.Title;
                NowAuthor = $scope.Author;

                $scope.BookInfo = wrappingSearchResult(p.Items);
            },
            //失敗時
            function (c) { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
        );
    }

    /*ページング変更*/
    $scope.ChangePage = function (pg) {
        apiw.get(
            //クエリストリング
            { title: NowTitle, author: NowAuthor,page:pg },
            //成功時
            function (p) {
                $scope.Paging = wrappingPageIndex(p.pageCount);//総ページ数
                $scope.NowPage = p.page;

                $scope.BookInfo = wrappingSearchResult(p.Items);
            },
            //失敗時
            function () { alert("通信エラー：しばらくしてからもう一度アクセスしてください。"); }
        );
    }

    /*帯編集ページへ遷移*/
    $scope.GoToEditObi = function (info) {
        editObiObject.bookData = info;
        $location.path("/editObi");
    }


    /*ここからプライベートメソッド*/
    //検索結果を行単位の塊でラッピング
    var wrappingSearchResult = function (items) {
        var bookInfo_wrap = [];
        var colNum = 4;//一行あたりの表示件数(固定)
        var count = 0;
        var work = [];
        for (var i = 0; i <items.length; i++) {
            if (count < colNum) {
                work.push(items[i]);
                count++;
            } else {
                bookInfo_wrap.push(work);
                work = [];
                count = 0;
            }
        }
        return bookInfo_wrap;
    }

    //ページング生成用
    var wrappingPageIndex=function(pageCount){
        var wrap=[];
        for(var i=0;i<pageCount;i++){
            var c={No:i+1};
            wrap.push(c);
        }
        return wrap;
    }
}]);