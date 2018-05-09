using HomecareManagement.Models.Web;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace HomecareManagement.Service
{
    public class SupervisorService
    {
        DBHelper dbHelper;
        String conStr = ConStr.getConstr(0);

        #region Supervisor_Schedule

        /// <summary>取得督導姓名清單</summary>
        /// <returns></returns>
        public List<InfoModel> selectSupervisorName()
        {
            List<InfoModel> info = new List<InfoModel>();
            try
            {
                String sql = "SELECT a.uid,b.displayname,b.phone1"
                          + " FROM u101b117_account AS a"
                          + " JOIN u101b117_info AS b"
                          + " ON a.uid=b.account_uid"
                          + " WHERE a.level=2;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    info.Add(new InfoModel
                    {
                        account_uid = int.Parse(mysqlDataReader["uid"].ToString()),
                        displayname = mysqlDataReader["displayname"].ToString(),
                        phone1 = mysqlDataReader["phone1"].ToString()
                    });
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

        /// <summary>取得督導旗下照服員的行程表</summary>
        /// <param name="account_uid">督導編號</param>
        /// <returns></returns>
        public List<ScheduleModel> selectSchedule(int account_uid)
        {
            List<ScheduleModel> schedule = new List<ScheduleModel>();
            try
            {
                String sql = @"SELECT a.uid,a.start,a.end,a.pay,a.account_uid_3,
		                                a.account_uid_2 AS EmployerID,
		                                a.account_uid_1 AS AttendantID,
		                                a.summary AS summary,
		                                (SELECT displayname
		                                FROM u101b117_info AS d
		                                WHERE a.account_uid_1=d.account_uid
		                                ) AS Attendant,
		                                (SELECT displayname
		                                FROM u101b117_info AS d
		                                WHERE a.account_uid_2=d.account_uid
		                                ) AS Employer,
		                                (SELECT displayname
		                                FROM u101b117_info AS d
		                                WHERE a.account_uid_3=d.account_uid
		                                ) AS LastEditer,
		                                a.edit_time,
		                                (SELECT GROUP_CONCAT(service_uid SEPARATOR ',')
		                                FROM u101b117_schedule_service
		                                WHERE schedule_uid=a.uid) AS serviceItem
                                FROM u101b117_schedule AS a
                                JOIN u101b117_info_attendant AS b
                                ON a.account_uid_1=b.account_uid_1
                                WHERE b.account_uid_2=@account_uid
                                ORDER BY Attendant;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                int ownerId = 0;
                var owner = new List<int>();
                owner.Add(0);
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@account_uid", account_uid);
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    ownerId = int.Parse(mysqlDataReader["AttendantID"].ToString());
                    if (ownerId != owner[owner.Count - 1])
                    {
                        owner.Add(ownerId);
                    }
                    String[] s = mysqlDataReader["serviceItem"].ToString().Split(',');
                    List<int> ls = new List<int>();
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] == "")
                        {
                            s[i] = "0";
                        }
                        int j = int.Parse(s[i]);
                        ls.Add(j);
                    }
                    schedule.Add(new ScheduleModel
                    {
                        colorId = (owner.Count - 1),
                        taskId = int.Parse(mysqlDataReader["uid"].ToString()),
                        AttendantID = int.Parse(mysqlDataReader["AttendantID"].ToString()),
                        EmployerID = int.Parse(mysqlDataReader["EmployerID"].ToString()),
                        title = "(" + DateTime.Parse(mysqlDataReader["start"].ToString()).ToString("HH:mm") + ")" + mysqlDataReader["Attendant"].ToString(),
                        start = DateTime.Parse(mysqlDataReader["start"].ToString()).ToString("yyyy/MM/dd HH:mm"),
                        end = DateTime.Parse(mysqlDataReader["end"].ToString()).ToString("yyyy/MM/dd HH:mm"),
                        pay = int.Parse(mysqlDataReader["pay"].ToString()),
                        Attendant = mysqlDataReader["Attendant"].ToString(),
                        Employer = mysqlDataReader["Employer"].ToString(),
                        LastEditTime = DateTime.Parse(mysqlDataReader["edit_time"].ToString()).ToString("yyyy/MM/dd HH:mm"),
                        LastEditer = mysqlDataReader["LastEditer"].ToString(),
                        serviceItem = ls,
                        summary = mysqlDataReader["summary"].ToString()
                    });
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                schedule = null;
            }
            return schedule;
        }

        /// <summary>搜尋服務項目</summary>
        /// <returns></returns>
        public List<ServiceItemModel> selectServiceItme()
        {
            List<ServiceItemModel> serviceItem = new List<ServiceItemModel>();
            try
            {
                String sql = "SELECT a.uid AS service_uid,"
                          + " a.service_item_uid AS fk_service_item_uid,"
                          + " a.name AS service_name,"
                          + " b.uid AS service_item_uid,"
                          + " b.name AS service_item_name"
                          + " FROM u101b117_service AS a"
                          + " JOIN u101b117_service_item AS b"
                          + " ON a.service_item_uid=b.uid"
                          + " WHERE a.isdelete=0 AND b.isdelete=0;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    serviceItem.Add(new ServiceItemModel
                    {
                        service_uid = int.Parse(mysqlDataReader["service_uid"].ToString()),
                        fk_service_item_uid = int.Parse(mysqlDataReader["fk_service_item_uid"].ToString()),
                        service_name = mysqlDataReader["service_name"].ToString(),
                        service_item_uid = int.Parse(mysqlDataReader["service_item_uid"].ToString()),
                        service_item_name = mysqlDataReader["service_item_name"].ToString()
                    });
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                serviceItem = null;
            }
            return serviceItem;
        }

        /// <summary>搜尋督導旗下照服員資料</summary>
        /// <param name="account_uid">督導編號</param>
        /// <returns></returns>
        public List<InfoModel> selectAttendantList(int account_uid)
        {
            List<InfoModel> info = new List<InfoModel>();
            try
            {
                String sql = @" SELECT a.account_uid,a.displayname,a.phone1
                                FROM u101b117_info AS a
                                JOIN u101b117_info_attendant AS b
                                ON a.account_uid=b.account_uid_1
                                WHERE b.account_uid_2=@account_uid;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@account_uid", account_uid);
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    info.Add(new InfoModel
                    {
                        account_uid = int.Parse(mysqlDataReader["account_uid"].ToString()),
                        displayname = mysqlDataReader["displayname"].ToString(),
                        phone1 = mysqlDataReader["phone1"].ToString()
                    });
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

        /// <summary>搜尋案主資料</summary>
        /// <returns></returns>
        public List<InfoModel> selectEmployerList(int account_uid)
        {
            List<InfoModel> info = new List<InfoModel>();
            try
            {
                String sql = "SELECT a.account_uid,a.displayname,a.phone1"
                          + " FROM u101b117_info AS a"
                          + " JOIN u101b117_info_employer AS b"
                          + " ON a.account_uid=b.account_uid_1"
                          + " WHERE b.account_uid_2=@account_uid;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@account_uid", account_uid);
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    info.Add(new InfoModel
                    {
                        account_uid = int.Parse(mysqlDataReader["account_uid"].ToString()),
                        displayname = mysqlDataReader["displayname"].ToString(),
                        phone1 = mysqlDataReader["phone1"].ToString()
                    });
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

        /// <summary>搜尋指定照服員工作時段是否有重複</summary>
        /// 1. 開始時間在其他時段之間
        /// (start BETWEEN '2015-10-07 00:00' AND '2015-10-07 09:30')
        /// 2. 結束時間在其他時段之間
        /// (end BETWEEN '2015-10-07 09:30' AND '2015-10-07 12:30')
        /// 3. 開始與結束時間在其他時段之間
        /// ('2015-10-07 09:30' BETWEEN start AND end) OR ('2015-10-07 09:40' BETWEEN start AND end)
        /// 4. 開始與結束時間包含了其他時段(同上)
        /// 5. 完全重疊(同上)
        /// 6. 無問題
        /// <param name="AttendantID">照服員編號</param>
        /// <param name="EmployerID">案主編號(未採用)</param>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <returns></returns>
        public int selectRepeatDate(int AttendantID, DateTime start, DateTime end, Boolean editMode, int taskID)
        {
            int i = 0;
            try
            {

                String sql = "SELECT *"
                          + " FROM u101b117_schedule"
                          + " WHERE ((@start BETWEEN start AND end)"
                          + " OR (@end BETWEEN start AND end)"
                          + " OR (start BETWEEN @start AND @end)"
                          + " OR (end BETWEEN @start AND @end))"
                          + " AND (account_uid_1=@AttendantID);";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@start", start);
                mysqlCmd.Parameters.AddWithValue("@end", end);
                mysqlCmd.Parameters.AddWithValue("@AttendantID", AttendantID);
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    i++;
                    if (editMode && mysqlDataReader["uid"].ToString() == taskID.ToString())
                    {
                        i--;
                    }
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                i = -1;
            }
            return i;
        }

        /// <summary>搜尋當日已排定時間</summary>
        /// <param name="AttendantID"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public int selectWorkTime_Day(int AttendantID, DateTime start)
        {
            int i = 0;
            try
            {
                String sql = @"SELECT SUM(TIMESTAMPDIFF(MINUTE,start,end)) AS Worktime_Day
                                FROM u101b117_schedule
                                WHERE YEAR(start) = YEAR(@start)
                                AND MONTH(start) = MONTH(@start)
                                AND DAY(start) = DAY(@start)
                                AND account_uid_1=@AttendantID
                                GROUP BY account_uid_1";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@start", start);
                mysqlCmd.Parameters.AddWithValue("@AttendantID", AttendantID);
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    i = int.Parse(mysqlDataReader["Worktime_Day"].ToString());
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                i = -1;
            }
            return i;
        }

        /// <summary>搜尋指定照服員當月已排定時數&薪資&到職日</summary>
        /// <param name="AttendantID"></param>
        /// <returns></returns>
        public Dictionary<String, Object> selectAttendanWorkInfo(int AttendantID)
        {
            var data = new Dictionary<String, Object>();
            data["worktime"] = 0;
            data["pay"] = null;
            data["firstday"] = null;
            try
            {
                String sql1 = "SELECT a.account_uid_1 AS AttendantID,b.pay,b.firstday,"
                          + " SUM(TIMESTAMPDIFF(MINUTE,start,end)) AS worktime"
                          + " FROM u101b117_schedule AS a"
                          + " JOIN u101b117_info_attendant AS b"
                          + " ON a.account_uid_1=b.account_uid_1"
                          + " WHERE MONTH(start) = MONTH(NOW())"
                          + " AND a.account_uid_1=@AttendantID"
                          + " GROUP BY a.account_uid_1;";
                String sql2 = "SELECT account_uid_1 AS AttendantID,pay,firstday"
                           + " FROM u101b117_info_attendant"
                           + " WHERE account_uid_1=@AttendantID;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql1;
                mysqlCmd.Parameters.AddWithValue("@AttendantID", AttendantID);
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    data["worktime"] = mysqlDataReader["worktime"].ToString();
                    data["pay"] = mysqlDataReader["pay"].ToString();
                    data["firstday"] = DateTime.Parse(mysqlDataReader["firstday"].ToString()).ToString("yyyy/MM/dd");
                }
                if (data["pay"] == null && data["firstday"] == null)
                {
                    mysqlDataReader.Dispose();
                    mysqlCmd.CommandText = sql2;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data["pay"] = mysqlDataReader["pay"].ToString();
                        data["firstday"] = DateTime.Parse(mysqlDataReader["firstday"].ToString()).ToString("yyyy/MM/dd");
                    }
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                data["worktime"] = 0;
                data["pay"] = null;
                data["firstday"] = null;
            }
            return data;
        }

        /// <summary>搜尋案主當月已排定時數</summary>
        /// <param name="EmployerID"></param>
        /// <returns></returns>
        public List<double> selectEmployerTime(int EmployerID)
        {
            var data = new List<double>();
            String sql1 = @"SELECT account_uid_2 AS EmployerID,
		                            SUM(TIMESTAMPDIFF(MINUTE,start,end)) AS minutes
                            FROM `u101b117_schedule` AS a
                            WHERE MONTH(start) = MONTH(NOW())
                            AND a.account_uid_2=@EmployerID
                            GROUP BY account_uid_2;";
            String sql2 = @"SELECT a.account_uid_1,a.minutes1,a.minutes2
                            FROM `u101b117_info_employer` AS a
                            WHERE a.account_uid_1=@EmployerID;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                try
                {
                    mysqlCmd.CommandText = sql1;
                    mysqlCmd.Parameters.AddWithValue("@EmployerID", EmployerID);
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(double.Parse(mysqlDataReader["minutes"].ToString()));
                    }
                    if (data.Count == 0)
                    {
                        data.Add(0);
                    }
                    mysqlDataReader.Dispose();
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql2;
                    mysqlCmd.Parameters.AddWithValue("@EmployerID", EmployerID);
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        data.Add(double.Parse(mysqlDataReader["minutes1"].ToString()));
                        data.Add(double.Parse(mysqlDataReader["minutes2"].ToString()));
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

        /// <summary>新增班表</summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public int insertScheduleData(ScheduleModel schedule)
        {
            int result = 0;
            try
            {
                String sql1 = @"INSERT INTO u101b117_schedule (`account_uid_1`, `account_uid_2`,
                                                               `start`, `end`, `account_uid_3`,
                                                               `edit_time`, `pay`, `summary`)
                                                       VALUES (@account_uid_1, @account_uid_2,
                                                               @start, @end, @account_uid_3,
                                                               @edit_time, @pay, @summary );";
                String sql2 = "INSERT INTO u101b117_schedule_service"
                           + " (`schedule_uid`, `service_uid`)"
                           + " VALUES ";
                String sql3 = "UPDATE u101b117_info_attendant SET pay=@pay"
                           + " WHERE account_uid_1=@account_uid_1"
                           + " AND pay!=@pay";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                mysqlCon.Open();
                mysqlCmd.CommandText = sql1;
                mysqlCmd.Parameters.AddWithValue("@account_uid_1", schedule.AttendantID);
                mysqlCmd.Parameters.AddWithValue("@account_uid_2", schedule.EmployerID);
                mysqlCmd.Parameters.AddWithValue("@start", schedule.start);
                mysqlCmd.Parameters.AddWithValue("@end", schedule.end);
                mysqlCmd.Parameters.AddWithValue("@account_uid_3", schedule.LastEditer);
                mysqlCmd.Parameters.AddWithValue("@edit_time", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                mysqlCmd.Parameters.AddWithValue("@pay", schedule.pay);
                mysqlCmd.Parameters.AddWithValue("@summary", schedule.summary);
                result = mysqlCmd.ExecuteNonQuery();
                if (result > 0)
                {
                    int uid = int.Parse(mysqlCmd.LastInsertedId.ToString());
                    mysqlCmd.Parameters.Clear();
                    for (int i = 0; i < schedule.serviceItem.Count; i++)
                    {
                        sql2 += "(@schedule_uid" + i + " , @service_uid" + i + ")";
                        if (i != schedule.serviceItem.Count - 1)
                        {
                            sql2 += ",";
                        }
                    }
                    mysqlCmd.CommandText = sql2;
                    for (int i = 0; i < schedule.serviceItem.Count; i++)
                    {
                        mysqlCmd.Parameters.AddWithValue("@schedule_uid" + i, uid);
                        mysqlCmd.Parameters.AddWithValue("@service_uid" + i, schedule.serviceItem[i]);
                    }
                    result += mysqlCmd.ExecuteNonQuery();
                }
                mysqlCmd.Parameters.Clear();
                mysqlCmd.CommandText = sql3;
                mysqlCmd.Parameters.AddWithValue("@pay", schedule.pay);
                mysqlCmd.Parameters.AddWithValue("@account_uid_1", schedule.AttendantID);
                result += mysqlCmd.ExecuteNonQuery();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }

        /// <summary>尋找該筆班次的服務項目</summary>
        /// <param name="taskId">schedule編號</param>
        /// <returns></returns>
        public List<int> selectServiceItem(int taskId)
        {
            var data = new List<int>();
            try
            {
                String sql = "SELECT *"
                          + " FROM u101b117_schedule_service"
                          + " WHERE schedule_uid = @schedule_uid;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@schedule_uid", taskId);
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    data.Add(int.Parse(mysqlDataReader["service_uid"].ToString()));
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                data = null;
            }
            return data;
        }

        /// <summary>修改班表</summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public int updateScheduleData(ScheduleModel schedule)
        {
            int result = 0;
            String sql1 = @"UPDATE `u101b117_schedule` SET `account_uid_1` = @account_uid_1, `account_uid_2` = @account_uid_2,
                                                               `start`= @start, `end` = @end, `account_uid_3` = @account_uid_3,
                                                               `edit_time` = @edit_time, `pay` = @pay, `summary` = @summary
                            WHERE `uid` = @uid";
            String sql2 = @"DELETE FROM `u101b117_schedule_service` WHERE `schedule_uid` = @schedule_uid";
            String sql3 = "INSERT INTO u101b117_schedule_service"
                       + " (`schedule_uid`, `service_uid`)"
                       + " VALUES ";
            String sql4 = "UPDATE u101b117_info_attendant SET pay=@pay"
                       + " WHERE account_uid_1=@account_uid_1"
                       + " AND pay!=@pay";

            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql1;
                    mysqlCmd.Parameters.AddWithValue("@account_uid_1", schedule.AttendantID);
                    mysqlCmd.Parameters.AddWithValue("@account_uid_2", schedule.EmployerID);
                    mysqlCmd.Parameters.AddWithValue("@start", schedule.start);
                    mysqlCmd.Parameters.AddWithValue("@end", schedule.end);
                    mysqlCmd.Parameters.AddWithValue("@account_uid_3", schedule.LastEditer);
                    mysqlCmd.Parameters.AddWithValue("@edit_time", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    mysqlCmd.Parameters.AddWithValue("@pay", schedule.pay);
                    mysqlCmd.Parameters.AddWithValue("@summary", schedule.summary);
                    mysqlCmd.Parameters.AddWithValue("@uid", schedule.taskId);
                    result = mysqlCmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        mysqlCmd.Parameters.Clear();
                        mysqlCmd.CommandText = sql2;
                        mysqlCmd.Parameters.AddWithValue("@schedule_uid", schedule.taskId);
                        result += mysqlCmd.ExecuteNonQuery();
                        int uid = schedule.taskId;
                        mysqlCmd.Parameters.Clear();
                        for (int i = 0; i < schedule.serviceItem.Count; i++)
                        {
                            sql3 += "(@schedule_uid" + i + " , @service_uid" + i + ")";
                            if (i != schedule.serviceItem.Count - 1)
                            {
                                sql3 += ",";
                            }
                        }
                        mysqlCmd.CommandText = sql3;
                        for (int i = 0; i < schedule.serviceItem.Count; i++)
                        {
                            mysqlCmd.Parameters.AddWithValue("@schedule_uid" + i, uid);
                            mysqlCmd.Parameters.AddWithValue("@service_uid" + i, schedule.serviceItem[i]);
                        }
                        result += mysqlCmd.ExecuteNonQuery();
                    }
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql4;
                    mysqlCmd.Parameters.AddWithValue("@pay", schedule.pay);
                    mysqlCmd.Parameters.AddWithValue("@account_uid_1", schedule.AttendantID);
                    result += mysqlCmd.ExecuteNonQuery();
                    Console.WriteLine("影響：" + mysqlCmd.ExecuteNonQuery());
                    Console.WriteLine("影響：" + mysqlCmd.ExecuteNonQuery());
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

        /// <summary>刪除班表</summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public int delectScheduleData(int taskId)
        {
            int result = 0;
            try
            {
                String sql = "DELETE FROM u101b117_schedule"
                          + " WHERE uid = @taskId";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@taskId", taskId);
                result = mysqlCmd.ExecuteNonQuery();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (MySqlException)
            {
                result = -1;
            }
            return result;
        }


        #endregion

        #region Supervisor_Member

        /// <summary>搜尋督導、照服員、案主資料</summary>
        /// <returns></returns>
        public Dictionary<String, Object> selectAllMemberInfo()
        {
            var data = new Dictionary<String, Object>();
            List<Info_AttendantModel> lsAttendant = new List<Info_AttendantModel>();
            List<Info_EmployerModel> lsEmployer = new List<Info_EmployerModel>();
            List<Info_SupervisorModel> lsSupervisor = new List<Info_SupervisorModel>();

            String sql1 = @"SELECT *,
		                            IFNULL((SELECT GROUP_CONCAT(license_uid SEPARATOR ',')
				                            FROM u101b117_info_license AS z
				                            WHERE a.account_uid=z.account_uid), '0') AS license,
		                            (SELECT z.username 
		                             FROM `u101b117_account` AS z 
		                             WHERE a.account_uid=z.uid) AS username,
		                            (SELECT z.displayname
		                             FROM `u101b117_info` AS z
		                             WHERE b.account_uid_2=z.account_uid) AS supervisorName
                            FROM `u101b117_info` AS a
                            JOIN `u101b117_info_attendant` AS b
                            ON a.account_uid=b.account_uid_1
                            WHERE (SELECT z.level 
	                               FROM`u101b117_account` AS z
	                               WHERE a.account_uid=z.uid)=3;";
            String sql2 = @"SELECT a.account_uid,a.displayname,a.birthday,
		                            a.sex,a.address,a.phone1,
		                            a.phone2,b.account_uid_2,
		                            b.info_employer_item1_uid,
		                            b.info_employer_item2_uid,
		                            b.info_employer_item3_uid,
		                            b.info_employer_sub_uid,
		                            b.emg1_displayname AS emg1_displayname,
		                            b.emg1_phone1 AS emg1_phone1,
		                            b.emg1_phone2 AS emg1_phone2,
		                            b.emg2_displayname AS emg2_displayname,
		                            b.emg2_phone1 AS emg2_phone1,
		                            b.emg2_phone2 AS emg2_phone2,
		                            b.minutes1,b.minutes2,b.summary,
		                            (SELECT username 
			                            FROM `u101b117_account` AS z 
			                            WHERE a.account_uid=z.uid) AS username,
		                            (SELECT z.displayname
		                             FROM `u101b117_info` AS z
		                             WHERE b.account_uid_2=z.account_uid) AS supervisorName
                            FROM `u101b117_info` AS a
                            JOIN `u101b117_info_employer` AS b
                            ON a.account_uid=b.account_uid_1
                            WHERE (SELECT z.level 
		                            FROM`u101b117_account` AS z
		                            WHERE a.account_uid=z.uid) = 4;";
            String sql3 = @"SELECT a.*,b.username,c.*,
                                    IFNULL((SELECT GROUP_CONCAT(license_uid SEPARATOR ',')
				                            FROM u101b117_info_license AS z
				                            WHERE a.account_uid=z.account_uid), '0') AS license
                            FROM `u101b117_info` AS a
                            JOIN `u101b117_account` AS b
                            ON a.account_uid=b.uid
                            JOIN `u101b117_info_supervisor` AS c
                            ON a.account_uid=c.account_uid
                            WHERE b.level = 2;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                try
                {
                    #region lsAttendant
                    mysqlCmd.CommandText = sql1;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        String[] s = mysqlDataReader["license"].ToString().Split(',');
                        List<int> ls = new List<int>();
                        for (int i = 0; i < s.Length; i++)
                        {
                            if (s[i] == "0")
                            {
                                s[i] = "0";
                            }
                            int j = int.Parse(s[i]);
                            ls.Add(j);
                        }
                        String str = mysqlDataReader["birthday"].ToString();
                        if (str != "")
                        {
                            str = DateTime.Parse(str).ToString("yyyy/MM/dd");
                        }
                        lsAttendant.Add(new Info_AttendantModel
                        {
                            username = mysqlDataReader["username"].ToString(),
                            account_uid = int.Parse(mysqlDataReader["account_uid"].ToString()),
                            displayname = mysqlDataReader["displayname"].ToString(),
                            birthday = str,
                            sex = int.Parse(mysqlDataReader["sex"].ToString()),
                            address = mysqlDataReader["address"].ToString(),
                            phone1 = mysqlDataReader["phone1"].ToString(),
                            phone2 = mysqlDataReader["phone2"].ToString(),
                            supervisorID = int.Parse(mysqlDataReader["account_uid_2"].ToString()),
                            supervisorName = mysqlDataReader["supervisorName"].ToString(),
                            pay = int.Parse(mysqlDataReader["pay"].ToString()),
                            firstday = DateTime.Parse(mysqlDataReader["firstday"].ToString()).ToString("yyyy/MM/dd"),
                            summary = mysqlDataReader["summary"].ToString(),
                            lsLicense = ls
                        });
                    }
                    mysqlDataReader.Dispose();

                    #endregion

                    #region lsEmployer

                    mysqlCmd.CommandText = sql2;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        String str = mysqlDataReader["birthday"].ToString();
                        if (str != "")
                        {
                            str = DateTime.Parse(str).ToString("yyyy/MM/dd");
                        }
                        String[] s = mysqlDataReader["info_employer_item3_uid"].ToString().Split(',');
                        List<int> ls = new List<int>();
                        String lsStr = "";
                        for (int i = 0; i < s.Length; i++)
                        {
                            if (s[i] == "")
                            {
                                s[i] = "0";
                                lsStr += ",";
                            }
                            else
                            {
                                switch (s[i])
                                {
                                    case "1":
                                        {
                                            lsStr += "身心障礙" + ",";
                                            break;
                                        }
                                    case "2":
                                        {
                                            lsStr += "原住民" + ",";
                                            break;
                                        }
                                    case "3":
                                        {
                                            lsStr += "老人" + ",";
                                            break;
                                        }
                                }
                            }
                            int j = int.Parse(s[i]);
                            ls.Add(j);
                        }
                        lsEmployer.Add(new Info_EmployerModel
                        {
                            username = mysqlDataReader["username"].ToString(),
                            account_uid = int.Parse(mysqlDataReader["account_uid"].ToString()),
                            displayname = mysqlDataReader["displayname"].ToString(),
                            birthday = str,
                            sex = int.Parse(mysqlDataReader["sex"].ToString()),
                            address = mysqlDataReader["address"].ToString(),
                            phone1 = mysqlDataReader["phone1"].ToString(),
                            phone2 = mysqlDataReader["phone2"].ToString(),
                            supervisorID = int.Parse(mysqlDataReader["account_uid_2"].ToString()),
                            supervisorName = mysqlDataReader["supervisorName"].ToString(),
                            info_employer_sub_uid = int.Parse(mysqlDataReader["info_employer_sub_uid"].ToString()),
                            info_employer_item1_uid = int.Parse(mysqlDataReader["info_employer_item1_uid"].ToString()),
                            info_employer_item2_uid = int.Parse(mysqlDataReader["info_employer_item2_uid"].ToString()),
                            info_employer_item3_uid = ls,
                            info_employer_item3_uid_str = lsStr.Substring(0, lsStr.Length - 1),
                            emg1_displayname = mysqlDataReader["emg1_displayname"].ToString(),
                            emg1_phone1 = mysqlDataReader["emg1_phone1"].ToString(),
                            emg1_phone2 = mysqlDataReader["emg1_phone2"].ToString(),
                            emg2_displayname = mysqlDataReader["emg2_displayname"].ToString(),
                            emg2_phone1 = mysqlDataReader["emg2_phone1"].ToString(),
                            emg2_phone2 = mysqlDataReader["emg2_phone2"].ToString(),
                            minutes1 = int.Parse(mysqlDataReader["minutes1"].ToString()),
                            minutes2 = int.Parse(mysqlDataReader["minutes2"].ToString()),
                            summary = mysqlDataReader["summary"].ToString()
                        });
                    }
                    mysqlDataReader.Dispose();

                    #endregion

                    #region lsSupervisor

                    mysqlCmd.CommandText = sql3;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        String[] s = mysqlDataReader["license"].ToString().Split(',');
                        String strBirthday = mysqlDataReader["birthday"].ToString();
                        String strFirstday = mysqlDataReader["firstday"].ToString();
                        List<int> lsL = new List<int>();
                        List<int> lsT = new List<int>();
                        int eduBG = 0;
                        for (int i = 0; i < s.Length; i++)
                        {
                            if (s[i] == "0")
                            {
                                s[i] = "0";
                            }
                            int j = int.Parse(s[i]);
                            lsL.Add(j);
                        }
                        if (mysqlDataReader["train"].ToString() != "")
                        {
                            s = mysqlDataReader["train"].ToString().Split(',');
                            for (int i = 0; i < s.Length; i++)
                            {
                                if (s[i] == "0")
                                {
                                    s[i] = "0";
                                }
                                int j = int.Parse(s[i]);
                                lsT.Add(j);
                            }
                        }
                        if (strBirthday != "")
                        {
                            strBirthday = DateTime.Parse(strBirthday).ToString("yyyy/MM/dd");
                        }
                        if (strFirstday != "")
                        {
                            strFirstday = DateTime.Parse(strFirstday).ToString("yyyy/MM/dd");
                        }
                        if (mysqlDataReader["edubg"].ToString() != "")
                        {
                            eduBG = int.Parse(mysqlDataReader["edubg"].ToString());
                        }

                        lsSupervisor.Add(new Info_SupervisorModel
                        {
                            username = mysqlDataReader["username"].ToString(),
                            account_uid = int.Parse(mysqlDataReader["account_uid"].ToString()),
                            displayname = mysqlDataReader["displayname"].ToString(),
                            birthday = strBirthday,
                            sex = int.Parse(mysqlDataReader["sex"].ToString()),
                            address = mysqlDataReader["address"].ToString(),
                            phone1 = mysqlDataReader["phone1"].ToString(),
                            phone2 = mysqlDataReader["phone2"].ToString(),
                            lsLicense = lsL,
                            firstday = strFirstday,
                            lsTrain = lsT,
                            proBG = mysqlDataReader["probg"].ToString(),
                            eduBG = eduBG
                        });
                    }
                    mysqlDataReader.Dispose();

                    #endregion

                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    lsAttendant = null;
                    lsEmployer = null;
                    lsSupervisor = null;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    lsAttendant = null;
                    lsEmployer = null;
                    lsSupervisor = null;
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
                lsAttendant = null;
                lsEmployer = null;
                lsSupervisor = null;
            }
            data.Add("lsAttendant", lsAttendant);
            data.Add("lsEmployer", lsEmployer);
            data.Add("lsSupervisor", lsSupervisor);
            return data;
        }

        /// <summary>搜尋照服員&案主的item</summary>
        /// <returns></returns>
        public Dictionary<String, Object> selectInfoItem()
        {
            var data = new Dictionary<String, Object>();
            List<Info_Item> lsInfo_employer_item1 = new List<Info_Item>();
            List<Info_Item> lsInfo_employer_item2 = new List<Info_Item>();
            List<Info_Item> lsInfo_employer_item3 = new List<Info_Item>();
            List<Info_Item> lsLicense = new List<Info_Item>();
            List<Info_Item> lsInfo_employer_sub = new List<Info_Item>();

            String sql1 = @"SELECT * FROM `u101b117_info_employer_item1`;";
            String sql2 = @"SELECT * FROM `u101b117_info_employer_item2`;";
            String sql3 = @"SELECT * FROM `u101b117_info_employer_item3`;";
            String sql4 = @"SELECT * FROM `u101b117_license`;";
            String sql5 = @"SELECT * FROM `u101b117_info_employer_sub`;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                try
                {
                    mysqlCmd.CommandText = sql1;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        lsInfo_employer_item1.Add(new Info_Item
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            name = mysqlDataReader["name"].ToString()
                        });
                    }
                    mysqlDataReader.Dispose();
                    mysqlCmd.CommandText = sql2;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        lsInfo_employer_item2.Add(new Info_Item
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            name = mysqlDataReader["name"].ToString()
                        });
                    }
                    mysqlDataReader.Dispose();
                    mysqlCmd.CommandText = sql3;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        lsInfo_employer_item3.Add(new Info_Item
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            name = mysqlDataReader["name"].ToString()
                        });
                    }
                    mysqlDataReader.Dispose();
                    mysqlCmd.CommandText = sql4;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        lsLicense.Add(new Info_Item
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            name = mysqlDataReader["name"].ToString(),
                            summary = mysqlDataReader["summary"].ToString()
                        });
                    }
                    mysqlDataReader.Dispose();
                    mysqlCmd.CommandText = sql5;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        lsInfo_employer_sub.Add(new Info_Item
                        {
                            uid = int.Parse(mysqlDataReader["uid"].ToString()),
                            name = mysqlDataReader["name"].ToString()
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
                    lsInfo_employer_item1 = null;
                    lsInfo_employer_item2 = null;
                    lsInfo_employer_item3 = null;
                    lsLicense = null;
                    lsInfo_employer_sub = null;
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                    lsInfo_employer_item1 = null;
                    lsInfo_employer_item2 = null;
                    lsInfo_employer_item3 = null;
                    lsLicense = null;
                    lsInfo_employer_sub = null;
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
                lsInfo_employer_item1 = null;
                lsInfo_employer_item2 = null;
                lsInfo_employer_item3 = null;
                lsLicense = null;
                lsInfo_employer_sub = null;
            }
            data.Add("lsInfo_employer_item1", lsInfo_employer_item1);
            data.Add("lsInfo_employer_item2", lsInfo_employer_item2);
            data.Add("lsInfo_employer_item3", lsInfo_employer_item3);
            data.Add("lsLicense", lsLicense);
            data.Add("lsSub", lsInfo_employer_sub);
            return data;
        }

        /// <summary>新增使用者-管理員</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int insertMember_Other(InfoModel info, String password, int level)
        {
            int result = 0;
            String sql1 = @"INSERT INTO `u101b117_account`
			                            (`username`, `password`, `level`)
			                            VALUES(@username,@password,@level);";
            String sql2 = @"INSERT INTO `u101b117_info`
                                        (`account_uid`, `displayname`,
				                            `birthday`, `sex`,
				                            `address`, `phone1`,
				                            `phone2`)
			                            VALUES(@account_uid,@displayname,
                                                @birthday,@sex,
                                                @address,@phone1,
                                                @phone2);";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql1;
                    mysqlCmd.Parameters.AddWithValue("@username", info.username);
                    mysqlCmd.Parameters.AddWithValue("@password", password);
                    mysqlCmd.Parameters.AddWithValue("@level", level);
                    result += mysqlCmd.ExecuteNonQuery();
                    info.account_uid = int.Parse(mysqlCmd.LastInsertedId.ToString());
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql2;
                    mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    mysqlCmd.Parameters.AddWithValue("@displayname", info.displayname);
                    mysqlCmd.Parameters.AddWithValue("@birthday", info.birthday);
                    mysqlCmd.Parameters.AddWithValue("@sex", info.sex);
                    mysqlCmd.Parameters.AddWithValue("@address", info.address);
                    mysqlCmd.Parameters.AddWithValue("@phone1", info.phone1);
                    mysqlCmd.Parameters.AddWithValue("@phone2", info.phone2);
                    result += mysqlCmd.ExecuteNonQuery();
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

        /// <summary>新增使用者-照服員</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int insertMember_Attendant(Info_AttendantModel info, String password, int level)
        {
            int result = 0;
            String sql1 = @"INSERT INTO `u101b117_account`
			                            (`username`, `password`, `level`)
			                            VALUES(@username,@password,@level);";
            String sql2 = @"INSERT INTO `u101b117_info`
                                        (`account_uid`, `displayname`,
				                            `birthday`, `sex`,
				                            `address`, `phone1`,
				                            `phone2`)
			                            VALUES(@account_uid,@displayname,
                                                @birthday,@sex,
                                                @address,@phone1,
                                                @phone2);";
            String sql3 = @"INSERT INTO `u101b117_info_attendant`
			                            (`account_uid_1`,`account_uid_2`,
                                            `pay`,`firstday`, `summary`)
			                            VALUES(@attendantID,@supervisorID,@pay,
					                            @firstday,@summary);";
            String sql4 = @"INSERT INTO `u101b117_info_license`
			                            (`account_uid`, `license_uid`)
			                            VALUES";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql1;
                    mysqlCmd.Parameters.AddWithValue("@username", info.username);
                    mysqlCmd.Parameters.AddWithValue("@password", password);
                    mysqlCmd.Parameters.AddWithValue("@level", level);
                    result += mysqlCmd.ExecuteNonQuery();
                    info.account_uid = int.Parse(mysqlCmd.LastInsertedId.ToString());
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql2;
                    mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    mysqlCmd.Parameters.AddWithValue("@displayname", info.displayname);
                    mysqlCmd.Parameters.AddWithValue("@birthday", info.birthday);
                    mysqlCmd.Parameters.AddWithValue("@sex", info.sex);
                    mysqlCmd.Parameters.AddWithValue("@address", info.address);
                    mysqlCmd.Parameters.AddWithValue("@phone1", info.phone1);
                    mysqlCmd.Parameters.AddWithValue("@phone2", info.phone2);
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql3;
                    mysqlCmd.Parameters.AddWithValue("@attendantID", info.account_uid);
                    mysqlCmd.Parameters.AddWithValue("@supervisorID", info.supervisorID);
                    mysqlCmd.Parameters.AddWithValue("@pay", info.pay);
                    mysqlCmd.Parameters.AddWithValue("@firstday", info.firstday);
                    mysqlCmd.Parameters.AddWithValue("@summary", info.summary);
                    result += mysqlCmd.ExecuteNonQuery();
                    if (info.lsLicense != null)
                    {
                        mysqlCmd.Parameters.Clear();
                        for (int i = 0; i < info.lsLicense.Count; i++)
                        {
                            sql4 += "(@attendantID" + i + ",@license_uid" + i + "),";
                        }
                        sql4 = sql4.Substring(0, sql4.Length - 1);
                        mysqlCmd.CommandText = sql4;
                        for (int i = 0; i < info.lsLicense.Count; i++)
                        {
                            mysqlCmd.Parameters.AddWithValue("@attendantID" + i, info.account_uid);
                            mysqlCmd.Parameters.AddWithValue("@license_uid" + i, info.lsLicense[i]);
                        }
                        result += mysqlCmd.ExecuteNonQuery();
                    }
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

        /// <summary>新增使用者-案主</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int insertMember_Employer(Info_EmployerModel info, String password, int level)
        {
            int result = 0;
            String sql1 = @"INSERT INTO `u101b117_account`
			                            (`username`, `password`, `level`)
			                            VALUES(@username,@password,@level);";
            String sql2 = @"INSERT INTO `u101b117_info`
                                        (`account_uid`, `displayname`,
				                            `birthday`, `sex`,
				                            `address`, `phone1`,
				                            `phone2`)
			                            VALUES (@account_uid,@displayname,
                                                @birthday,@sex,
                                                @address,@phone1,
                                                @phone2);";
            String sql3 = @"INSERT INTO `u101b117_info_employer`
			                            (`account_uid_1`, `account_uid_2`, `info_employer_sub_uid`,
				                            `info_employer_item1_uid`, `info_employer_item2_uid`,
				                            `info_employer_item3_uid`, `emg1_displayname`,
				                            `emg1_phone1`,`emg1_phone2`,`emg2_displayname`,
				                            `emg2_phone1`,`emg2_phone2`,
				                            `minutes1`,`minutes2`,
				                            `summary`)
			                            VALUES (@employerID,@supervisorID,
					                            @sub,@item1,@item2,@item3,
					                            @emg1_displayname,@emg1_phone1,
					                            @emg1_phone2,
                                                @emg2_displayname,@emg2_phone1,
		                                        @emg2_phone2,
                                                @minutes1,
					                            @minutes2,@summary);";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql1;
                    mysqlCmd.Parameters.AddWithValue("@username", info.username);
                    mysqlCmd.Parameters.AddWithValue("@password", password);
                    mysqlCmd.Parameters.AddWithValue("@level", level);
                    result += mysqlCmd.ExecuteNonQuery();
                    info.account_uid = int.Parse(mysqlCmd.LastInsertedId.ToString());
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql2;
                    mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    mysqlCmd.Parameters.AddWithValue("@displayname", info.displayname);
                    mysqlCmd.Parameters.AddWithValue("@birthday", info.birthday);
                    mysqlCmd.Parameters.AddWithValue("@sex", info.sex);
                    mysqlCmd.Parameters.AddWithValue("@address", info.address);
                    mysqlCmd.Parameters.AddWithValue("@phone1", info.phone1);
                    mysqlCmd.Parameters.AddWithValue("@phone2", info.phone2);
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql3;
                    mysqlCmd.Parameters.AddWithValue("@employerID", info.account_uid);
                    mysqlCmd.Parameters.AddWithValue("@supervisorID", info.supervisorID);
                    mysqlCmd.Parameters.AddWithValue("@sub", info.info_employer_sub_uid);
                    mysqlCmd.Parameters.AddWithValue("@item1", info.info_employer_item1_uid);
                    mysqlCmd.Parameters.AddWithValue("@item2", info.info_employer_item2_uid);
                    String str = "";
                    if (info.info_employer_item3_uid != null)
                    {
                        for (int i = 0; i < info.info_employer_item3_uid.Count; i++)
                        {
                            str += info.info_employer_item3_uid[i] + ",";
                        }
                    }
                    mysqlCmd.Parameters.AddWithValue("@item3", str.Substring(0, str.Length - 1));
                    mysqlCmd.Parameters.AddWithValue("@emg1_displayname", info.emg1_displayname);
                    mysqlCmd.Parameters.AddWithValue("@emg1_phone1", info.emg1_phone1);
                    mysqlCmd.Parameters.AddWithValue("@emg1_phone2", info.emg1_phone2);
                    mysqlCmd.Parameters.AddWithValue("@emg2_displayname", info.emg2_displayname);
                    mysqlCmd.Parameters.AddWithValue("@emg2_phone1", info.emg2_phone1);
                    mysqlCmd.Parameters.AddWithValue("@emg2_phone2", info.emg2_phone2);
                    mysqlCmd.Parameters.AddWithValue("@minutes1", info.minutes1);
                    mysqlCmd.Parameters.AddWithValue("@minutes2", info.minutes2);
                    mysqlCmd.Parameters.AddWithValue("@summary", info.summary);
                    result += mysqlCmd.ExecuteNonQuery();
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

        /// <summary>新增使用者-督導</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int insertMember_Supervisor(Info_SupervisorModel info, String password, int level)
        {
            dbHelper = new DBHelper();
            int result = 0;
            String sql1 = @"INSERT INTO `u101b117_account`
			                            (`username`, `password`, `level`)
			                            VALUES(@username, @password, @level);";
            String sql_LAST_INSERT_ID = @"SELECT LAST_INSERT_ID();";
            String sql2 = @"INSERT INTO `u101b117_info`
                                        (`account_uid`, `displayname`,
				                            `birthday`, `sex`,
				                            `address`, `phone1`,
				                            `phone2`)
			                            VALUES(@account_uid, @displayname,
                                                @birthday, @sex,
                                                @address, @phone1,
                                                @phone2);";
            String sql3 = @"INSERT INTO `u101b117_info_supervisor`
			                            (`account_uid`, `firstday`, `train`, `probg`, `edubg`)
			                            VALUES(@account_uid, @firstday, @train, @probg, @edubg);";
            String sql4 = @"INSERT INTO `u101b117_info_license`
			                            (`account_uid`, `license_uid`)
			                            VALUES";
            try
            {
                dbHelper.BeginTransaction();
                var cmd = dbHelper.GetCommand(sql1);
                //帳號
                cmd.Parameters.AddWithValue("@username", info.username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@level", level);
                result += dbHelper.ExecuteNonQuery(cmd);
                cmd = dbHelper.GetCommand(sql_LAST_INSERT_ID);
                int account_uid = int.Parse(dbHelper.ExecuteScalar(cmd).ToString());
                //資料
                cmd.Parameters.Clear();
                cmd = dbHelper.GetCommand(sql2);
                cmd.Parameters.AddWithValue("@account_uid", account_uid);
                cmd.Parameters.AddWithValue("@displayname", info.displayname);
                cmd.Parameters.AddWithValue("@birthday", info.birthday);
                cmd.Parameters.AddWithValue("@sex", info.sex);
                cmd.Parameters.AddWithValue("@address", info.address);
                cmd.Parameters.AddWithValue("@phone1", info.phone1);
                cmd.Parameters.AddWithValue("@phone2", info.phone2);
                result += dbHelper.ExecuteNonQuery(cmd);
                //督導限定
                cmd.Parameters.Clear();
                cmd = dbHelper.GetCommand(sql3);
                cmd.Parameters.AddWithValue("@firstday", info.firstday);
                String lsTrain = "";
                if (info.lsTrain != null)
                {
                    for (int i = 0; i < info.lsTrain.Count; i++)
                    {
                        lsTrain += info.lsTrain[i] + ",";
                    }
                    lsTrain = lsTrain.Substring(0, lsTrain.Length - 1);
                }
                cmd.Parameters.AddWithValue("@account_uid", account_uid);
                cmd.Parameters.AddWithValue("@train", lsTrain);
                cmd.Parameters.AddWithValue("@probg", info.proBG);
                cmd.Parameters.AddWithValue("@edubg", info.eduBG);
                result += dbHelper.ExecuteNonQuery(cmd);
                //督導證照
                if (info.lsLicense != null)
                {
                    for (int i = 0; i < info.lsLicense.Count; i++)
                    {
                        sql4 += "('" + account_uid + "', @license_uid" + i + "),";
                    }
                    sql4 = sql4.Substring(0, sql4.Length - 1);
                    cmd.Parameters.Clear();
                    cmd = dbHelper.GetCommand(sql4);
                    for (int i = 0; i < info.lsLicense.Count; i++)
                    {
                        cmd.Parameters.AddWithValue("@license_uid" + i, info.lsLicense[i]);
                    }
                    result += dbHelper.ExecuteNonQuery(cmd);
                }
                dbHelper.TransactionCommit();
            }
            catch (MySqlException ex)
            {
                dbHelper.TransactionRollback();
                Console.WriteLine(ex.Message);
                result = -1;
            }
            return result;
        }

        /// <summary>搜尋帳號是否有重複</summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public int selectUsername(String username)
        {
            int result = 0;
            try
            {
                String sql = "SELECT *"
                    + " FROM u101b117_account"
                    + " WHERE username=@username;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                MySqlDataReader mysqlDataReader;
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@username", username);
                mysqlDataReader = mysqlCmd.ExecuteReader();
                while (mysqlDataReader.Read())
                {
                    result++;
                }
                mysqlDataReader.Dispose();
                mysqlCmd.Dispose();
                mysqlCon.Dispose();
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }

        /// <summary>更新管理員</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int updateMember_Other(InfoModel info, String password, int level)
        {
            int result = 0;
            String sql1 = null;
            if (password != "")
            {
                sql1 = @"UPDATE `u101b117_account`
                         SET `username` = @username,
	                         `password` = @password,
	                         `level` = @level
                         WHERE `uid` = @account_uid;";
            }
            else
            {
                sql1 = @"UPDATE `u101b117_account`
                         SET `username` = @username,
	                         `level` = @level
                         WHERE `uid` = @account_uid;";
            }
            String sql2 = @"UPDATE `u101b117_info`
                            SET `displayname` = @displayname,
	                            `birthday` = @birthday,	
	                            `sex` = @sex,
	                            `address` = @address,
	                            `phone1` = @phone1,
	                            `phone2` = @phone2
                            WHERE `account_uid` = @account_uid;";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql1;
                    if (password != null)
                    {
                        mysqlCmd.Parameters.AddWithValue("@username", info.username);
                        mysqlCmd.Parameters.AddWithValue("@password", password);
                        mysqlCmd.Parameters.AddWithValue("@level", level);
                        mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    }
                    else
                    {
                        mysqlCmd.Parameters.AddWithValue("@username", info.username);
                        mysqlCmd.Parameters.AddWithValue("@level", level);
                        mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    }
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql2;
                    mysqlCmd.Parameters.AddWithValue("@displayname", info.displayname);
                    mysqlCmd.Parameters.AddWithValue("@birthday", info.birthday);
                    mysqlCmd.Parameters.AddWithValue("@sex", info.sex);
                    mysqlCmd.Parameters.AddWithValue("@address", info.address);
                    mysqlCmd.Parameters.AddWithValue("@phone1", info.phone1);
                    mysqlCmd.Parameters.AddWithValue("@phone2", info.phone2);
                    mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    result += mysqlCmd.ExecuteNonQuery();
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

        /// <summary>更新照服員資料</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int updateMember_Attendant(Info_AttendantModel info, String password, int level)
        {
            int result = 0;
            String sql1 = null;
            if (password != "")
            {
                sql1 = @"UPDATE `u101b117_account`
                         SET `username` = @username,
	                         `password` = @password,
	                         `level` = @level
                         WHERE `uid` = @account_uid;";
            }
            else
            {
                sql1 = @"UPDATE `u101b117_account`
                         SET `username` = @username,
	                         `level` = @level
                         WHERE `uid` = @account_uid;";
            }
            String sql2 = @"UPDATE `u101b117_info`
                            SET `displayname` = @displayname,
	                            `birthday` = @birthday,	
	                            `sex` = @sex,
	                            `address` = @address,
	                            `phone1` = @phone1,
	                            `phone2` = @phone2
                            WHERE `account_uid` = @account_uid;";
            String sql3 = @"DELETE FROM `u101b117_info_attendant`
                            WHERE `account_uid_1` = @account_uid";
            String sql4 = @"DELETE FROM `u101b117_info_license`
                            WHERE `account_uid` = @account_uid";
            String sql5 = @"INSERT INTO `u101b117_info_attendant`
							(`account_uid_1`,`account_uid_2`,
								`pay`,`firstday`, `summary`)
							VALUES(@attendantID,@supervisorID,@pay,
									@firstday,@summary);";
            String sql6 = @"INSERT INTO `u101b117_info_license`
							(`account_uid`, `license_uid`)
							VALUES";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql1;
                    if (password != null)
                    {
                        mysqlCmd.Parameters.AddWithValue("@username", info.username);
                        mysqlCmd.Parameters.AddWithValue("@password", password);
                        mysqlCmd.Parameters.AddWithValue("@level", level);
                        mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    }
                    else
                    {
                        mysqlCmd.Parameters.AddWithValue("@username", info.username);
                        mysqlCmd.Parameters.AddWithValue("@level", level);
                        mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    }
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql2;
                    mysqlCmd.Parameters.AddWithValue("@displayname", info.displayname);
                    mysqlCmd.Parameters.AddWithValue("@birthday", info.birthday);
                    mysqlCmd.Parameters.AddWithValue("@sex", info.sex);
                    mysqlCmd.Parameters.AddWithValue("@address", info.address);
                    mysqlCmd.Parameters.AddWithValue("@phone1", info.phone1);
                    mysqlCmd.Parameters.AddWithValue("@phone2", info.phone2);
                    mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.CommandText = sql3;
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.CommandText = sql4;
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql5;
                    mysqlCmd.Parameters.AddWithValue("@attendantID", info.account_uid);
                    mysqlCmd.Parameters.AddWithValue("@supervisorID", info.supervisorID);
                    mysqlCmd.Parameters.AddWithValue("@pay", info.pay);
                    mysqlCmd.Parameters.AddWithValue("@firstday", info.firstday);
                    mysqlCmd.Parameters.AddWithValue("@summary", info.summary);
                    result += mysqlCmd.ExecuteNonQuery();
                    if (info.lsLicense != null)
                    {
                        mysqlCmd.Parameters.Clear();
                        for (int i = 0; i < info.lsLicense.Count; i++)
                        {
                            sql6 += "(@attendantID" + i + ",@license_uid" + i + "),";
                        }
                        sql6 = sql6.Substring(0, sql6.Length - 1);
                        mysqlCmd.CommandText = sql6;
                        for (int i = 0; i < info.lsLicense.Count; i++)
                        {
                            mysqlCmd.Parameters.AddWithValue("@attendantID" + i, info.account_uid);
                            mysqlCmd.Parameters.AddWithValue("@license_uid" + i, info.lsLicense[i]);
                        }
                        result += mysqlCmd.ExecuteNonQuery();
                    }
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

        /// <summary>更新案主資料</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int updateMember_Employer(Info_EmployerModel info, String password, int level)
        {
            int result = 0;
            String sql1 = null;
            if (password != "")
            {
                sql1 = @"UPDATE `u101b117_account`
                         SET `username` = @username,
	                         `password` = @password,
	                         `level` = @level
                         WHERE `uid` = @account_uid;";
            }
            else
            {
                sql1 = @"UPDATE `u101b117_account`
                         SET `username` = @username,
	                         `level` = @level
                         WHERE `uid` = @account_uid;";
            }
            String sql2 = @"UPDATE `u101b117_info`
                            SET `displayname` = @displayname,
	                            `birthday` = @birthday,	
	                            `sex` = @sex,
	                            `address` = @address,
	                            `phone1` = @phone1,
	                            `phone2` = @phone2
                            WHERE `account_uid` = @account_uid;";
            String sql3 = @"DELETE FROM `u101b117_info_employer`
                            WHERE `account_uid_1` = @account_uid";
            String sql4 = @"INSERT INTO `u101b117_info_employer`
			                            (`account_uid_1`, `account_uid_2`, `info_employer_sub_uid`,
				                            `info_employer_item1_uid`, `info_employer_item2_uid`,
				                            `info_employer_item3_uid`, `emg1_displayname`,
				                            `emg1_phone1`,`emg1_phone2`,`emg2_displayname`,
				                            `emg2_phone1`,`emg2_phone2`,
				                            `minutes1`,`minutes2`,
				                            `summary`)
			                            VALUES (@employerID,@supervisorID,
					                            @sub,@item1,@item2,@item3,
					                            @emg1_displayname,@emg1_phone1,
					                            @emg1_phone2,
                                                @emg2_displayname,@emg2_phone1,
					                            @emg2_phone2,
                                                @minutes1,
					                            @minutes2,@summary);";
            try
            {
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                mysqlCon.Open();
                MySqlTransaction mysqlTransaction = mysqlCon.BeginTransaction();
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                try
                {
                    mysqlCmd.CommandText = sql1;
                    if (password != null)
                    {
                        mysqlCmd.Parameters.AddWithValue("@username", info.username);
                        mysqlCmd.Parameters.AddWithValue("@password", password);
                        mysqlCmd.Parameters.AddWithValue("@level", level);
                        mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    }
                    else
                    {
                        mysqlCmd.Parameters.AddWithValue("@username", info.username);
                        mysqlCmd.Parameters.AddWithValue("@level", level);
                        mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    }
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql2;
                    mysqlCmd.Parameters.AddWithValue("@displayname", info.displayname);
                    mysqlCmd.Parameters.AddWithValue("@birthday", info.birthday);
                    mysqlCmd.Parameters.AddWithValue("@sex", info.sex);
                    mysqlCmd.Parameters.AddWithValue("@address", info.address);
                    mysqlCmd.Parameters.AddWithValue("@phone1", info.phone1);
                    mysqlCmd.Parameters.AddWithValue("@phone2", info.phone2);
                    mysqlCmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.CommandText = sql3;
                    result += mysqlCmd.ExecuteNonQuery();
                    mysqlCmd.Parameters.Clear();
                    mysqlCmd.CommandText = sql4;
                    mysqlCmd.Parameters.AddWithValue("@employerID", info.account_uid);
                    mysqlCmd.Parameters.AddWithValue("@supervisorID", info.supervisorID);
                    mysqlCmd.Parameters.AddWithValue("@sub", info.info_employer_sub_uid);
                    mysqlCmd.Parameters.AddWithValue("@item1", info.info_employer_item1_uid);
                    mysqlCmd.Parameters.AddWithValue("@item2", info.info_employer_item2_uid);
                    String str = "";
                    if (info.info_employer_item3_uid != null)
                    {
                        for (int i = 0; i < info.info_employer_item3_uid.Count; i++)
                        {
                            str += info.info_employer_item3_uid[i] + ",";
                        }
                    }
                    mysqlCmd.Parameters.AddWithValue("@item3", str.Substring(0, str.Length - 1));
                    mysqlCmd.Parameters.AddWithValue("@emg1_displayname", info.emg1_displayname);
                    mysqlCmd.Parameters.AddWithValue("@emg1_phone1", info.emg1_phone1);
                    mysqlCmd.Parameters.AddWithValue("@emg1_phone2", info.emg1_phone2);
                    mysqlCmd.Parameters.AddWithValue("@emg2_displayname", info.emg2_displayname);
                    mysqlCmd.Parameters.AddWithValue("@emg2_phone1", info.emg2_phone1);
                    mysqlCmd.Parameters.AddWithValue("@emg2_phone2", info.emg2_phone2);
                    mysqlCmd.Parameters.AddWithValue("@minutes1", info.minutes1);
                    mysqlCmd.Parameters.AddWithValue("@minutes2", info.minutes2);
                    mysqlCmd.Parameters.AddWithValue("@summary", info.summary);
                    result += mysqlCmd.ExecuteNonQuery();
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

        /// <summary>更新督導資料</summary>
        /// <param name="info"></param>
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public int updateMember_Supervisor(Info_SupervisorModel info, String password, int level)
        {
            dbHelper = new DBHelper();
            int result = 0;
            String sql1 = null;
            if (password != "")
            {
                sql1 = @"UPDATE `u101b117_account`
                         SET `username` = @username,
	                         `password` = @password,
	                         `level` = @level
                         WHERE `uid` = @account_uid;";
            }
            else
            {
                sql1 = @"UPDATE `u101b117_account`
                         SET `username` = @username,
	                         `level` = @level
                         WHERE `uid` = @account_uid;";
            }
            String sql2 = @"UPDATE `u101b117_info`
                            SET `displayname` = @displayname,
	                            `birthday` = @birthday,	
	                            `sex` = @sex,
	                            `address` = @address,
	                            `phone1` = @phone1,
	                            `phone2` = @phone2
                            WHERE `account_uid` = @account_uid;";
            String sql3 = @"DELETE FROM `u101b117_info_supervisor`
                            WHERE `account_uid` = @account_uid;";
            String sql4 = @"DELETE FROM `u101b117_info_license`
                            WHERE `account_uid` = @account_uid;";
            String sql5 = @"INSERT INTO `u101b117_info_supervisor`
			                            (`account_uid`, `firstday`, `train`, `probg`, `edubg`)
			                            VALUES(@account_uid, @firstday, @train, @probg, @edubg);";
            String sql6 = @"INSERT INTO `u101b117_info_license`
			                            (`account_uid`, `license_uid`)
			                            VALUES";
            try
            {
                dbHelper.BeginTransaction();
                var cmd = dbHelper.GetCommand(sql1);
                if (password != null)
                {
                    cmd.Parameters.AddWithValue("@username", info.username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@level", level);
                    cmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@username", info.username);
                    cmd.Parameters.AddWithValue("@level", level);
                    cmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                }
                result += dbHelper.ExecuteNonQuery(cmd);
                //資料
                cmd.Parameters.Clear();
                cmd = dbHelper.GetCommand(sql2);
                cmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                cmd.Parameters.AddWithValue("@displayname", info.displayname);
                cmd.Parameters.AddWithValue("@birthday", info.birthday);
                cmd.Parameters.AddWithValue("@sex", info.sex);
                cmd.Parameters.AddWithValue("@address", info.address);
                cmd.Parameters.AddWithValue("@phone1", info.phone1);
                cmd.Parameters.AddWithValue("@phone2", info.phone2);
                result += dbHelper.ExecuteNonQuery(cmd);
                //刪除舊資料
                cmd.Parameters.Clear();
                cmd = dbHelper.GetCommand(sql3);
                cmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                result += dbHelper.ExecuteNonQuery(cmd);
                cmd.Parameters.Clear();
                cmd = dbHelper.GetCommand(sql4);
                cmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                result += dbHelper.ExecuteNonQuery(cmd);
                //督導限定
                cmd.Parameters.Clear();
                cmd = dbHelper.GetCommand(sql5);
                cmd.Parameters.AddWithValue("@firstday", info.firstday);
                String lsTrain = "";
                if (info.lsTrain != null)
                {
                    for (int i = 0; i < info.lsTrain.Count; i++)
                    {
                        lsTrain += info.lsTrain[i] + ",";
                    }
                    lsTrain = lsTrain.Substring(0, lsTrain.Length - 1);
                }
                cmd.Parameters.AddWithValue("@account_uid", info.account_uid);
                cmd.Parameters.AddWithValue("@train", lsTrain);
                cmd.Parameters.AddWithValue("@probg", info.proBG);
                cmd.Parameters.AddWithValue("@edubg", info.eduBG);
                result += dbHelper.ExecuteNonQuery(cmd);
                //督導證照
                if (info.lsLicense != null)
                {
                    for (int i = 0; i < info.lsLicense.Count; i++)
                    {
                        sql6 += "('" + info.account_uid + "', @license_uid" + i + "),";
                    }
                    sql6 = sql6.Substring(0, sql6.Length - 1);
                    cmd.Parameters.Clear();
                    cmd = dbHelper.GetCommand(sql6);
                    for (int i = 0; i < info.lsLicense.Count; i++)
                    {
                        cmd.Parameters.AddWithValue("@license_uid" + i, info.lsLicense[i]);
                    }
                    result += dbHelper.ExecuteNonQuery(cmd);
                }
                dbHelper.TransactionCommit();
            }
            catch (MySqlException ex)
            {
                dbHelper.TransactionRollback();
                Console.WriteLine(ex.Message);
                result = -1;
            }

            return result;
        }


        /// <summary>刪除成員</summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int delectMember(int account_uid)
        {
            int result = 0;
            try
            {
                String sql = "UPDATE u101b117_account"
                    + " SET `level` = 5"
                    + " WHERE `uid` = @account_uid;";
                MySqlConnection mysqlCon = new MySqlConnection(conStr);
                MySqlCommand mysqlCmd = mysqlCon.CreateCommand();
                mysqlCon.Open();
                mysqlCmd.CommandText = sql;
                mysqlCmd.Parameters.AddWithValue("@account_uid", account_uid);
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

        #region Supervisor_Report

        /// <summary>搜尋指定督導編號旗下照服員清單</summary>
        /// <param name="SupervisorID">督導編號</param>
        /// <returns></returns>
        public List<GridRptWorkRecordModel> selectGridRptWorkRecordForSupervisorID(int SupervisorID)
        {
            List<GridRptWorkRecordModel> data = new List<GridRptWorkRecordModel>();
            String sql = @"SELECT a.account_uid_1 AS AttendantID,
		                            a.account_uid_2 AS EmployerID,
		                            (SELECT z.displayname 
		                            FROM `u101b117_info` AS z
		                            WHERE a.account_uid_2=z.account_uid) AS EmployerName,
		                            (SELECT z.displayname 
		                            FROM `u101b117_info` AS z
		                            WHERE a.account_uid_1=z.account_uid) AS displayname,
		                            (SELECT z.phone1 
		                            FROM `u101b117_info` AS z
		                            WHERE a.account_uid_1=z.account_uid) AS phone,
		                            COUNT(a.start) AS Count,
		                            YEAR(a.start) AS Year,
		                            MONTH(a.start) AS Month,
		                            SUM(TIMESTAMPDIFF(MINUTE,a.start,a.end)) AS Worktime
                            FROM `u101b117_schedule` AS a
                            WHERE (SELECT account_uid_2 
		                            FROM `u101b117_info_attendant` AS z 
		                            WHERE a.account_uid_1=z.account_uid_1)=@SupervisorID
                            GROUP BY Year,Month,a.account_uid_2,a.account_uid_1
                            ORDER BY a.account_uid_1,Year,Month;";
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
                    mysqlCmd.Parameters.AddWithValue("@SupervisorID", SupervisorID);
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        String month = mysqlDataReader["Month"].ToString();
                        if (int.Parse(month) < 10)
                        {
                            month = "0" + month;
                        }
                        data.Add(new GridRptWorkRecordModel
                        {
                            AttendantID = int.Parse(mysqlDataReader["AttendantID"].ToString()),
                            EmployerID = int.Parse(mysqlDataReader["EmployerID"].ToString()),
                            EmployerName = mysqlDataReader["EmployerName"].ToString(),
                            Displayname = mysqlDataReader["Displayname"].ToString(),
                            phone = mysqlDataReader["phone"].ToString(),
                            Count = int.Parse(mysqlDataReader["Count"].ToString()),
                            Year = int.Parse(mysqlDataReader["Year"].ToString()),
                            Month = int.Parse(mysqlDataReader["Month"].ToString()),
                            Year_Month = mysqlDataReader["Year"].ToString() + "-" + month,
                            Worktime = int.Parse(mysqlDataReader["Worktime"].ToString())
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


        /// <summary>搜尋WorkRecord編號</summary>
        /// <param name="AttendantID"></param>
        /// <param name="EmployerID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public DataTable selectWorkRecordPeopleData(int AttendantID, int EmployerID, int Year, int Month)
        {
            DataTable dt = null;
            try
            {
                dbHelper = new DBHelper();
                var cmd = dbHelper.GetCommand(@"SELECT a.uid,DAY(a.start) AS day,
		                                                (SELECT z.sex 
		                                                 FROM `u101b117_info` AS z
		                                                 WHERE a.account_uid_2=z.account_uid) AS sex,
		                                                (SELECT minutes1 
		                                                 FROM `u101b117_info_employer` AS z
		                                                 WHERE a.account_uid_2=z.account_uid_1) AS time,
		                                                IFNULL((SELECT SUM(minutes)
				                                                FROM `u101b117_work_service` AS z
				                                                WHERE ((SELECT service_item_uid
						                                                FROM `u101b117_service` AS x
						                                                WHERE z.service_uid = x.uid) = 2)
				                                                AND a.uid = z.schedule_uid), 0) AS min1,
		                                                IFNULL((SELECT SUM(minutes)
				                                                FROM `u101b117_work_service` AS z
				                                                WHERE ((SELECT service_item_uid
						                                                FROM `u101b117_service` AS x
						                                                WHERE z.service_uid = x.uid) = 3)
				                                                AND a.uid = z.schedule_uid), 0) AS min2,
		                                                IFNULL((SELECT COUNT(minutes)
				                                                FROM `u101b117_work_service` AS z
				                                                WHERE ((SELECT service_item_uid
						                                                FROM `u101b117_service` AS x
						                                                WHERE z.service_uid = x.uid) = 2)
				                                                AND a.uid = z.schedule_uid
				                                                AND minutes != 0), 0) AS count_min1,
		                                                IFNULL((SELECT COUNT(minutes)
				                                                FROM `u101b117_work_service` AS z
				                                                WHERE ((SELECT service_item_uid
						                                                FROM `u101b117_service` AS x
						                                                WHERE z.service_uid = x.uid) = 3)
				                                                AND a.uid = z.schedule_uid
				                                                AND minutes != 0), 0) AS count_min2,
                                                        IFNULL((TIME_TO_SEC((SELECT TIMEDIFF(MAX(end),MIN(start)) 
			                                                                 FROM `u101b117_work_record` AS z 
			                                                                 WHERE a.uid=z.schedule_uid))),0) AS total_time		
                                                FROM u101b117_schedule AS a
                                                WHERE a.account_uid_1 = @AttendantID
                                                AND a.account_uid_2 = @EmployerID
                                                AND YEAR(a.start) = @Year
                                                AND MONTH(a.start) = @Month
                                                ORDER BY a.start;");
                cmd.Parameters.AddWithValue("@AttendantID", AttendantID);
                cmd.Parameters.AddWithValue("@EmployerID", EmployerID);
                cmd.Parameters.AddWithValue("@Year", Year);
                cmd.Parameters.AddWithValue("@Month", Month);
                dt = dbHelper.GetDataTable(cmd);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dt;
        }

        /// <summary>搜尋WorkRecord編號</summary>
        /// <param name="AttendantID"></param>
        /// <param name="EmployerID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<DataTable> selectWorkRecordPeopleData(List<int> AttendantID, List<int> EmployerID, List<int> Year, List<int> Month)
        {
            List<DataTable> dt = new List<DataTable>(); ;
            try
            {
                int tableSize = AttendantID.Count;
                dbHelper = new DBHelper();
                for (int i = 0; i < tableSize; i++)
                {
                    var cmd = dbHelper.GetCommand(@"SELECT a.uid,DAY(a.start) AS day,
		                                                    (SELECT z.sex 
		                                                     FROM `u101b117_info` AS z
		                                                     WHERE a.account_uid_2=z.account_uid) AS sex,
		                                                    (SELECT minutes1 
		                                                     FROM `u101b117_info_employer` AS z
		                                                     WHERE a.account_uid_2=z.account_uid_1) AS time,
		                                                    IFNULL((SELECT SUM(minutes)
				                                                    FROM `u101b117_work_service` AS z
				                                                    WHERE ((SELECT service_item_uid
						                                                    FROM `u101b117_service` AS x
						                                                    WHERE z.service_uid = x.uid) = 2)
				                                                    AND a.uid = z.schedule_uid), 0) AS min1,
		                                                    IFNULL((SELECT SUM(minutes)
				                                                    FROM `u101b117_work_service` AS z
				                                                    WHERE ((SELECT service_item_uid
						                                                    FROM `u101b117_service` AS x
						                                                    WHERE z.service_uid = x.uid) = 3)
				                                                    AND a.uid = z.schedule_uid), 0) AS min2,
		                                                    IFNULL((SELECT COUNT(minutes)
				                                                    FROM `u101b117_work_service` AS z
				                                                    WHERE ((SELECT service_item_uid
						                                                    FROM `u101b117_service` AS x
						                                                    WHERE z.service_uid = x.uid) = 2)
				                                                    AND a.uid = z.schedule_uid
				                                                    AND minutes != 0), 0) AS count_min1,
		                                                    IFNULL((SELECT COUNT(minutes)
				                                                    FROM `u101b117_work_service` AS z
				                                                    WHERE ((SELECT service_item_uid
						                                                    FROM `u101b117_service` AS x
						                                                    WHERE z.service_uid = x.uid) = 3)
				                                                    AND a.uid = z.schedule_uid
				                                                    AND minutes != 0), 0) AS count_min2,
                                                            IFNULL((TIME_TO_SEC((SELECT TIMEDIFF(MAX(end),MIN(start)) 
			                                                             FROM `u101b117_work_record` AS z 
			                                                             WHERE a.uid=z.schedule_uid))),0) AS total_time		
                                                    FROM u101b117_schedule AS a
                                                    WHERE a.account_uid_1 = @AccountID
                                                    AND a.account_uid_2 = @EmployerID
                                                    AND YEAR(a.start) = @Year
                                                    AND MONTH(a.start) = @Month
                                                    ORDER BY a.start;");
                    cmd.Parameters.AddWithValue("@AccountID", AttendantID[i]);
                    cmd.Parameters.AddWithValue("@EmployerID", EmployerID[i]);
                    cmd.Parameters.AddWithValue("@Year", Year[i]);
                    cmd.Parameters.AddWithValue("@Month", Month[i]);
                    dt.Add(dbHelper.GetDataTable(cmd));
                }
            }
            catch (MySqlException)
            {
                throw;
            }
            return dt;
        }

        /// <summary>搜尋指定WorkRecord</summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataTable selectWorkRecordDataTable(String strWhere)
        {
            DataTable dt = null;
            try
            {
                dbHelper = new DBHelper();
                var cmd = dbHelper.GetCommand(@"SELECT 
		                                                a.uid AS workID,
		                                                c.uid AS ServiceID,
		                                                d.uid AS Service_ItemID,
		                                                c.name AS Serivce,
		                                                d.name AS Service_item,
		                                                a.summary,
		                                                (SELECT MIN(start) 
			                                                FROM `u101b117_work_record` AS z 
			                                                WHERE a.uid=z.schedule_uid) AS start,
		                                                (SELECT MAX(end) 
			                                                FROM `u101b117_work_record` AS z 
			                                                WHERE a.uid=z.schedule_uid) AS end,
		                                                a.start AS scheduled_start,
		                                                a.end AS scheduled_end,
		                                                b.minutes
                                                FROM `u101b117_schedule` AS a
                                                JOIN `u101b117_work_service` AS b
                                                ON a.uid=b.schedule_uid
                                                JOIN `u101b117_service` AS c
                                                ON b.service_uid=c.uid
                                                JOIN `u101b117_service_item` AS d
                                                ON c.service_item_uid=d.uid
                                                WHERE " + strWhere + @"
                                                ORDER BY a.start,workID;");
                dt = dbHelper.GetDataTable(cmd);
            }
            catch (MySqlException)
            {
                throw;
            }
            return dt;
        }

        /// <summary>搜尋CaseServiceRecord編號</summary>
        /// <param name="AttendantID"></param>
        /// <param name="EmployerID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public DataTable getCaseServiceRecord(int AttendantID, int EmployerID, int Year, int Month)
        {
            DataTable dt = null;
            try
            {
                dbHelper = new DBHelper();
                var cmd = dbHelper.GetCommand(@"SELECT c.uid AS 'itemID',
	                                                   b.uid AS 'answerID',
	                                                   a.schedule_uid,
	                                                   c.name AS 'itemName',
	                                                   b.name AS 'answerName',
	                                                   a.summary
                                                FROM `u101b117_work_case_record` AS a
                                                JOIN `u101b117_case_serivce_record_answer` AS b
                                                ON a.`case_record_answer_uid` = b.`uid`
                                                JOIN `u101b117_case_serivce_record_item` AS c
                                                ON b.`case_serivce_record_item_uid` = c.`uid`
                                                JOIN `u101b117_schedule` AS d
                                                ON a.`schedule_uid` = d.`uid`
                                                WHERE d.account_uid_1 = @AttendantID
                                                AND d.account_uid_2 = @EmployerID
                                                AND YEAR(d.start) = @Year
                                                AND MONTH(d.start) = @Month
                                                ORDER BY d.start,itemID,answerID;");
                /*
                    WHERE d.account_uid_1 = @AttendantID
                    AND d.account_uid_2 = @EmployerID
                    AND YEAR(d.start) = @Year
                    AND MONTH(d.start) = @Month
                 */
                cmd.Parameters.AddWithValue("@AttendantID", AttendantID);
                cmd.Parameters.AddWithValue("@EmployerID", EmployerID);
                cmd.Parameters.AddWithValue("@Year", Year);
                cmd.Parameters.AddWithValue("@Month", Month);
                dt = dbHelper.GetDataTable(cmd);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dt;
        }

        /// <summary>搜尋CaseServiceRecord編號</summary>
        /// <param name="AttendantID"></param>
        /// <param name="EmployerID"></param>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <returns></returns>
        public List<DataTable> getCaseServiceRecord(List<int> AttendantID, List<int> EmployerID, List<int> Year, List<int> Month)
        {
            List<DataTable> dt = new List<DataTable>(); ;
            try
            {
                int tableSize = AttendantID.Count;
                dbHelper = new DBHelper();
                for (int i = 0; i < tableSize; i++)
                {
                    var cmd = dbHelper.GetCommand(@"SELECT c.uid AS 'itemID',
	                                                   b.uid AS 'answerID',
	                                                   a.schedule_uid,
	                                                   c.name AS 'itemName',
	                                                   b.name AS 'answerName',
	                                                   a.summary
                                                    FROM `u101b117_work_case_record` AS a
                                                    JOIN `u101b117_case_serivce_record_answer` AS b
                                                    ON a.`case_record_answer_uid` = b.`uid`
                                                    JOIN `u101b117_case_serivce_record_item` AS c
                                                    ON b.`case_serivce_record_item_uid` = c.`uid`
                                                    JOIN `u101b117_schedule` AS d
                                                    ON a.`schedule_uid` = d.`uid`
                                                    WHERE d.account_uid_1 = @AttendantID
                                                    AND d.account_uid_2 = @EmployerID
                                                    AND YEAR(d.start) = @Year
                                                    AND MONTH(d.start) = @Month
                                                    ORDER BY d.start,itemID,answerID;");
                    /*
                        WHERE d.account_uid_1 = @AttendantID
                        AND d.account_uid_2 = @EmployerID
                        AND YEAR(d.start) = @Year
                        AND MONTH(d.start) = @Month
                     */
                    cmd.Parameters.AddWithValue("@AttendantID", AttendantID[i]);
                    cmd.Parameters.AddWithValue("@EmployerID", EmployerID[i]);
                    cmd.Parameters.AddWithValue("@Year", Year[i]);
                    cmd.Parameters.AddWithValue("@Month", Month[i]);
                    dt.Add(dbHelper.GetDataTable(cmd));
                }
            }
            catch (MySqlException)
            {
                throw;
            }
            return dt;
        }


        /// <summary>搜尋指定CaseServiceRecord</summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable getCaseServiceRecord(int uid)
        {
            DataTable dt = null;
            try
            {
                dbHelper = new DBHelper();
                var cmd = dbHelper.GetCommand(@"SELECT c.uid AS 'itemID',
                                                       b.uid AS 'answerID',
	                                                   a.schedule_uid,
                                                       c.name AS 'itemName',
                                                       b.name AS 'answerName',
                                                       a.summary,
                                                       (SELECT start
                                                        FROM `u101b117_schedule` AS z
                                                        WHERE a.schedule_uid = z.uid) AS start,
                                                       (SELECT end
                                                        FROM `u101b117_schedule` AS z
                                                        WHERE a.schedule_uid = z.uid) AS end
                                                FROM `u101b117_work_case_record` AS a
                                                JOIN `u101b117_case_serivce_record_answer` AS b
                                                ON a.`case_record_answer_uid` = b.`uid`
                                                JOIN `u101b117_case_serivce_record_item` AS c
                                                ON b.`case_serivce_record_item_uid` = c.`uid`
                                                WHERE a.schedule_uid = @schedule_uid
                                                ORDER BY start,itemID,answerID;");
                cmd.Parameters.AddWithValue("schedule_uid", uid);
                dt = dbHelper.GetDataTable(cmd);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dt;
        }

        #endregion

        #region Supervisor_Map

        /// <summary>取得督導編號&姓名&電話</summary>
        /// <returns></returns>
        public DataTable selectSupervisorData()
        {
            dbHelper = new DBHelper();
            DataTable dt;
            try
            {
                dbHelper.BeginTransaction();
                var cmd = dbHelper.GetCommand(@"SELECT a.account_uid,a.displayname,a.phone1
                                                FROM `u101b117_info` AS a
                                                WHERE (SELECT level
	                                                   FROM `u101b117_account` AS z
	                                                   WHERE a.account_uid = z.uid) = 2;");
                dt = dbHelper.GetDataTable(cmd);
                dbHelper.TransactionCommit();
            }
            catch (Exception ex)
            {
                dbHelper.TransactionRollback();
                Console.WriteLine(ex.Message);
                dt = null;
            }
            return dt;
        }


        /// <summary>用督導編號搜尋案主資料</summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public DataTable selectEmployerDataForSupervisorID(int uid)
        {
            dbHelper = new DBHelper();
            DataTable dt;
            try
            {
                dbHelper.BeginTransaction();
                var cmd = dbHelper.GetCommand(@"SELECT a.account_uid AS account_uid,
	                                                   a.displayname,a.phone1,a.address,
	                                                   (SELECT COUNT(z.start) 
		                                                FROM `u101b117_schedule` AS z
		                                                WHERE (CONVERT_TZ(UTC_TIMESTAMP(),'+00:00','+08:00') BETWEEN z.start AND z.end)
		                                                AND a.account_uid=z.account_uid_2) AS online
                                                FROM `u101b117_info` AS a
                                                WHERE (SELECT z.account_uid_2
	                                                   FROM `u101b117_info_employer` AS z
	                                                   WHERE a.account_uid = z.account_uid_1) = @SupervisorID;");
                cmd.Parameters.AddWithValue("@SupervisorID", uid);
                dt = dbHelper.GetDataTable(cmd);
                dbHelper.TransactionCommit();
            }
            catch (Exception ex)
            {
                dbHelper.TransactionRollback();
                Console.WriteLine(ex.Message);
                dt = null;
            }
            return dt;
        }


        #endregion

    }
}