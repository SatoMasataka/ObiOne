using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Model
{
    /// <summary>
    /// 本システム自前のログイン情報
    /// </summary>
    public class LoginInfoModel
    {
        /// <summary>
        /// このシステムでのID
        /// </summary>
        public string Id { get; set; }
        public string Name { get; set; }
        public string GoogleId { get; set; }

    }
}
