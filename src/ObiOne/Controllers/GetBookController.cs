using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using System.IO;
using System.Web;
using ObiOne.Action;
using ObiOne.Model;
using System.Net;
using WebApi.OutputCache.V2;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ObiOne.Controllers
{
    /// <summary>
    /// 書誌情報取得
    /// </summary>
    [Route("ObiOne/[controller]")]
    public class GetBookController : Controller
    {
        // GET: api/values
        [HttpGet]
        [CacheOutput(NoCache = true)]
        public RootObject Get(string title, string author, int page = 1)
        {
            RakutenApiHandler r = new RakutenApiHandler();
            r.Author = author;
            r.Title = title;
            r.Page = page;
            return r.GetBookInfo();
        }

        [HttpPost()]
        public object Post([FromBody]PostecData pos)
        {
            //画像をサーバーに保存
            string url = pos.ImageUrl.ToString();
            return new{ Path = RakutenApiHandler.SavePicture(url)};

        }
        public class PostecData
        {
            public Uri ImageUrl{get;set;}
            public string test { get; set; }
        }
    }
}
