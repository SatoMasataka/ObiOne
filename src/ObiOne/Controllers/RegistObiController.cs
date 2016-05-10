using System;
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

        // POST api/values
        [HttpPost()]
        public void Post([FromBody] RegistObiInfo inf)
        {
            //帯を保存
            RegistObiHandler roh = new RegistObiHandler();
            roh.RegistObi(inf.BookData, inf.Imgdata, inf.LoginInfo);
         
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete()]
        public void Delete(string obiId)
        {
            RegistObiHandler roh = new RegistObiHandler();
            roh.DeleteObi(obiId);


        }

        public class RegistObiInfo
        {
            public string Imgdata { get; set; }
            public Item BookData { get; set; }
            public LoginInfoModel LoginInfo { get; set; }
        }
    }
}
