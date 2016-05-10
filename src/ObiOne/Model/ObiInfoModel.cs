using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Model
{
    /// <summary>
    /// ObiInfo　テーブルモデル
    /// </summary>
    public class ObiInfoModel
    {
        public string ObiId { get; set; }
        public string Id { get; set; }
        public string ImgPath { get; set; }
        public string  Author { get; set; }
        public string BookTitle { get; set; }
        public string Publisher { get; set; }
        public string Link { get; set; }
        public string RegistDate { get; set; }
        public string DeleteFlg { get; set; }

    }
}
