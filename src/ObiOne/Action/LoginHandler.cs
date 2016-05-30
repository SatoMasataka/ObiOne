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

        private static string SQLITE_PATH { get { return Startup.Configuration["AppPath:SqlitePath"]; } }

        /// <summary>
        /// OAuthのユーザー情報
        /// </summary>
        public UserInfoModel UserInfoOfGoogle {
            get {
                if (_userInfo == null)
                    _userInfo = GetUserInfo(this.AccessToken);
                return _userInfo;
            }
        }
        UserInfoModel _userInfo;

        /// <summary>
        /// ログイン情報受け渡し用
        /// </summary>
        public LoginInfoModel LoginInfo { get; set; }

        /// <summary>
        /// 本アプリで登録されているかチェック
        /// </summary>
        /// <returns>0:未登録　1:登録済み</returns>
        public string IsAlmostLogin(string accessToken) {
            AccessToken = accessToken;

            //基本的にここには引っかからないはず
            if (this.UserInfoOfGoogle == null) throw new Exception("Googleのログインデータが取得できません");
            this.LoginInfo = getUserInfoFromGoogleId(this.UserInfoOfGoogle.id);


            if (this.LoginInfo !=null)   //すでに登録済みの場合
                return "1";
            else //未登録＝これから新規登録の場合
            {
                this.LoginInfo = new LoginInfoModel() { AccessToken = accessToken };

                //グーグル登録名を
                this.LoginInfo.Name = UserInfoOfGoogle.name;
                this.LoginInfo.GoogleId = this.UserInfoOfGoogle.id;
                return "0";
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
        /// 認証チェック・ログイン情報整合性チェック
        /// </summary>
        /// <param name="infoFromCli"></param>
        /// <returns></returns>
        public static bool Auth_LoginInfoCheck(LoginInfoModel infoFromCli)
        {
            try
            {
                //googleアカウント
                var gooA = GetUserInfo(infoFromCli.AccessToken);
                //アプリアカウント
                var appA = getUserInfoFromGoogleId(gooA.id);

                //クライアントから上がってきた情報と齟齬があるとNG
                if (appA.Id != infoFromCli.Id)
                    return false;
                return true;
            }
            catch { return false; }

        }

        /// <summary>
        /// アクセストークンからシステムでのID取得
        /// </summary>
        /// <param name="accToken"></param>
        /// <returns></returns>
        public static string GetIdFromToken(string accToken)
        {
            return getUserInfoFromGoogleId(GetUserInfo(accToken).id).Id;
        }
        /// <summary>
        /// アクセストークンを元にGoogle認証データ取得
        /// </summary>
        /// <param name="accessToken"></param>
        private static UserInfoModel GetUserInfo(string accToken)
        {
            WebClient wc = new WebClient();
            NameValueCollection nameValue = new NameValueCollection();

            //string ac = string.IsNullOrEmpty(accToken) ? AccessToken : accToken;
            nameValue.Add("access_token", accToken);
            wc.QueryString = nameValue;
            
            wc.Headers.Add("user-agent",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
            wc.Headers.Add("Content-type", "charset=UTF-8");
            
            string result = wc.DownloadString(@"https://www.googleapis.com/oauth2/v2/userinfo");
            return JsonConvert.DeserializeObject<UserInfoModel>(result);
        }

        /// <summary>
        /// gooogleIDからユーザー情報取得
        /// ユーザー未登録ならnullを返す
        /// </summary>
        /// <param name="googleId"></param>
        /// <returns></returns>
        private static LoginInfoModel getUserInfoFromGoogleId(string googleId)
        {
            //DBにユーザーデータがあるかチェック
            using (var conn = new SQLiteConnection("Data Source=" + SQLITE_PATH))
            {
                //登録データの取得
                conn.Open();
                SQLiteCommand command = conn.CreateCommand();
                command.CommandText = "SELECT ID,NAME,GOOGLE_ID " +
                                              "FROM USER_INFO " +
                                              "WHERE GOOGLE_ID = @google_id " +
                                              "AND DELETE_FLG = @delete_flg; ";
                command.Parameters.Add("google_id", System.Data.DbType.String).Value = googleId;
                command.Parameters.Add("delete_flg", System.Data.DbType.Int16).Value = 0;
                int count = 0;
                LoginInfoModel loginInfo = null;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (!reader.HasRows) continue;

                        //通常はここに引っかからないはず
                        if (count > 0) throw new Exception("登録データが複数件存在します。");

                        loginInfo = new LoginInfoModel();
                        loginInfo.Id = Convert.ToString(reader["ID"].ToString());
                        loginInfo.Name = Convert.ToString(reader["NAME"].ToString());
                        loginInfo.GoogleId = Convert.ToString(reader["GOOGLE_ID"].ToString());
                        count++;
                    }
                }
                return loginInfo;
            }
        }
    }
}
