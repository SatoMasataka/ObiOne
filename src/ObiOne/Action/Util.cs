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

        /// <summary>
        /// 重複しない画像名を生成
        /// </summary>
        /// <returns></returns>
        public static string NewPictName(string head="")
        {
            return head + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid()   + ".jpg";
        }
    }
}
