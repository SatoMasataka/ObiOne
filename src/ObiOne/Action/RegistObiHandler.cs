using ObiOne.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ObiOne.Action
{
    public class RegistObiHandler
    {
        /// <summary>
        /// ユーザーIDから帯を取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ObiInfoModel> GetObi(string id)
        {
            //帯情報取得
            using (var conn = new SQLiteConnection("Data Source=" + Setting.SQLITE_PATH))
            {
                //登録データの取得
                conn.Open();
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = "SELECT * " +
                                      "FROM OBI_INFO " +
                                      "WHERE ID = @id "+
                                      "AND DELETE_FLG = 0 "+
                                      "ORDER BY REGIST_DATE DESC;";
                command.Parameters.Add("id", System.Data.DbType.String).Value = id;

                List<ObiInfoModel> ret =new List<ObiInfoModel>();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.HasRows) continue;
                        ObiInfoModel mdl = new ObiInfoModel();

                        mdl.Id = Convert.ToString(reader["ID"].ToString());
                        string imgP = Convert.ToString(reader["IMG_PATH"].ToString());
                        mdl.ImgPath = Util.GenerateServerPath(imgP);

                        mdl.ObiId = Convert.ToString(reader["OBI_ID"].ToString());
                        mdl.Author = Convert.ToString(reader["AUTHOR"].ToString());
                        mdl.BookTitle = Convert.ToString(reader["BOOK_TITLE"].ToString());
                        mdl.Link = Convert.ToString(reader["LINK"].ToString());
                        mdl.Publisher = Convert.ToString(reader["PUBLISHER"].ToString());
                        mdl.RegistDate = Convert.ToString(reader["REGIST_DATE"].ToString());

                        ret.Add(mdl);
                    }
                }
                return ret;
            }
        }


        /// <summary>
        /// 帯の新規登録
        /// </summary>
        /// <param name="model"></param>
        public void RegistObi(Item bookData,string imgData, LoginInfoModel loginInfo)
        {
            // Base64で上がってくる画像データをjpgで保存
            byte[] bs = System.Convert.FromBase64String(imgData.Split(',')[1]);

            System.Drawing.Image bmp = this.ConvertBytesToImage(bs);
            string savePath = Path.Combine(Setting.PICT_PATH, Util.NewPictName());
            bmp.Save(savePath);

            //DBへの登録処理
            using (var conn = new SQLiteConnection("Data Source=" + Setting.SQLITE_PATH))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand command = conn.CreateCommand();

                    /////////////////////////////////
                    //帯テーブルへの格納
                    /////////////////////////////////
                    command.CommandText = "INSERT INTO OBI_INFO (ID, IMG_PATH,BOOK_TITLE,AUTHOR,PUBLISHER , LINK , REGIST_DATE, DELETE_FLG) " +
                                         "VALUES (@ID,@IMG_PATH , @BOOK_TITLE, @AUTHOR , @PUBLISHER ,@LINK, @REGIST_DATE, '0');";

                    //IDはオートインクリメント
                    command.Parameters.Add("ID", System.Data.DbType.String).Value = loginInfo.Id;
                    command.Parameters.Add("IMG_PATH", System.Data.DbType.String).Value = savePath;
                    command.Parameters.Add("BOOK_TITLE", System.Data.DbType.String).Value = bookData.title;
                    command.Parameters.Add("AUTHOR", System.Data.DbType.String).Value = bookData.author;
                    command.Parameters.Add("PUBLISHER", System.Data.DbType.String).Value = bookData.publisherName;
                    command.Parameters.Add("LINK", System.Data.DbType.String).Value = bookData.affiliateUrl;

                    command.Parameters.Add("REGIST_DATE", System.Data.DbType.String).Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    command.ExecuteNonQuery();

                    trans.Commit();
                }
            }
        }

        /// <summary>
        /// 帯の削除
        /// </summary>
        /// <param name="model"></param>
        public void DeleteObi(string obiId)
        {
          
            //DBへの登録処理
            using (var conn = new SQLiteConnection("Data Source=" + Setting.SQLITE_PATH))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand command = conn.CreateCommand();

                    /////////////////////////////////
                    //帯テーブルへの格納
                    /////////////////////////////////
                    command.CommandText = "UPDATE OBI_INFO SET DELETE_FLG = '1' " +
                                          "WHERE OBI_ID = @OBI_ID ;";

                    command.Parameters.Add("OBI_ID", System.Data.DbType.String).Value = obiId;

                    command.ExecuteNonQuery();

                    trans.Commit();
                }
            }
        }

        // ==============
        // Byte型配列(バイナリ)をImageに変換
        // 第１引数: Byte型配列(バイナリ)の画像情報
        // 戻り値: Image型画像情報
        private System.Drawing.Image ConvertBytesToImage(byte[] Image_Bytes)
        {
            // 入力引数の異常時のエラー処理
            if ((Image_Bytes == null) || (Image_Bytes.Length == 0))
            {
                return null;
            }

            // 返却用Image型オブジェクト
            System.Drawing.Image ResultImg;

            // バイナリ(Byte配列)をストリームに保存
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Image_Bytes))
            {
                // ストリームのバイナリ(Byte配列)をImageに変換
                ResultImg = System.Drawing.Image.FromStream(ms);

                // ストリームのクローズ
                ms.Close();
            }

            return ResultImg;
        }
    }
}
