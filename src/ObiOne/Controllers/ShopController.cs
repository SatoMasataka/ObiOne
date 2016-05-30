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
using ObiOne.Attribute;
using Microsoft.AspNet.Authorization;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ObiOne.Controllers
{
    /// <summary>
    /// 店舗情報
    /// </summary>
    [Route("ObiOne/[controller]")]
    public class ShopController : Controller
    {
        /// <summary>
        /// 店舗取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        [CacheOutput(NoCache = true)]
        public object Get(string id, string shopId,bool auth)
        {
            ShopHandler s = new ShopHandler();
            if (string.IsNullOrEmpty(shopId))
                return s.GetUsersShops(id); //特定のユーザーに紐付く書店一覧を返す
            else
                return s.GetShop(shopId); //特定の書店の情報を返す
        }

        /// <summary>
        /// 店舗新規登録・更新
        /// </summary>
        /// <param name="pos"></param>
        [HttpPost()]
        public void Post([FromBody]PostecData pos)
        {
            //情報チェック
            if (!LoginHandler.Auth_LoginInfoCheck(pos.LoginInfo)) throw new Exception("ログイン情報不正");
            if (!pos.ShopInfo.Check()) throw new Exception("店舗情報不正");

            ShopHandler s = new ShopHandler();
            //店舗IDの有無で判定
            if (string.IsNullOrEmpty(pos.ShopInfo.ShopId))
            {
                //新規登録
                s.ReagistShop(pos.ShopInfo, pos.LoginInfo);
            }
            else
            {
                //更新
                //todo店舗のっとりチェック
                s.UpdateShop(pos.ShopInfo, pos.LoginInfo);
            }
            return;
        }

        /// <summary>
        /// 店舗削除
        /// </summary>
        /// <param name="pos"></param>
        [HttpDelete()]
        public void Delete(string ShopId, string accessToken)
        {
            //所有者のIDを取得
            string id = LoginHandler.GetIdFromToken(accessToken);

            ShopHandler s = new ShopHandler();
            s.DeleteShop(ShopId,id);
        }

        /// <summary>
        /// ポスト用
        /// </summary>
        public class PostecData
        {
            public LoginInfoModel LoginInfo { get; set; }
            public ShopInfoModel ShopInfo { get; set; }
        }
    }
}
