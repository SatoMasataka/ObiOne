using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ObiOne.Action;
using ObiOne.Model;
using WebApi.OutputCache.V2;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ObiOne.Controllers
{
    [Route("ObiOne/[controller]")]
    public class LoginController : Controller
    {


        /// <summary>
        /// アクセストークン問い合わせ
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [HttpGet()]
        [CacheOutput(NoCache = true)]
        public GetReturnModel Get(string accessToken)
        {
            var glh =new LoginHandler();

            GetReturnModel ret = new GetReturnModel()
            {
                registFlg = glh.IsAlmostLogin(accessToken),
                loginInfo = glh.LoginInfo
            }; 
            return ret;
        }

        /// <summary>
        /// ユーザーの新規登録
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        public GetReturnModel Post([FromBody]GetReturnModel model)
        {
            //情報チェック
            if (string.IsNullOrEmpty( LoginHandler.GetIdFromToken(model.loginInfo.AccessToken) ))
                throw new Exception("ログイン情報不正");

            var glh = new LoginHandler();
            GetReturnModel ret = new GetReturnModel();
            ret.loginInfo = glh.NewRegist(model.loginInfo);
            return ret;
        }



        public class GetReturnModel
        {
            /// <summary>
            /// 0:未登録　1:登録済み
            /// </summary>
            public string registFlg { get; set; }
            public LoginInfoModel loginInfo { get; set; }
        }
    }
}
