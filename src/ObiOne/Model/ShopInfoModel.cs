using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Model
{
    /// <summary>
    /// 店舗情報
    /// </summary>
    public class ShopInfoModel
    {
        public ShopInfoModel()
        {
            this.ShopObiList = new List<ShopObiModel>();
            this.OwnerInfo = new LoginInfoModel_Public();
        }
        /// <summary>
        /// 店舗あたりの書籍数
        /// </summary>
        public const int ShopObiNum = 8;

        #region 編集対象項目 : サバ←→クラ
        public string Id { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopComment { get; set; }
        public string BackImg { get; set; }
        public string Layout { get; set; }
        public string RegistDate { get; set; }
        public List<ShopObiModel> ShopObiList { get; set; }
        #endregion

        #region　非編集対象項目 : サバ→クラ
        /// <summary>
        /// 書店所有者情報
        /// </summary>
        public LoginInfoModel_Public OwnerInfo { get; set; }
        #endregion

        /// <summary>
        /// 項目チェック
        /// </summary>
        /// <returns></returns>
        public bool Check() {
            ///// 必須チェック /////
            if (string.IsNullOrEmpty(this.ShopName))
                return false;

            ////// seq連番・内容チェック //////
            var obiList = (from s in ShopObiList
                           orderby s.OrderSeq
                           select s).ToList();
            
            //所定数あるか
            if (obiList.Count != ShopObiNum) return false;

            int rightSeq = 0;
            bool allNull = true;//全て未選択フラグ
            foreach (var o in obiList)
            {
                //連番どおりか
                if (o.OrderSeq != rightSeq) return false;
                if (!string.IsNullOrEmpty(o.ObiInfo.ObiId))
                    allNull = false;

                rightSeq++;
            }

            //1つも選択されていなければNG
            if (allNull) return false;


            return true;
        }
    }

    /// <summary>
    /// 店舗配置書籍情報
    /// </summary>
    public class ShopObiModel
    {
        public ShopObiModel() {
            ObiInfo = new ObiInfoModel();
        }

        public int OrderSeq { get; set; }
        public ObiInfoModel ObiInfo { get; set; }
    }
}
