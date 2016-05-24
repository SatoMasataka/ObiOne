using Newtonsoft.Json;
using ObiOne.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ObiOne.Action
{
    /// <summary>
    /// GoogleApiからの
    /// </summary>
    public class RakutenApiHandler
    {
        /// <summary>
        /// 楽天書籍APIのURL
        /// </summary>
        private const string RakutenUrl = "https://app.rakuten.co.jp/services/api/BooksBook/Search/20130522";

        /// <summary>
        /// 検索条件：タイトル
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 検索条件：著者
        /// </summary>
        public string Author { get; set; }

        public int Page { get; set; }

        /// <summary>
        /// 書誌情報取得
        /// </summary>
        public RootObject GetBookInfo()
        {
            WebClient wc = new WebClient();

            //パラメタセット
            NameValueCollection nameValue = new NameValueCollection();
            nameValue.Add("applicationId", "1066764158692643350");
            nameValue.Add("affiliateId", "14d99458.7ef47fc5.14d99459.b7b2a47f");
            nameValue.Add("formatVersion", "2");
            nameValue.Add("size", "0");
            nameValue.Add("outOfStockFlag", "1");
            
            if (!string.IsNullOrEmpty(Title))
                nameValue.Add("title", HttpUtility.UrlEncode(Title, Encoding.UTF8));
            if (!string.IsNullOrEmpty(Author))
                nameValue.Add("author", HttpUtility.UrlEncode(Author, Encoding.UTF8));
            nameValue.Add("page", HttpUtility.UrlEncode(this.Page.ToString(), Encoding.UTF8));
            wc.QueryString = nameValue;

            wc.Headers.Add("user-agent",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
            wc.Headers.Add("Content-type", "charset=UTF-8");

            string result = wc.DownloadString(RakutenUrl);

            return JsonConvert.DeserializeObject<RootObject>(result);

        }

        /// <summary>
        /// 表紙画像をサーバーに保存
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string SavePicture(string url)
        {
            string filePath= Path.Combine(Setting.PICT_TEMP_PATH, Util.NewPictName());

            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(url);
            Bitmap src = new Bitmap(stream);
            stream.Close();

            src.Save(filePath, ImageFormat.Jpeg);
          
            return Util.GenerateServerPath(filePath);
        }
    }
}
