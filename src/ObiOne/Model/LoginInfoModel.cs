using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Model
{
    /// <summary>
    /// 本システムのログイン情報(ログインユーザー当人用)
    /// </summary>
    public class LoginInfoModel: LoginInfoModel_Public
    {
        /// <summary>
        /// このシステムでのID
        /// </summary>
        public string Id { get; set; }
       
        public string GoogleId { get; set; }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            if (string.IsNullOrEmpty(this.Id))
                return false;
            if (string.IsNullOrEmpty(this.Name))
                return false;

            return true;
        }

    }

    /// <summary>
    /// 本システムのログイン情報(他ユーザーにも公開してよい部分)
    /// </summary>
    public class LoginInfoModel_Public
    {
        public string Name { get; set; }
    }
}
