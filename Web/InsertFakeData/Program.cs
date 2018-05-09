using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        public static String getConstr(int i)
        {
            String serverIP = "";
            String serverUser = "";
            String serverPwd = "";
            String serverPort = "";
            String serverDataBaseName = "";
            String conStr = "";
            switch (i)
            {
                case 0:
                    {
                        //HK-Server
                        serverIP = "www.mobilelabhk.info";
                        serverUser = "finalproject";
                        serverPwd = "M6unm4G4jyNseWv4";
                        serverPort = "5000";
                        serverDataBaseName = "finalproject";
                        break;
                    }
                case 1:
                    {
                        //Godaddy
                        serverIP = "tomatoLab.db.8394846.hostedresource.com";
                        serverUser = "tomatoLab";
                        serverPwd = "CSIE#j504";
                        serverPort = "3306";
                        serverDataBaseName = "tomatoLab";
                        break;
                    }
                case 2:
                    {
                        //shuang.myftp.org
                        serverIP = "shuang.myftp.org";
                        serverUser = "tomatoLab";
                        serverPwd = "zxcv1111";
                        serverPort = "3306";
                        serverDataBaseName = "tomatoLab";
                        break;
                    }
            }
            conStr = "Server=" + serverIP
                  + ";Port=" + serverPort
                  + ";Database=" + serverDataBaseName
                  + ";User=" + serverUser
                  + ";Pwd=" + serverPwd
                  + ";CharSet=utf8;";
            return conStr;
        }



        static void Main(string[] args)
        {
            String conStr = getConstr(0);
            String sql1 = @"SELECT a.uid,a.start,a.end,a.account_uid_2,
		                            (SELECT GROUP_CONCAT(service_uid SEPARATOR ',')
		                             FROM u101b117_schedule_service
		                             WHERE schedule_uid=a.uid) AS serviceItem
                            FROM u101b117_schedule AS a;";
            String sql2 = "INSERT u101b117_work_record (schedule_uid,start,end,equipment_uid_1,equipment_uid_2,update_time) VALUES";
            String sql3 = "SELECT uid FROM u101b117_service WHERE isdelete = 0;";
            String sql4 = "INSERT u101b117_work_service (schedule_uid,service_uid,minutes) VALUES";
            String sql5 = @"INSERT `u101b117_work_case_record` ( schedule_uid, case_record_answer_uid, update_time) 
                                                        VALUES ( @schedule_uid, @case_record_answer_uid_1, @update_time ),
                                                               ( @schedule_uid, @case_record_answer_uid_2, @update_time ),
                                                               ( @schedule_uid, @case_record_answer_uid_3, @update_time ),
                                                               ( @schedule_uid, @case_record_answer_uid_4, @update_time ),
                                                               ( @schedule_uid, @case_record_answer_uid_5, @update_time ),
                                                               ( @schedule_uid, @case_record_answer_uid_6, @update_time ),
                                                               ( @schedule_uid, @case_record_answer_uid_7, @update_time ),
                                                               ( @schedule_uid, @case_record_answer_uid_8, @update_time ),
                                                               ( @schedule_uid, @case_record_answer_uid_9, @update_time );";
            try
            {
                List<String> lsService = new List<string>();
                List<String> lsUID = new List<String>();
                List<DateTime> lsStart = new List<DateTime>();
                List<DateTime> lsEnd = new List<DateTime>();
                List<String> lsEmployerID = new List<String>();
                List<String> lsServiceID = new List<String>();
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
                        lsUID.Add(mysqlDataReader["uid"].ToString());
                        lsStart.Add(DateTime.Parse(mysqlDataReader["start"].ToString()));
                        lsEnd.Add(DateTime.Parse(mysqlDataReader["end"].ToString()));
                        lsEmployerID.Add(mysqlDataReader["account_uid_2"].ToString());
                        lsService.Add(mysqlDataReader["serviceItem"].ToString());
                    }
                    mysqlDataReader.Dispose();
                    for (int i = 0; i < lsEmployerID.Count; i++)
                    {
                        int beaconID = 0;
                        if (lsEmployerID[i] == "5")
                        {
                            beaconID = 2;
                        }
                        else if (lsEmployerID[i] == "83")
                        {
                            beaconID = 3;
                        }
                        sql2 += "('" + lsUID[i] + "','" + lsStart[i].ToString("yyyy/MM/dd HH:mm:ss") + "','" + lsEnd[i].ToString("yyyy/MM/dd HH:mm:ss") + "','" + beaconID + "','12','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "')";
                        if (lsEmployerID.Count - 1 != i)
                        {
                            sql2 += ",";
                        }
                    }
                    mysqlCmd.CommandText = sql2;
                    int insertRow = mysqlCmd.ExecuteNonQuery();
                    Console.WriteLine("lsEmployerID：" + lsEmployerID.Count);
                    Console.WriteLine("lsStart：" + lsStart.Count);
                    Console.WriteLine("lsEnd：" + lsEnd.Count);
                    Console.WriteLine("新增：" + insertRow);
                    mysqlCmd.CommandText = sql3;
                    mysqlDataReader = mysqlCmd.ExecuteReader();
                    while (mysqlDataReader.Read())
                    {
                        lsServiceID.Add(mysqlDataReader["uid"].ToString());
                    }
                    mysqlDataReader.Dispose();
                    for (int i = 0; i < lsUID.Count; i++)
                    {
                        if (lsService[i] == "")
                        {
                            for (int j = 0; j < lsServiceID.Count; j++)
                            {
                                sql4 += "(" + lsUID[i] + "," + lsServiceID[j] + ",10),";
                            }
                        }
                        else
                        {
                            List<string> tmp = new List<string>(lsService[i].Split(','));
                            for (int j = 0; j < lsServiceID.Count; j++)
                            {
                                int x = 0;
                                for (int k = 0; k < tmp.Count; k++)
                                {
                                    if (lsServiceID[j] == tmp[k])
                                    {
                                        x = 10;
                                        break;
                                    }
                                }
                                sql4 += "(" + lsUID[i] + "," + lsServiceID[j] + "," + x + "),";
                            }
                        }
                    }
                    sql4 = sql4.TrimEnd(',');
                    mysqlCmd.CommandText = sql4;
                    Console.WriteLine("新增：" + mysqlCmd.ExecuteNonQuery());
                    int work_case_record = 0;
                    for (int i = 0; i < lsUID.Count; i++)
                    {
                        Random random = new Random();
                        mysqlCmd.CommandText = sql5;
                        mysqlCmd.Parameters.AddWithValue("@schedule_uid", lsUID[i]);
                        mysqlCmd.Parameters.AddWithValue("@update_time", DateTime.Now);
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_1", random.Next(1, 4));
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_2", random.Next(4, 6));
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_3", random.Next(6, 9));
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_4", random.Next(9, 12));
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_5", random.Next(12, 18));
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_6", random.Next(18, 21));
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_7", random.Next(21, 23));
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_8", "23");
                        mysqlCmd.Parameters.AddWithValue("@case_record_answer_uid_9", "24");
                        work_case_record += mysqlCmd.ExecuteNonQuery();
                        mysqlCmd.Parameters.Clear();
                    }
                    Console.WriteLine("新增：" + work_case_record);
                    mysqlTransaction.Commit();
                }
                catch (MySqlException ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    //取消交易，復原至交易前
                    mysqlTransaction.Rollback();
                    //列出訊息
                    Console.WriteLine(ex.Message);
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
            }
            //不關閉
            Console.ReadLine();
        }
    }
}
