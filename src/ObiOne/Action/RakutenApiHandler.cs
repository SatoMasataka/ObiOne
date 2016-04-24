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
            nameValue.Add("formatVersion", "2");
            nameValue.Add("size", "0");
            if(!string.IsNullOrEmpty(Title))
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
        /// 表紙画像を引き伸ばし
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string MakePictureHighQuality(string url)
        {
            string tenpFolder=Startup.Configuration["AppPath:PictTempPath"];
            string filePath= Path.Combine(tenpFolder, Guid.NewGuid() +
                "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");

            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(url);
            Bitmap src = new Bitmap(stream);
            stream.Close();

            int scalePoint = 800 / src.Height;
            int w = src.Width * scalePoint;
            int h = 800;

            Bitmap dest = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(dest);

            g.InterpolationMode = InterpolationMode.Bicubic;
            g.DrawImage(src, 0, 0, w, h);
            dest.Save(filePath, ImageFormat.Jpeg);

            g.Dispose();
            dest.Dispose();

            return Util.GenerateServerPath(filePath);
        }
    }
}
