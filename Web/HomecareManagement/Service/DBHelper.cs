using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace HomecareManagement.Service
{
    /// <summary>
    /// SQL SERVER 存取物件，表像模式
    /// </summary>
    public class DBHelper
    {

        /// <summary>資料庫連線字串
        /// </summary>
        string _connectionString;

        public DBHelper()
        {
            Initialize();
        }
        /// <summary>
        /// </summary>
        /// <param name="ConnectionString">資料庫連線字串</param>
        private void Initialize()
        {
            _connectionString = ConStr.getConstr(0);
            _connection = new MySqlConnection(_connectionString);
        }

        /// <summary>傳入之前使用的Transaction，讓此資料庫存取，使用同一個Transaction
        /// </summary>
        /// <param name="transaction"></param>
        public DBHelper(MySqlTransaction transaction)
        {
            this._transaction = transaction;
        }

        #region 資料庫連線 Connection

        MySqlConnection _connection;

        /// <summary>取得Connection物件
        /// </summary>
        public MySqlConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>開啟連線
        /// </summary>
        public void ConnectionOpen()
        {
            if (Connection.State == ConnectionState.Closed && _transaction == null)
                Connection.Open();
        }

        /// <summary>結束連線
        /// </summary>
        public void ConnectionClose()
        {
            if (Connection.State == ConnectionState.Open && _transaction == null)
                Connection.Close();
        }

        #endregion

        #region 資料庫指令 Command

        /// <summary>取得SqlCommand物件，預設CommandType=Text
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <returns>SqlCommand物件</returns>
        public MySqlCommand GetCommand(string sql)
        {
            return GetCommand(sql, CommandType.Text);
        }

        /// <summary>取得SqlCommand物件，預設CommandType=Text
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <param name="Arr_sParam">要傳入的參數，名稱與值的對應</param>
        /// <returns>SqlCommand物件</returns>
        public MySqlCommand GetCommand(string sql, params string[] Arr_sParam)
        {
            return GetCommand(sql, CommandType.Text, Arr_sParam);
        }

        /// <summary>取得SqlCommand物件
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <param name="comType">Command類型</param>
        /// <returns>SqlCommand物件</returns>
        public MySqlCommand GetCommand(string sql, CommandType comType)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sql;
            command.CommandType = comType;
            if (this._transaction == null)
                command.Connection = this.Connection;
            else
                command.Transaction = this._transaction;
            return command;
        }

        /// <summary>取得SqlCommand物件
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <param name="comType">Command類型</param>
        /// <param name="fields">參數名稱與值的對應</param>
        /// <returns>SqlCommand物件</returns>
        public MySqlCommand GetCommand(string sql, CommandType comType, NameValueCollection fields)
        {
            MySqlCommand command = this.GetCommand(sql, comType);
            if (fields != null)
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    command.Parameters.Add(new MySqlParameter(fields.Keys[i].ToString(), fields[i]));
                }
            }
            return command;
        }

        /// <summary>取得SqlCommand物件
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <param name="comType">Command類型</param>
        /// <param name="Arr_sParam">參數名稱與值的對應</param>
        /// <returns>SqlCommand物件</returns>
        public MySqlCommand GetCommand(string sql, CommandType comType, params string[] Arr_sParam)
        {
            MySqlCommand command = this.GetCommand(sql, comType);
            for (int iParamIndex = 0; iParamIndex < Arr_sParam.Length; iParamIndex++)
            {
                command.Parameters.Add(new MySqlParameter(Arr_sParam[iParamIndex], Arr_sParam[++iParamIndex]));
            }

            return command;
        }

        ///// <summary>取得SqlCommand物件
        ///// </summary>
        ///// <param name="cdataCommand">CDataCommand</param>
        ///// <returns>SqlCommand物件</returns>
        //public SqlCommand getCommand(CDataCommand cdataCommand)
        //{
        //    return this.getCommand(cdataCommand.CommandText,
        //        cdataCommand.CommandType, cdataCommand.getFields());

        //}

        #endregion

        #region 資料庫存取

        /// <summary>執行Command，並傳回異動筆數
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>執行結果，傳回資料易動筆數</returns>
        public int ExecuteNonQuery(MySqlCommand command)
        {
            if (command.Connection == null)
                command.Connection = this.Connection;
            int iValue = 0;
            try
            {
                this.ConnectionOpen();
                iValue = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }
            return iValue;
        }

        /// <summary>執行Command，並傳回執行結果
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <returns>執行結果，傳回資料易動筆數</returns>
        public int ExecuteNonQuery(string sql)
        {
            MySqlCommand command = this.GetCommand(sql);
            return this.ExecuteNonQuery(command);
        }

        /// <summary>執行Command，並傳回執行結果
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>執行結果，只傳回第一筆第一列的資料</returns>
        public object ExecuteScalar(MySqlCommand command)
        {
            if (command.Connection == null)
                command.Connection = this.Connection;
            object obj = null;
            try
            {
                this.ConnectionOpen();
                obj = command.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }
            return obj;
        }

        /// <summary>執行Command，並傳回執行結果
        /// </summary>
        /// <param name="sql">SQL語法</param>
        /// <returns>執行結果，只傳回第一筆第一列的資料</returns>
        public object ExecuteScalar(string sql)
        {
            MySqlCommand command = this.GetCommand(sql);
            return this.ExecuteScalar(command);
        }

        /// <summary>執行Command，並傳回執行結果</summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(String sql)
        {
            MySqlCommand command = this.GetCommand(sql);
            if (command.Connection == null)
                command.Connection = this.Connection;
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }

            return dt;
        }


        /// <summary>取得資料
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(MySqlCommand command)
        {
            if (command.Connection == null)
                command.Connection = this.Connection;
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }

            return dt;
        }

        /// <summary>取得DataSet資料
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(MySqlCommand command)
        {
            if (command.Connection == null)
                command.Connection = this.Connection;
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            DataSet ds = new DataSet();
            try
            {
                da.Fill(ds);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }
            return ds;
        }

        /// <summary>填滿資料到傳入的DataTable
        /// </summary>
        /// <param name="dt">要填滿的DataTable</param>
        /// <param name="command">SqlCommand物件</param>
        public void FillDataTable(DataTable dt, string command)
        {
            FillDataTable(dt, this.GetCommand(command));
        }

        /// <summary>依傳入的DS結構，來填資料。
        /// </summary>
        /// <param name="dt">DT結構</param>
        /// <param name="command">SqlCommand物件</param>
        public void FillDataTable(DataTable dt, MySqlCommand command)
        {
            if (command.Connection == null)
                command.Connection = this.Connection;
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            try
            {
                da.Fill(dt);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }
        }

        /// <summary>依傳入的DataSet結構，來填資料。
        /// </summary>
        /// <param name="ds">DataSet結構</param>
        /// <param name="command">SqlCommand物件</param>
        public void FillDataSet(DataSet ds, MySqlCommand command)
        {
            if (command.Connection == null)
                command.Connection = this.Connection;
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            try
            {
                da.Fill(ds);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }

            //if (command.Connection == null)
            //    command.Connection = this.Connection;
            //try
            //{
            //    this.ConnectionOpen();
            //    SqlDataReader dr = command.ExecuteReader();
            //    foreach (DataTable dt in ds.Tables)
            //    {
            //        dt.Load(dr);
            //        //dr.NextResult();//此行不需要，Load會自動next
            //    }
            //    dr.Close();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //finally
            //{
            //    this.ConnectionClose();
            //}
        }

        /// <summary>取得資料並透過Reflectionx回傳相對應的List<class>
        /// </summary>
        /// <param name="command">SqlCommand物件</param>
        /// <returns>DataTable</returns>
        public List<T> GetDataTableWithClass<T>(MySqlCommand command)
        {

            if (command.Connection == null)
                command.Connection = this.Connection;
            MySqlDataAdapter da = new MySqlDataAdapter(command);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ConnectionClose();
            }

            List<T> targetList = new List<T>();

            foreach (DataRow dr in dt.AsEnumerable())
            {
                T targetClass = Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PropertyInfo property = targetClass.GetType().GetProperty(dt.Columns[i].ToString());
                    property.SetValue(targetClass, dr[i] == System.DBNull.Value ? "" : dr[i], null);
                }
                targetList.Add(targetClass);
            }

            return targetList;


        }


        #endregion

        #region 交易 Transaction

        MySqlTransaction _transaction;
        /// <summary>取得Transaction物件
        /// </summary>
        /// 
        public MySqlTransaction Transaction
        {
            get
            {
                //不使用，原因是需要明確BeginTransaction，才會有Transaction，不應該自動建立
                //否則使用上會不明確
                //且外界可依 Transaction == null 來判別是否有用Transaction
                //if (_transaction == null)
                //    _transaction = this.BeginTransaction();
                return _transaction;
            }
        }

        /// <summary>開始交易，並傳回Transaction物件
        /// </summary>
        /// <returns>Transaction物件</returns>
        public MySqlTransaction BeginTransaction()
        {
            this.ConnectionOpen();
            this._transaction = this.Connection.BeginTransaction();
            return _transaction;
        }

        /// <summary>還原交易
        /// </summary>
        public void TransactionRollback()
        {
            if (_transaction != null)
                this._transaction.Rollback();
            this.ConnectionClose();
        }

        /// <summary>確認交易
        /// </summary>
        public void TransactionCommit()
        {
            if (_transaction != null)
                this._transaction.Commit();
            this.ConnectionClose();
        }
        #endregion

    }
}