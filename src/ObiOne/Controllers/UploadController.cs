using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ObiOne.Model;
using static System.Net.WebRequestMethods;
using System.Web;
using System.IO;
using Microsoft.AspNet.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ObiOne.Controllers
{
    [Microsoft.AspNet.Mvc.Route("ObiOne/Upload")]
    public class UploadController : System.Web.Http.ApiController
    {
        // POST api/values
        [HttpPost()]
        public void Post()
        {
            var c = Request.Content;
        }
    }
}
