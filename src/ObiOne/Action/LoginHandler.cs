using Newtonsoft.Json;
using ObiOne.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ObiOne.Action
{
    public class LoginHandler
    {
        public LoginHandler() {
        }

        /// <summary>
        /// OAuthから返ってきたトークン
        /// </summary>
        private string AccessToken;

        private string SQLITE_PATH { get { return Startup.Configuration["AppPath:SqlitePath"]; } }

        /// <summary>
        /// OAuthのユーザー情報
        /// </summary>
        public UserInfoModel UserInfoOfGoogle {
            get {
                if (_userInfo == null)
                    _userInfo = GetUserInfo();
                return _userInfo;
            }
        }
        UserInfoModel _userInfo;

        /// <summary>
        /// ログイン情報受け渡し用
        /// </summary>
        public LoginInfoModel LoginInfo { get; set; }

        /// <summary>
        /// ログインされているかチェック
        /// </summary>
        /// <returns>0:未登録　1:登録済み</returns>
        public string IsAlmostLogin(string accessToken) {
            AccessToken = accessToken;

            //基本的にここには引っかからないはず
            if (this.UserInfoOfGoogle == null) throw new Exception("Googleのログインデータが取得できません");

            //DBにユーザーデータがあるかチェック
            using (var conn = new SQLiteConnection("Data Source=" + SQLITE_PATH))
            {
                //登録データの取得
                conn.Open();
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = "SELECT ID,NAME,GOOGLE_ID " +
                                      "FROM USER_INFO "+
                                      "WHERE GOOGLE_ID = @google_id " +
                                      "AND DELETE_FLG = @delete_flg; ";
                command.Parameters.Add("google_id", System.Data.DbType.String).Value = this.UserInfoOfGoogle.id;
                command.Parameters.Add("delete_flg", System.Data.DbType.Int16).Value = 0;
                int count = 0;
                this.LoginInfo = new LoginInfoModel();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        if (!reader.HasRows) continue;

                        //通常はここに引っかからないはず
                        if (count > 0) throw new Exception("登録データが複数件存在します。");

                        LoginInfo.Id = Convert.ToString(reader["ID"].ToString());
                        LoginInfo.Name = Convert.ToString(reader["NAME"].ToString());
                        LoginInfo.GoogleId = Convert.ToString(reader["GOOGLE_ID"].ToString());
                        count++;
                    }
                }

                if (count > 0)   //すでに登録済みの場合
                    return "1";
                else //未登録＝これから新規登録の場合
                {
                    //グーグル登録名を
                    this.LoginInfo.Name = UserInfoOfGoogle.name;
                    this.LoginInfo.GoogleId = this.UserInfoOfGoogle.id;
                    return "0";
                }
            }
        }

        /// <summary>
        /// 新規登録
        /// </summary>
        /// <param name="model"></param>
        public LoginInfoModel NewRegist(LoginInfoModel model)
        {
            using (var conn = new SQLiteConnection("Data Source=" + SQLITE_PATH))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    SQLiteCommand command = conn.CreateCommand();

                    /////////////////////////////////
                    //ユーザーテーブルへの格納
                    /////////////////////////////////
                    command.CommandText = "INSERT INTO USER_INFO (NAME , REGIST_DATE, DELETE_FLG ,GOOGLE_ID) " +
                                         "VALUES (@NAME , @REGIST_DATE, '0' ,@GOOGLE_ID);";

                    //IDはオートインクリメント
                    command.Parameters.Add("NAME", System.Data.DbType.String).Value = model.Name.ToString();
                    command.Parameters.Add("REGIST_DATE", System.Data.DbType.String).Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    command.Parameters.Add("GOOGLE_ID", System.Data.DbType.String).Value = model.GoogleId.ToString();
                    command.ExecuteNonQuery();

                    trans.Commit();

                    //以下きりだし
                    command.CommandText = "SELECT ID,NAME,GOOGLE_ID " +
                                      "FROM USER_INFO " +
                                      "WHERE GOOGLE_ID = @google_id " +
                                      "AND DELETE_FLG = @delete_flg; ";
                    command.Parameters.Add("google_id", System.Data.DbType.String).Value = model.GoogleId;
                    command.Parameters.Add("delete_flg", System.Data.DbType.Int16).Value = 0;
                    int count = 0;
                    this.LoginInfo = new LoginInfoModel();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            if (!reader.HasRows) break;

                            //通常はここに引っかからないはず
                            if (count > 0) throw new Exception("登録データが複数件存在します。");

                            LoginInfo.Id = Convert.ToString(reader["ID"].ToString());
                            LoginInfo.Name = Convert.ToString(reader["NAME"].ToString());
                            LoginInfo.GoogleId = Convert.ToString(reader["GOOGLE_ID"].ToString());
                            count++;
                        }
                    }
                }
            }
            return this.LoginInfo;
        }




        /// <summary>
        /// アクセストークンを元にGoogle認証データ取得
        /// </summary>
        /// <param name="accessToken"></param>
        private UserInfoModel GetUserInfo()
        {
            WebClient wc = new WebClient();
            NameValueCollection nameValue = new NameValueCollection();
            nameValue.Add("access_token", AccessToken);
            wc.QueryString = nameValue;
            
            wc.Headers.Add("user-agent",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
            wc.Headers.Add("Content-type", "charset=UTF-8");
            
            string result = wc.DownloadString(@"https://www.googleapis.com/oauth2/v2/userinfo");
            return JsonConvert.DeserializeObject<UserInfoModel>(result);
        }
    }
}
