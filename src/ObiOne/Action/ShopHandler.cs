using ObiOne.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Action
{
    public class ShopHandler
    {
        public ShopHandler() { this.Now = DateTime.Now; }

        /// <summary>
        /// 現在時刻
        /// </summary>
        DateTime Now { get; set; }

        /// <summary>
        /// 新規登録
        /// </summary>
        public void ReagistShop(ShopInfoModel shopInfo, LoginInfoModel loginInfo)
        {
            //DBへの登録処理
            using (var conn = new SQLiteConnection("Data Source=" + Setting.SQLITE_PATH))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand command = conn.CreateCommand();

                    /////////////////////////////////
                    //店舗情報への格納
                    /////////////////////////////////
                    command.CommandText = "INSERT INTO SHOP_INFO (SHOP_ID,ID, SHOP_NAME, SHOP_COMMENT, BACK_IMG,LAYOUT, REGIST_DATE, DELETE_FLG) " +
                                         "VALUES (@SHOP_ID,@ID,@SHOP_NAME , @SHOP_COMMENT, @BACK_IMG , @LAYOUT , @REGIST_DATE, '0');";


                    //店舗IDはユーザーID＋タイムスタンプ
                    string shopId = loginInfo.Id + "_" + Now.ToString("yyyyMMddHHmmssfff");

                    command.Parameters.Add("SHOP_ID", System.Data.DbType.String).Value = shopId;
                    command.Parameters.Add("ID", System.Data.DbType.String).Value = loginInfo.Id;
                    command.Parameters.Add("SHOP_NAME", System.Data.DbType.String).Value = shopInfo.ShopName;
                    command.Parameters.Add("SHOP_COMMENT", System.Data.DbType.String).Value = shopInfo.ShopComment;
                    command.Parameters.Add("BACK_IMG", System.Data.DbType.String).Value = shopInfo.BackImg;
                    command.Parameters.Add("LAYOUT", System.Data.DbType.String).Value = shopInfo.Layout;

                    command.Parameters.Add("REGIST_DATE", System.Data.DbType.String).Value =Now.ToString("yyyy/MM/dd HH:mm:ss");
                    command.ExecuteNonQuery();

                    //店舗＿帯情報更新
                    registShopObi(conn, shopId, shopInfo.ShopObiList);

                    trans.Commit();
                }
            
            }
        }
        /// <summary>
        /// 書店更新
        /// </summary>
        /// <param name="shopInfo"></param>
        /// <param name="loginInfo"></param>
        public void UpdateShop(ShopInfoModel shopInfo, LoginInfoModel loginInfo)
        {           
            using (var conn = new SQLiteConnection("Data Source=" + Setting.SQLITE_PATH))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand command = conn.CreateCommand();

                    /////////////////////////////////
                    //店舗情報更新
                    /////////////////////////////////
                    command.CommandText = "UPDATE SHOP_INFO "+
                                          "SET SHOP_NAME =@SHOP_NAME, SHOP_COMMENT=@SHOP_COMMENT, BACK_IMG=@BACK_IMG,LAYOUT=@LAYOUT " +
                                          "WHERE SHOP_ID = @SHOP_ID AND DELETE_FLG = '0';";


                    command.Parameters.Add("SHOP_ID", System.Data.DbType.String).Value = shopInfo.ShopId;
                    command.Parameters.Add("SHOP_NAME", System.Data.DbType.String).Value = shopInfo.ShopName;
                    command.Parameters.Add("SHOP_COMMENT", System.Data.DbType.String).Value = shopInfo.ShopComment;
                    command.Parameters.Add("BACK_IMG", System.Data.DbType.String).Value = shopInfo.BackImg;
                    command.Parameters.Add("LAYOUT", System.Data.DbType.String).Value = shopInfo.Layout;

                    int up = command.ExecuteNonQuery();

                    if (up != 1) throw new Exception("更新失敗");

                    //店舗＿帯情報更新
                    registShopObi(conn, shopInfo.ShopId, shopInfo.ShopObiList);

                    trans.Commit();
                }

            }
        }
        /// <summary>
        /// 店舗削除
        /// </summary>
        /// <param name="shopInfo"></param>
        /// <param name="loginInfo"></param>
        public void DeleteShop(string shopId,string id)
        {
            using (var conn = new SQLiteConnection("Data Source=" + Setting.SQLITE_PATH))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand command = conn.CreateCommand();

                    /////////////////////////////////
                    //店舗情報更新
                    /////////////////////////////////
                    command.CommandText = "UPDATE SHOP_INFO " +
                                          "SET DELETE_FLG = '1' " +
                                          "WHERE SHOP_ID = @SHOP_ID "+
                                          "AND ID = @ID;";


                    command.Parameters.Add("SHOP_ID", System.Data.DbType.String).Value = shopId;
                    command.Parameters.Add("ID", System.Data.DbType.String).Value = id;
                    if( command.ExecuteNonQuery()<1) throw new Exception("削除対象がありません");

                    trans.Commit();
                }
            }
        }

        /// <summary>
        /// ユーザーIDから店舗一覧取得
        /// (店舗配置書籍情報は取得しない)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ShopInfoModel> GetUsersShops(string id)
        {
            using (var conn = new SQLiteConnection("Data Source=" + Setting.SQLITE_PATH))
            {
                //登録データの取得
                conn.Open();
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * " +
                                      "FROM SHOP_INFO " +
                                      "WHERE ID = @id " +
                                      "AND DELETE_FLG = 0 " +
                                      "ORDER BY REGIST_DATE DESC;";
                command.Parameters.Add("id", System.Data.DbType.String).Value = id;

                List<ShopInfoModel> ret = new List<ShopInfoModel>();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.HasRows) continue;
                        ShopInfoModel mdl = new ShopInfoModel();

                        mdl.Id = Convert.ToString(reader["ID"].ToString());

                        mdl.BackImg = Convert.ToString(reader["BACK_IMG"].ToString());
                        mdl.Layout= Convert.ToString(reader["LAYOUT"].ToString());
                        mdl.ShopComment = Convert.ToString(reader["SHOP_COMMENT"].ToString());
                        mdl.ShopId = Convert.ToString(reader["SHOP_ID"].ToString());
                        mdl.ShopName = Convert.ToString(reader["SHOP_NAME"].ToString());
                        mdl.RegistDate = Convert.ToString(reader["REGIST_DATE"].ToString());

                        ret.Add(mdl);
                    }
                }
                return ret;
            }
        }


        /// <summary>
        /// 店舗IDから店舗情報取得
        /// </summary>
        /// <param name="shopId"></param>
        /// <returns></returns>
        public ShopInfoModel GetShop(string shopId) {
            using (var conn = new SQLiteConnection("Data Source=" + Setting.SQLITE_PATH))
            {
                
                conn.Open();
                SQLiteCommand command = conn.CreateCommand();
                ShopInfoModel ret = new ShopInfoModel();

                /////////////
                //店舗情報
                ////////
                command.CommandText = "SELECT S.ID AS ID, " +
                                      "    S.BACK_IMG AS BACK_IMG, " +
                                      "    S.LAYOUT AS LAYOUT, " +
                                      "    S.SHOP_COMMENT AS SHOP_COMMENT, " +
                                      "    S.SHOP_ID AS SHOP_ID, " +
                                      "    S.SHOP_NAME AS SHOP_NAME, " +
                                      "    S.REGIST_DATE AS REGIST_DATE, " +
                                      "    U.NAME AS NAME " +
                                      "FROM SHOP_INFO S " +
                                      "INNER JOIN USER_INFO U " +
                                      "    ON U.ID = S.ID " +
                                      "WHERE S.SHOP_ID = @shopId " +
                                      "AND U.DELETE_FLG = 0; "+
                                      "AND S.DELETE_FLG = 0; ";

                command.Parameters.Add("shopId", System.Data.DbType.String).Value = shopId;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.HasRows) return null;//空ならおしまい
                        
                        ret.Id = Convert.ToString(reader["ID"].ToString());
                        ret.BackImg = Convert.ToString(reader["BACK_IMG"].ToString());
                        ret.Layout = Convert.ToString(reader["LAYOUT"].ToString());
                        ret.ShopComment = Convert.ToString(reader["SHOP_COMMENT"].ToString());
                        ret.ShopId = Convert.ToString(reader["SHOP_ID"].ToString());
                        ret.ShopName = Convert.ToString(reader["SHOP_NAME"].ToString());
                        ret.RegistDate = Convert.ToString(reader["REGIST_DATE"].ToString());
                        ret.OwnerInfo.Name = Convert.ToString(reader["NAME"].ToString());
                    }
                }

                /////////////
                //店舗-帯画像情報
                //////////
                command = conn.CreateCommand();
                command.CommandText = "SELECT * " +
                                      "FROM SHOP_OBI SO " +
                                      "INNER JOIN OBI_INFO O " +
                                      "  ON SO.OBI_ID = O.OBI_ID " +
                                      "WHERE SO.SHOP_ID = @shopId " +
                                      "AND O.DELETE_FLG = 0 "+
                                      "ORDER BY SO.ORDER_SEQ ";

                command.Parameters.Add("shopId", System.Data.DbType.String).Value = shopId;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.HasRows) continue;
                        ShopObiModel mdl = new ShopObiModel();

                        mdl.OrderSeq = int.Parse(Convert.ToString(reader["ORDER_SEQ"].ToString()));


                        //todo 以下切り出し
                        mdl.ObiInfo.Id = Convert.ToString(reader["ID"].ToString());
                        string imgP = Convert.ToString(reader["IMG_PATH"].ToString());
                        mdl.ObiInfo.ImgPath = Util.GenerateServerPath(imgP);

                        mdl.ObiInfo.ObiId = Convert.ToString(reader["OBI_ID"].ToString());
                        mdl.ObiInfo.Author = Convert.ToString(reader["AUTHOR"].ToString());
                        mdl.ObiInfo.BookTitle = Convert.ToString(reader["BOOK_TITLE"].ToString());
                        mdl.ObiInfo.Link = Convert.ToString(reader["LINK"].ToString());
                        mdl.ObiInfo.Publisher = Convert.ToString(reader["PUBLISHER"].ToString());
                        mdl.ObiInfo.RegistDate = Convert.ToString(reader["REGIST_DATE"].ToString());

                        ret.ShopObiList.Add(mdl);
                    }
                }

                //歯抜け分を補完
                for(int i = 0; i < ShopInfoModel.ShopObiNum; i++)
                {
                    int cnt = (from l in ret.ShopObiList
                               where l.OrderSeq == i
                               select "1").Count();

                    if (cnt == 0)
                        ret.ShopObiList.Add(new ShopObiModel() { OrderSeq = i });
                }
                ret.ShopObiList = (from l in ret.ShopObiList
                                   orderby l.OrderSeq
                                   select l).ToList();
                return ret;
            }
        }



        /// <summary>
        /// 店舗・帯情報更新
        /// 一旦全消ししてからインサート
        /// </summary>
        private void registShopObi(SQLiteConnection conn,string shopId,List<ShopObiModel> obiList)
        {
            var command=conn.CreateCommand();

            //削除処理
            command.CommandText = "DELETE FROM SHOP_OBI WHERE SHOP_ID=@SHOP_ID;";
            command.Parameters.Add("SHOP_ID", System.Data.DbType.String).Value = shopId;
            command.ExecuteNonQuery();

            //新規インサート
            foreach(var o in obiList)
            {
                command.CommandText = "INSERT INTO SHOP_OBI (SHOP_ID, OBI_ID, ORDER_SEQ, REGIST_DATE) " +
                                         "VALUES (@SHOP_ID, @OBI_ID, @ORDER_SEQ, @REGIST_DATE);";

                command.Parameters.Add("SHOP_ID", System.Data.DbType.String).Value = shopId;
                command.Parameters.Add("OBI_ID", System.Data.DbType.String).Value = string.IsNullOrEmpty(o.ObiInfo.ObiId)?"": o.ObiInfo.ObiId ;
                command.Parameters.Add("ORDER_SEQ", System.Data.DbType.Int16).Value = o.OrderSeq;
                command.Parameters.Add("REGIST_DATE", System.Data.DbType.String).Value = Now.ToString("yyyy/MM/dd HH:mm:ss");

                command.ExecuteNonQuery();
            }
        }
    }
}
