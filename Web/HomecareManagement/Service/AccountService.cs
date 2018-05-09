using HomecareManagement.Models.Web;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Service
{
    public class AccountService
    {
        String conStr = ConStr.getConstr(0);


        #region Account_Profile

        /// <summary>驗證用戶帳號密碼</summary>
        /// <param name="username">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns>用戶編號與階級</returns>
        public AccountModel selectAccount(String username, String password)
        {
            AccountModel account = new AccountModel();
            account.uid = 0;
            try
            {
                String sql = "SELECT"
                    + " u101b117_account.uid,"
                    + " u101b117_account.username,"
                    + " u101b117_account.password,"
                    + " u101b117_account.level,"
                    + " u101b117_info.displayname"
                    + " FROM u101b117_account"
                    + " JOIN u101b117_info"
                    + " ON u101b117_account.uid"
                    + " = u101b117_info.account_uid"
                    + " WHERE username=@username"
                    + " AND level != 5;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    if (mysqlDataReader["password"].ToString() == password)
                    {
                        account.uid = int.Parse(mysqlDataReader["uid"].ToString());
                        account.level = int.Parse(mysqlDataReader["level"].ToString());
                        account.displayname = mysqlDataReader["displayname"].ToString();
                    }
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                account.uid = -1;
            }
            return account;
        }

        /// <summary>取得使用者資料</summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public InfoModel selectInfo(int uid)
        {
            InfoModel info = new InfoModel();
            try
            {
                String sql = "SELECT *"
                    + " FROM u101b117_info"
                    + " WHERE account_uid=@account_uid;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@account_uid", uid);
                MySqlDataReader mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    info.displayname = mysqlDataReader["displayname"].ToString();
                    //String idcard = mysqlDataReader["idcard"].ToString();
                    //info.idcard = idcard.Substring(0, idcard.Length - 4) + "****";
                    info.birthday = DateTime.Parse(mysqlDataReader["birthday"].ToString()).ToString("yyyy/MM/dd");
                    info.sex = int.Parse(mysqlDataReader["sex"].ToString());
                    info.address = mysqlDataReader["address"].ToString();
                    info.phone1 = mysqlDataReader["phone1"].ToString();
                    info.phone2 = mysqlDataReader["phone2"].ToString();
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                info = null;
            }

            return info;
        }

        /// <summary>個人-更新使用者資料</summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int updateInfo(InfoModel info)
        {
            int result = 0;
            try
            {
                String sql = "UPDATE u101b117_info"
                    + " SET `displayname` = @displayname,"
                    + " `birthday` = @birthday,"
                    + " `sex` = @sex,"
                    + " `address` = @address,"
                    + " `phone1` = @phone1,"
                    + " `phone2` = @phone2"
                    + " WHERE `account_uid` = @account_uid;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@displayname", info.displayname);
                mysqlCmd.Parameters.AddWithValue("@birthday", DateTime.Parse(info.birthday).ToString("yyyy/MM/dd"));
                mysqlCmd.Parameters.AddWithValue("@sex", info.sex);
                mysqlCmd.Parameters.AddWithValue("@address", info.address);
                mysqlCmd.Parameters.AddWithValue("@phone1", info.phone1);
                mysqlCmd.Parameters.AddWithValue("@phone2", info.phone2);
                mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                result = mysqlCmd.ExecuteNonQuery();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }


        #endregion

    }
}