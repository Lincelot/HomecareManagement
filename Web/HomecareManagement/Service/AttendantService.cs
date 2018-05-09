using HomecareManagement.Models.Mobile;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace HomecareManagement.Service
{
    public class AttendantService
    {
        String conStr = ConStr.getConstr(0);
        DBHelper dbhelper;

        /// <summary>利用Mobile唯一碼搜尋裝置uid</summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public int selectEquipmentUIDFromMobile(String MAC)
        {
            int i = 0;
            dbhelper = new DBHelper();
            try
            {
                String sql = @"SELECT uid FROM `u101b117_equipment`
                               WHERE `macaddress` = @macaddress
                               AND type='1';";
                var cmd = dbhelper.GetCommand(sql);
                cmd.Parameters.AddWithValue("@macaddress", MAC);
                i = (int)dbhelper.ExecuteScalar(cmd);
            }
            catch (MySqlException)
            {
                i = -1;
                throw;
            }
            return i;
        }

        /// <summary>驗證使用者裝置MAC碼</summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public int selectMobileMAC(String MAC)
        {
            int i = 0;
            String sql = @"SELECT * FROM `u101b117_equipment`
                           WHERE `macaddress` = @macaddress
                           AND type='1';";
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
                    mysqlCmd.Parameters.AddWithValue("@macaddress", MAC);
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        i = int.Parse(mysqlDataReader["account_uid"].ToString());
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
                    i = -1;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    i = -1;
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
                i = -1;
            }
            return i;
        }

        /// <summary>搜尋Equipment內容</summary>
        /// <returns></returns>
        public List<Mobile_EquipmentModel> selectEquipment()
        {
            List<Mobile_EquipmentModel> data = new List<Mobile_EquipmentModel>();
            String sql = "SELECT * FROM u101b117_equipment;";
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
                        data.Add(new Mobile_EquipmentModel
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            account_uid = int.Parse(mysqlDataReader["account_uid"].ToString()),
                            macaddress = mysqlDataReader["macaddress"].ToString(),
                            type = int.Parse(mysqlDataReader["type"].ToString()),
                            edit_time = mysqlDataReader["edit_time"].ToString()
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

        /// <summary>搜尋Service內容</summary>
        /// <returns></returns>
        public List<Mobile_ServiceModel> selectService()
        {
            List<Mobile_ServiceModel> data = new List<Mobile_ServiceModel>();
            String sql = "SELECT * FROM u101b117_service;";
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
                        data.Add(new Mobile_ServiceModel
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            service_item_uid = (int)mysqlDataReader["service_item_uid"],
                            name = mysqlDataReader["name"].ToString(),
                            edit_time = DateTime.Parse(mysqlDataReader["edit_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                            isdelete = int.Parse(mysqlDataReader["isdelete"].ToString())
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

        /// <summary>搜尋Schedule內容</summary>
        /// <param name="uid">照服員編號</param>
        /// <returns></returns>
        public List<Mobile_ScheduleModel> selectMobile_Schedule(int uid)
        {
            List<Mobile_ScheduleModel> data = new List<Mobile_ScheduleModel>();
            String sql = "SELECT * FROM u101b117_schedule WHERE account_uid_1=@account_uid_1;";
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
                    mysqlCmd.Parameters.AddWithValue("@account_uid_1", uid);
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(new Mobile_ScheduleModel
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            account_uid_1 = int.Parse(mysqlDataReader["account_uid_1"].ToString()),
                            account_uid_2 = int.Parse(mysqlDataReader["account_uid_2"].ToString()),
                            start = DateTime.Parse(mysqlDataReader["start"].ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                            end = DateTime.Parse(mysqlDataReader["end"].ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                            edit_time = DateTime.Parse(mysqlDataReader["edit_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                            summary = mysqlDataReader["summary"].ToString()
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

        /// <summary>搜尋Schedule_Service內容</summary>
        /// <param name="uid">照服員編號</param>
        /// <returns></returns>
        public List<Mobile_Schedule_ServiceModel> selectSchedule_Service(int uid)
        {
            List<Mobile_Schedule_ServiceModel> data = new List<Mobile_Schedule_ServiceModel>();
            String sql = "SELECT a.uid,a.schedule_uid,a.service_uid"
                      + " FROM u101b117_schedule_service AS a"
                      + " JOIN u101b117_schedule AS b"
                      + " ON a.schedule_uid=b.uid"
                      + " WHERE b.account_uid_1=@account_uid_1;";
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
                    mysqlCmd.Parameters.AddWithValue("@account_uid_1", uid);
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(new Mobile_Schedule_ServiceModel
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            schedule_uid = int.Parse(mysqlDataReader["schedule_uid"].ToString()),
                            service_uid = int.Parse(mysqlDataReader["service_uid"].ToString())
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

        /// <summary>搜尋案主的所有資訊</summary>
        /// <param name="uid">照服員編號</param>
        /// <returns></returns>
        public List<Mobile_EmployerModel> selectAllEmployerInfo(int uid)
        {
            List<Mobile_EmployerModel> data = new List<Mobile_EmployerModel>();
            String sql = @"SELECT a.uid,a.account_uid_1,
		                            f.birthday,f.sex,f.address,
		                            f.displayname AS employer_name,
		                            f.phone1 AS employer_phone1,
		                            f.phone2 AS employer_phone2,
		                            b.name AS employer_item1,
		                            c.name AS employer_item2,
		                            d.name AS employer_item3,
		                            a.emg1_displayname AS emg1_displayname,
		                            a.emg1_phone1 AS emg1_phone1,
		                            a.emg1_phone2 AS emg1_phone2,
		                            a.emg2_displayname AS emg2_displayname,
		                            a.emg2_phone1 AS emg2_phone1,
		                            a.emg2_phone2 AS emg2_phone2,
		                            a.summary AS summary
                            FROM u101b117_info_employer AS a
                            JOIN u101b117_info_employer_item1 AS b
                            ON a.info_employer_item1_uid=b.uid
                            JOIN u101b117_info_employer_item2 AS c
                            ON a.info_employer_item2_uid=c.uid
                            JOIN u101b117_info_employer_item3 AS d
                            ON a.info_employer_item3_uid=d.uid
                            JOIN u101b117_schedule AS e
                            ON a.account_uid_1=e.account_uid_2
                            JOIN u101b117_info AS f
                            ON a.account_uid_1=f.account_uid
                            WHERE e.account_uid_1=@account_uid_1
                            GROUP BY a.account_uid_1;";
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
                    mysqlCmd.Parameters.AddWithValue("@account_uid_1", uid);
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(new Mobile_EmployerModel
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            account_uid_1 = int.Parse(mysqlDataReader["account_uid_1"].ToString()),
                            birthday = mysqlDataReader["birthday"].ToString(),
                            sex = int.Parse(mysqlDataReader["sex"].ToString()),
                            address = mysqlDataReader["address"].ToString(),
                            employer_name = mysqlDataReader["employer_name"].ToString(),
                            employer_phone1 = mysqlDataReader["employer_phone1"].ToString(),
                            employer_phone2 = mysqlDataReader["employer_phone2"].ToString(),
                            employer_item1 = mysqlDataReader["employer_item1"].ToString(),
                            employer_item2 = mysqlDataReader["employer_item2"].ToString(),
                            employer_item3 = mysqlDataReader["employer_item3"].ToString(),
                            emg1_displayname = mysqlDataReader["emg1_displayname"].ToString(),
                            emg1_phone1 = mysqlDataReader["emg1_phone1"].ToString(),
                            emg1_phone2 = mysqlDataReader["emg1_phone2"].ToString(),
                            emg2_displayname = mysqlDataReader["emg2_displayname"].ToString(),
                            emg2_phone1 = mysqlDataReader["emg2_phone1"].ToString(),
                            emg2_phone2 = mysqlDataReader["emg2_phone2"].ToString(),
                            summary = mysqlDataReader["summary"].ToString()
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

        /// <summary>搜尋個案服務紀錄表類別</summary>
        /// <returns></returns>
        public DataTable selectCaseRecordItem()
        {
            var dt = new DataTable();
            dbhelper = new DBHelper();
            try
            {
                dbhelper.BeginTransaction();
                String sql = @" SELECT *
                                FROM `u101b117_case_serivce_record_item`;";
                dt = dbhelper.GetDataTable(sql);
                dbhelper.TransactionCommit();
            }
            catch (MySqlException)
            {
                dbhelper.TransactionRollback();
                throw;
            }
            return dt;
        }

        /// <summary>搜尋個案服務紀錄表答案</summary>
        /// <returns></returns>
        public DataTable selectCaseRecordAnswer()
        {
            var dt = new DataTable();
            dbhelper = new DBHelper();
            try
            {
                dbhelper.BeginTransaction();
                String sql = @" SELECT *
                                FROM `u101b117_case_serivce_record_answer`;";
                dt = dbhelper.GetDataTable(sql);
                dbhelper.TransactionCommit();
            }
            catch (MySqlException)
            {
                dbhelper.TransactionRollback();
                throw;
            }
            return dt;
        }

        /// <summary>填入照服員簽到退紀錄</summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<int> insertWork_Record(List<Mobile_Work_RecordModel> list, int uid)
        {
            var result = new List<int>();
            dbhelper = new DBHelper();
            try
            {
                dbhelper.BeginTransaction();

                String sql1 = @"SELECT IFNULL(( SELECT uid
				                                FROM `u101b117_schedule` AS a
				                                WHERE @start BETWEEN DATE_ADD(a.start,INTERVAL -15 MINUTE) AND DATE_ADD(a.end,INTERVAL 15 MINUTE)
				                                AND account_uid_1 = (SELECT account_uid
									                                 FROM `u101b117_equipment`AS z
									                                 WHERE z.uid = @equipment_uid_1
                                                                     AND z.type = '1')
				                                AND account_uid_2 = (SELECT account_uid
									                                 FROM `u101b117_equipment`AS z
									                                 WHERE z.uid = @equipment_uid_2
                                                                     AND z.type != '1')) ,0);";
                String sql2 = @"INSERT INTO `u101b117_work_record` ( schedule_uid, start, end,
                                                                     equipment_uid_1, equipment_uid_2, update_time)
                                SELECT @schedule_uid,
                                       @start, @end, @equipment_uid_1, @equipment_uid_2, CONVERT_TZ(UTC_TIMESTAMP(),'+00:00','+08:00')
                                FROM DUAL
                                WHERE NOT EXISTS ( SELECT * 
				                                   FROM `u101b117_work_record` 
				                                   WHERE schedule_uid = @schedule_uid
				                                   AND start = @start
				                                   AND end = @end
				                                   AND equipment_uid_1 = @equipment_uid_1
				                                   AND equipment_uid_2 = @equipment_uid_2 );";

                foreach (var item in list)
                {
                    long schedule_uid = 0;
                    var cmd = dbhelper.GetCommand(sql1);
                    cmd.Parameters.AddWithValue("@start", item.start);
                    //照服員編號
                    cmd.Parameters.AddWithValue("@equipment_uid_1", uid);
                    //Beacon編號
                    cmd.Parameters.AddWithValue("@equipment_uid_2", item.equipment_uid_1);
                    schedule_uid = (long)dbhelper.ExecuteScalar(cmd);
                    if (schedule_uid > 0)
                    {
                        cmd = dbhelper.GetCommand(sql2);
                        cmd.Parameters.AddWithValue("@schedule_uid", schedule_uid);
                        cmd.Parameters.AddWithValue("@start", item.start);
                        cmd.Parameters.AddWithValue("@end", item.end);
                        //Beacon編號
                        cmd.Parameters.AddWithValue("@equipment_uid_1", item.equipment_uid_1);
                        //Mobile編號
                        cmd.Parameters.AddWithValue("@equipment_uid_2", uid);
                        dbhelper.ExecuteNonQuery(cmd);
                    }
                    result.Add(item.uid);
                    cmd.Dispose();
                }

                dbhelper.TransactionCommit();
            }
            catch (MySqlException)
            {
                dbhelper.TransactionRollback();
                result = null;
                throw;
            }
            return result;
        }

        /// <summary>填入照服員服務項目時數</summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<int> insertWork_Service(List<Mobile_Work_ServiceModel> list)
        {
            var result = new List<int>();
            dbhelper = new DBHelper();
            try
            {
                dbhelper.BeginTransaction();
                String sql = @" INSERT INTO `u101b117_work_service` ( schedule_uid, service_uid, minutes, summary, update_time)
                                SELECT @schedule_uid, @service_uid, @minutes, @summary, CONVERT_TZ(UTC_TIMESTAMP(),'+00:00','+08:00')
                                FROM DUAL
                                WHERE NOT EXISTS ( SELECT * 
				                                   FROM `u101b117_work_service` 
				                                   WHERE schedule_uid = @schedule_uid
                                                   AND service_uid = @service_uid
                                                   AND minutes = @minutes);";
                foreach (var item in list)
                {
                    var cmd = dbhelper.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@schedule_uid", item.schedule_uid);
                    cmd.Parameters.AddWithValue("@service_uid", item.service_uid);
                    cmd.Parameters.AddWithValue("@minutes", item.minutes);
                    cmd.Parameters.AddWithValue("@summary", item.summary);
                    dbhelper.ExecuteNonQuery(cmd);
                    result.Add(item.uid);
                    cmd.Dispose();
                }
                dbhelper.TransactionCommit();
            }
            catch (MySqlException)
            {
                dbhelper.TransactionRollback();
                result = null;
                throw;
            }
            return result;
        }

        /// <summary>填入照服員個案服務紀錄表資料</summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<int> insertWork_Case_Record(List<Mobile_Work_Case_Record> list)
        {
            var result = new List<int>();
            dbhelper = new DBHelper();
            try
            {
                dbhelper.BeginTransaction();
                String sql = @" INSERT INTO `u101b117_work_case_record` ( schedule_uid, case_record_answer_uid, summary, update_time)
                                SELECT @schedule_uid, @case_record_answer_uid, @summary, CONVERT_TZ(UTC_TIMESTAMP(),'+00:00','+08:00')
                                FROM DUAL
                                WHERE NOT EXISTS ( SELECT * 
				                                   FROM `u101b117_work_case_record` 
				                                   WHERE schedule_uid = @schedule_uid
                                                   AND case_record_answer_uid = @case_record_answer_uid );";
                foreach (var item in list)
                {
                    var cmd = dbhelper.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@schedule_uid", item.schedule_uid);
                    cmd.Parameters.AddWithValue("@case_record_answer_uid", item.case_record_answer_uid);
                    cmd.Parameters.AddWithValue("@summary", item.summary);
                    dbhelper.ExecuteNonQuery(cmd);
                    result.Add(item.uid);
                    cmd.Dispose();
                }
                dbhelper.TransactionCommit();
            }
            catch (MySqlException)
            {
                dbhelper.TransactionRollback();
                result = null;
                throw;
            }
            return result;
        }
    }
}