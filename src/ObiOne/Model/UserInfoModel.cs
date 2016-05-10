using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Model
{
    /// <summary>
    /// OAuthのログイン情報
    /// </summary>
    public class UserInfoModel
    {
        public string id { get; set; }
        public string email { get; set; }
        public bool verifiedEmail { get; set; }
        public string name { get; set; }
        public string givenName { get; set; }
        public string familyName { get; set; }
        public string link { get; set; }
        public string picture { get; set; }
        public string gender { get; set; }
        public string locale { get; set; }

    }
}
