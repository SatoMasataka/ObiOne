using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Action
{
    public class Util
    {
        /// <summary>
        /// サーバーパス⇒クライアントパス
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GenerateServerPath(string path)
        {
            //区切り文字
            int rootIndex = path.IndexOf("wwwroot");
            return "http://" + path.Substring(rootIndex).Replace("wwwroot", Startup.Configuration["RootAddress"]);
        }
    }
}
