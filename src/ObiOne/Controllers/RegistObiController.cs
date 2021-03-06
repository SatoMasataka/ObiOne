﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using ObiOne.Model;
using ObiOne.Action;
using WebApi.OutputCache.V2;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ObiOne.Controllers
{
    [Route("ObiOne/[controller]")]
    public class RegistObiController : Controller
    {

        // GET api/values/5
        [HttpGet()]
        [CacheOutput(NoCache = true)]
        public List<ObiInfoModel> Get(string id)
        {
            //ユーザーIDから帯を取得
            RegistObiHandler roh = new RegistObiHandler();
            return roh.GetObi(id);
        }

        /// <summary>
        /// 帯新規登録
        /// </summary>
        /// <param name="pos"></param>
        [HttpPost()]
        public void Post([FromBody] PostedData pos)
        {            
            //情報チェック
            if (!LoginHandler.Auth_LoginInfoCheck(pos.LoginInfo)) throw new Exception("ログイン情報不正");

            //帯を保存
            RegistObiHandler roh = new RegistObiHandler();
            roh.RegistObi(pos.BookData, pos.Imgdata, pos.LoginInfo);
         
        }

        // DELETE api/values/5
        [HttpDelete()]
        public void Delete(string obiId,string accessToken)
        {
            //所有者のIDを取得
            string id = LoginHandler.GetIdFromToken(accessToken);

            RegistObiHandler roh = new RegistObiHandler();
            roh.DeleteObi(obiId,id);
        }

        public class PostedData
        {
            public string Imgdata { get; set; }
            public Item BookData { get; set; }
            public LoginInfoModel LoginInfo { get; set; }
        }
    }
}
