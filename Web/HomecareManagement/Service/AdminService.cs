using HomecareManagement.Models.Web;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace HomecareManagement.Service
{
    public class AdminService
    {
        DBHelper dbhelper;
        String conStr = ConStr.getConstr(0);

        #region Admin_Service

        /// <summary>搜尋服務項目名稱</summary>
        /// <returns></returns>
        public List<ServiceItemModel> selectService_ItemData()
        {
            var data = new List<ServiceItemModel>();
            String sql = @"SELECT *
                            FROM `u101b117_service_item`
                            WHERE isdelete = 0;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                try
                {
                    mysqlCmd.CommandText = sql;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(new ServiceItemModel
                        {
                            service_uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            service_name = mysqlDataReader["name"].ToString()
                        });
                    }
                    mysqlDataReader.Dispose();
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    data = null;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    data = null;
                }
                finally
                {
                    mysqlTransaction.Dispose();
                    mysqlCmd.Dispose();
                    mysqlCon.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                data = null;
            }
            return data;
        }


        /// <summary>搜尋後台表格[服務項目]資料</summary>
        /// <returns></returns>
        public List<ServiceItemModel> selectGridServiceInitData()
        {
            var data = new List<ServiceItemModel>();
            String sql = @"SELECT a.uid AS service_item_uid,
		                            a.name AS service_item_name,
		                            a.edit_time AS service_item_edit_time,
		                            a.isdelete AS service_item_isdelete,
		                            b.uid AS service_uid,
		                            b.name AS service_name,
		                            b.edit_time AS service_edit_time,
		                            b.isdelete AS service_isdelete
                            FROM `u101b117_service_item` AS a
                            JOIN `u101b117_service` AS b
                            ON a.uid = b.service_item_uid;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                try
                {
                    mysqlCmd.CommandText = sql;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(new ServiceItemModel
                        {
                            service_item_uid = int.Parse(mysqlDataReader["service_item_uid"].ToString()),
                            service_item_name = mysqlDataReader["service_item_name"].ToString(),
                            service_item_edit_time = DateTime.Parse(mysqlDataReader["service_item_edit_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                            service_item_isdelete = int.Parse(mysqlDataReader["service_item_isdelete"].ToString()),
                            service_uid = int.Parse(mysqlDataReader["service_uid"].ToString()),
                            service_name = mysqlDataReader["service_name"].ToString(),
                            service_edit_time = DateTime.Parse(mysqlDataReader["service_edit_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                            service_isdelete = int.Parse(mysqlDataReader["service_isdelete"].ToString())
                        });
                    }
                    mysqlDataReader.Dispose();
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    data = null;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    data = null;
                }
                finally
                {
                    mysqlTransaction.Dispose();
                    mysqlCmd.Dispose();
                    mysqlCon.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                data = null;
            }
            return data;
        }

        /// <summary>更新服務項目類別&名稱&狀態</summary>
        /// <param name="serviceName"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int updateServiceNameAndStatus(int uid, int service_Item, String serviceName, Boolean status)
        {
            int result = 0;
            String sql1 = @"UPDATE `u101b117_service` 
		                            SET service_item_uid = @service_item_uid,
                                        name = @name,
                                        isdelete = 0,
                                        edit_time=@edit_time
                            WHERE uid=@uid;";
            String sql2 = @"UPDATE `u101b117_service` 
		                            SET service_item_uid = '1',
                                        name = @name,
                                        isdelete = 1,
                                        edit_time=@edit_time
                            WHERE uid=@uid;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    if (status)
                    {
                        mysqlCmd.CommandText = sql2;
                    }
                    else
                    {
                        mysqlCmd.CommandText = sql1;
                        mysqlCmd.Parameters.AddWithValue("@service_item_uid", service_Item);
                    }
                    mysqlCmd.Parameters.AddWithValue("@name", serviceName);
                    mysqlCmd.Parameters.AddWithValue("@uid", uid);
                    mysqlCmd.Parameters.AddWithValue("@edit_time", DateTime.Now);
                    result = mysqlCmd.ExecuteNonQuery();
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    result = -1;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    result = -1;
                }
                finally
                {
                    mysqlTransaction.Dispose();
                    mysqlCmd.Dispose();
                    mysqlCon.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = -1;
            }
            return result;
        }


        #endregion

        #region Admin_Equipment

        /// <summary>搜尋後台表格[裝置管理]資料</summary>
        /// <returns></returns>
        public List<GridEquipmentModel> selectGridEquipmentInitData()
        {
            var data = new List<GridEquipmentModel>();
            String sql = @"SELECT *,
		                            (CASE WHEN a.isdelete='0' THEN '啟用' 
				                            WHEN a.isdelete='1' THEN '停用'
				                            END) AS status,
		                            (CASE WHEN a.type='0' THEN 'Beacon' 
				                            WHEN a.type='1' THEN 'Mobile'
				                            WHEN a.type='2' THEN 'NFC'
				                            WHEN a.type='3' THEN 'QRCode'
				                            END) AS typeName,
		                            (SELECT displayname
			                            FROM `u101b117_info` AS z
			                            WHERE a.account_uid = z.account_uid) AS displayname,
		                            (SELECT phone1
			                            FROM `u101b117_info` AS z
			                            WHERE a.account_uid = z.account_uid) AS phone
                            FROM `u101b117_equipment` AS a;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                try
                {
                    mysqlCmd.CommandText = sql;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(new GridEquipmentModel
                        {
                            EquipmentUID = int.Parse(mysqlDataReader["uid"].ToString()),
                            account_uid = int.Parse(mysqlDataReader["account_uid"].ToString()),
                            MACAddress = mysqlDataReader["macaddress"].ToString(),
                            type = int.Parse(mysqlDataReader["type"].ToString()),
                            typeName = mysqlDataReader["typeName"].ToString(),
                            summary = mysqlDataReader["summary"].ToString(),
                            edit_time = DateTime.Parse(mysqlDataReader["edit_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                            isdelete = int.Parse(mysqlDataReader["isdelete"].ToString()),
                            status = mysqlDataReader["status"].ToString(),
                            displayname = mysqlDataReader["displayname"].ToString(),
                            phone = mysqlDataReader["phone"].ToString()
                        });
                    }
                    mysqlDataReader.Dispose();
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    data = null;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    data = null;
                }
                finally
                {
                    mysqlTransaction.Dispose();
                    mysqlCmd.Dispose();
                    mysqlCon.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                data = null;
            }
            return data;
        }

        /// <summary>搜尋[使用者]資料</summary>
        /// <returns></returns>
        public List<FormEquipment_User> selectFormEquipmentUserData()
        {
            var data = new List<FormEquipment_User>();
            String sql = @"SELECT a.account_uid,a.displayname,
		                            a.phone1,b.level,
		                            (CASE WHEN b.level = 3
				                            THEN '照服員'
				                            WHEN b.level = 4
				                            THEN '案主'
				                            END) AS levelName
                            FROM `u101b117_info` AS a
                            JOIN `u101b117_account` AS b
                            ON a.account_uid = b.uid
                            WHERE b.level = 3 OR b.level = 4;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                try
                {
                    mysqlCmd.CommandText = sql;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(new FormEquipment_User
                        {
                            account_uid = int.Parse(mysqlDataReader["account_uid"].ToString()),
                            showName = mysqlDataReader["displayname"].ToString() + "（" + mysqlDataReader["phone1"].ToString() + "）",
                            displayname = mysqlDataReader["displayname"].ToString(),
                            phone = mysqlDataReader["phone1"].ToString(),
                            level = int.Parse(mysqlDataReader["level"].ToString()),
                            levelName = mysqlDataReader["levelName"].ToString()
                        });
                    }
                    mysqlDataReader.Dispose();
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    data = null;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    data = null;
                }
                finally
                {
                    mysqlTransaction.Dispose();
                    mysqlCmd.Dispose();
                    mysqlCon.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                data = null;
            }
            return data;
        }

        /// <summary>新增裝置</summary>
        /// <param name="account_uid"></param>
        /// <param name="mac"></param>
        /// <param name="type"></param>
        /// <param name="summary"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int insertNewEquipment(int account_uid, String mac, int type, String summary, Boolean status)
        {
            int result = 0;
            String sql = @"INSERT `u101b117_equipment`
		                            (account_uid, macaddress, type,
			                            summary, edit_time, isdelete)
		                            VALUES(@account_uid, @macaddress, @type,
				                            @summary, @edit_time, @isdelete);";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql;
                    mysqlCmd.Parameters.AddWithValue("@account_uid", account_uid);
                    mysqlCmd.Parameters.AddWithValue("@macaddress", mac);
                    mysqlCmd.Parameters.AddWithValue("@type", type);
                    mysqlCmd.Parameters.AddWithValue("@summary", summary);
                    mysqlCmd.Parameters.AddWithValue("@edit_time", DateTime.Now);
                    if (status) { mysqlCmd.Parameters.AddWithValue("@isdelete", 0); }
                    else { mysqlCmd.Parameters.AddWithValue("@isdelete", 1); }
                    result = mysqlCmd.ExecuteNonQuery();
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    result = -1;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    result = -1;
                }
                finally
                {
                    mysqlTransaction.Dispose();
                    mysqlCmd.Dispose();
                    mysqlCon.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = -1;
            }
            return result;
        }

        /// <summary>更新裝置</summary>
        /// <param name="account_uid"></param>
        /// <param name="mac"></param>
        /// <param name="type"></param>
        /// <param name="summary"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int updateOldEquipment(int uid, int account_uid, String mac, int type, String summary, Boolean status)
        {
            int result = 0;
            String sql = @"UPDATE `u101b117_equipment`
		                            SET `account_uid` = @account_uid, `macaddress` = @macaddress,
                                        `type` = @type, `summary` = @summary, `edit_time` = @edit_time, 
                                        `isdelete` = @isdelete
                            WHERE uid = @uid;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql;
                    mysqlCmd.Parameters.AddWithValue("@uid", uid);
                    mysqlCmd.Parameters.AddWithValue("@account_uid", account_uid);
                    mysqlCmd.Parameters.AddWithValue("@macaddress", mac);
                    mysqlCmd.Parameters.AddWithValue("@type", type);
                    mysqlCmd.Parameters.AddWithValue("@summary", summary);
                    mysqlCmd.Parameters.AddWithValue("@edit_time", DateTime.Now);
                    if (status) { mysqlCmd.Parameters.AddWithValue("@isdelete", 0); }
                    else { mysqlCmd.Parameters.AddWithValue("@isdelete", 1); }
                    result = mysqlCmd.ExecuteNonQuery();
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    result = -1;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    result = -1;
                }
                finally
                {
                    mysqlTransaction.Dispose();
                    mysqlCmd.Dispose();
                    mysqlCon.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = -1;
            }
            return result;
        }

        /// <summary>刪除裝置</summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int deleteOldEquipment(int uid)
        {
            int result = 0;
            String sql = @"DELETE FROM `u101b117_equipment`
                            WHERE uid = @uid;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql;
                    mysqlCmd.Parameters.AddWithValue("@uid", uid);
                    result = mysqlCmd.ExecuteNonQuery();
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    result = -1;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    result = -1;
                }
                finally
                {
                    mysqlTransaction.Dispose();
                    mysqlCmd.Dispose();
                    mysqlCon.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = -1;
            }
            return result;
        }


        #endregion

        #region Admin_License

        /// <summary>搜尋表格證照初始化資料</summary>
        /// <returns></returns>
        public DataTable selectGridLicenseInitData()
        {
            dbhelper = new DBHelper();
            DataTable dt = null;
            try
            {
                dbhelper.BeginTransaction();
                var cmd = dbhelper.GetCommand(@"SELECT *
                                                FROM `u101b117_license`;");
                dt = dbhelper.GetDataTable(cmd);
                dbhelper.TransactionCommit();
            }
            catch (Exception ex)
            {
                dbhelper.TransactionRollback();
                Console.Write(ex.Message);
            }
            return dt;
        }

        /// <summary>更新證照資料</summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public int updateOldLicense(int uid, String name, String summary)
        {
            dbhelper = new DBHelper();
            int result = 0;
            try
            {
                dbhelper.BeginTransaction();
                var cmd = dbhelper.GetCommand(@"UPDATE `u101b117_license`
                                                SET name = @name, summary = @summary,
                                                    edit_time = @edit_time
                                                WHERE uid = @uid;");
                cmd.Parameters.AddWithValue("@uid", uid);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@summary", summary);
                cmd.Parameters.AddWithValue("@edit_time", DateTime.Now);
                result = dbhelper.ExecuteNonQuery(cmd);
                dbhelper.TransactionCommit();
            }
            catch (Exception ex)
            {
                dbhelper.TransactionRollback();
                Console.Write(ex.Message);
                result = -1;
            }
            return result;
        }

        /// <summary>新增證照資料</summary>
        /// <param name="name"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public int insertNewLicense(String name, String summary)
        {
            dbhelper = new DBHelper();
            int result = 0;
            try
            {
                dbhelper.BeginTransaction();
                var cmd = dbhelper.GetCommand(@"INSERT `u101b117_license`
                                                       (`name`, `summary`, `edit_time`)
                                                VALUES (@name, @summary, @edit_time);");
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@summary", summary);
                cmd.Parameters.AddWithValue("@edit_time", DateTime.Now);
                result = dbhelper.ExecuteNonQuery(cmd);
                dbhelper.TransactionCommit();
            }
            catch (Exception ex)
            {
                dbhelper.TransactionRollback();
                Console.Write(ex.Message);
                result = -1;
            }
            return result;
        }

        /// <summary>刪除證照資料</summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int deleteOldLicense(int uid)
        {
            dbhelper = new DBHelper();
            int result = 0;
            try
            {
                dbhelper.BeginTransaction();
                var cmd = dbhelper.GetCommand(@"DELETE FROM `u101b117_license`
                                                WHERE uid = @uid;");
                cmd.Parameters.AddWithValue("@uid", uid);
                result = dbhelper.ExecuteNonQuery(cmd);
                dbhelper.TransactionCommit();
            }
            catch (Exception ex)
            {
                dbhelper.TransactionRollback();
                Console.Write(ex.Message);
                result = -1;
            }
            return result;
        }


        #endregion


    }
}