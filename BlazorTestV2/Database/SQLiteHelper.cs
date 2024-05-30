using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Configuration;

namespace BlazorTestV2.Database
{
    public class SQLiteHelper
    {
        /// <summary>
        /// 建立 SQLite 資料庫實體檔案
        /// </summary>
        /// <param name="fileName">文件名</param>
        public static void CreateDBFile(string fileName)
        {
            string path = Environment.CurrentDirectory + @"/Database/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string databaseFileName = path + fileName;
            if (!File.Exists(databaseFileName))
            {
                SQLiteConnection.CreateFile(databaseFileName);
            }
        }
        /// <summary>
        /// 删除資料庫
        /// </summary>
        /// <param name="fileName">文件名</param>
        public static void DeleteDBFile(string fileName)
        {
            string path = Environment.CurrentDirectory + @"/Data/";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        /// <summary>
        /// 生成資料庫連接字串(讀取 appsettings.json)
        /// </summary>
        /// <returns></returns>
        private static string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder();
            connectionString.DataSource = config.GetRequiredSection("ConnectionString").Value;
            string conStr = connectionString.ToString();
            return conStr;
        }
        private static SQLiteConnection m_dbConnection;
        /// <summary>
        /// 事务
        /// </summary>
        public static SQLiteTransaction trans = null;
        #region 事务控制
        /// <summary>
        /// 开始事务
        /// </summary>
        public static void BeginTransaction()
        {
            trans = m_dbConnection.BeginTransaction(IsolationLevel.ReadCommitted);
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public static void Rollback()
        {
            trans.Rollback();
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public static void Commit()
        {
            trans.Commit();
        }
        #endregion
        /// <summary>
        /// 连接到数据库
        /// </summary>
        /// <returns></returns>
        public static SQLiteConnection dbConnection()
        {
            m_dbConnection = new SQLiteConnection(GetConnectionString());
            m_dbConnection.Open();
            return m_dbConnection;
        }

        /// <summary>
        /// 在指定数据库中创建一个table
        /// </summary>
        /// <param name="sql">sql语言，如：create table pat_info (name varchar(20), score int)</param>
        /// <returns></returns>
        public static bool CreateTable(string sql)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, dbConnection());
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                //_ErrorLog.Insert("ExecuteNonQuery(" + sql + ")Err:" + ex);
                return false;
            }
            finally
            {
                closeConn();
            }
        }
        /// <summary>
        /// 在指定数据库中删除一个table
        /// </summary>
        /// <param name="tablename">表名称</param>
        /// <returns></returns>
        public static bool DeleteTable(string tablename)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand("DROP TABLE IF EXISTS " + tablename, dbConnection());
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                //_ErrorLog.Insert("ExecuteNonQuery(DROP TABLE IF EXISTS " + tablename + ")Err:" + ex);
                return false;
            }
            finally
            {
                closeConn();
            }
        }
        /// <summary>
        /// 在指定表中添加列
        /// </summary>
        /// <param name="tablename">表名</param>
        /// <param name="columnname">列名</param>
        /// <param name="ctype">列的数值类型</param>
        /// <returns></returns>
        public static bool AddColumn(string tablename, string columnname, string ctype)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand("ALTER TABLE " + tablename + " ADD COLUMN " + columnname + " " + ctype, dbConnection());
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                //_ErrorLog.Insert("ExecuteNonQuery(ALTER TABLE " + tablename + " ADD COLUMN " + columnname + " " + ctype + ")Err:" + ex);
                return false;
            }
            finally
            {
                closeConn();
            }
        }
        /// <summary>
        /// 执行增删改查操作
        /// </summary>
        /// <param name="sql">查询语言</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql)
        {
            try
            {
                SQLiteCommand cmd;
                cmd = new SQLiteCommand(sql, dbConnection());
                cmd.ExecuteNonQuery().ToString();
                return 1;
            }
            catch (Exception ex)
            {
                //_ErrorLog.Insert("ExecuteNonQuery(" + sql + ")Err:" + ex);
                return 0;
            }
            finally
            {
                closeConn();
            }
        }
        /// <summary>
        /// <para>执行SQL,返回受影响的行数</para>
        /// <para>可用于执行表创建语句</para>
        /// <para>paramArr == null 表示无参数</para>
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, SQLiteParameter[] paramArr)
        {
            if (sql == null)
            {
                throw new ArgumentNullException("sql=null");
            }
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = sql;
            if (paramArr != null)
            {
                foreach (SQLiteParameter p in paramArr)
                {
                    cmd.Parameters.Add(p);
                }
            }
            cmd.Connection = m_dbConnection;
            int i = cmd.ExecuteNonQuery();
            return i;
        }
        /// <summary>
        /// 返回一条记录查询
        /// </summary>
        /// <param name="sql">sql查询语言</param>
        /// <returns>返回字符串数组</returns>
        public static string[] SqlRow(string sql)
        {
            try
            {
                SQLiteCommand sqlcmd = new SQLiteCommand(sql, dbConnection());//sql语句
                SQLiteDataReader reader = sqlcmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                string[] Row = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Row[i] = (reader[i].ToString());
                }
                reader.Close();
                return Row;
            }
            catch (Exception ex)
            {
                //_ErrorLog.Insert("SqlRow(" + sql + ")Err:" + ex);
                return null;
            }
            finally
            {
                closeConn();
            }
        }
        /// <summary>
        /// 唯一结果查询
        /// </summary>
        /// <param name="sql">sql查询语言</param>
        /// <returns>返回一个字符串</returns>
        public static string sqlone(string sql)
        {
            try
            {
                SQLiteCommand sqlcmd = new SQLiteCommand(sql, dbConnection());//sql语句
                return sqlcmd.ExecuteScalar().ToString();
            }
            catch
            {
                return "";
            }
            finally
            {
                closeConn();
            }
        }
        /// <summary>
        /// 获取一列数据
        /// </summary>
        /// <param name="sql">单列查询</param>
        /// <param name="count">返回结果数量</param>
        /// <returns>返回一个数组</returns>
        public static List<string> sqlcolumn(string sql)
        {
            try
            {
                List<string> Column = new List<string>();
                SQLiteCommand sqlcmd = new SQLiteCommand(sql, dbConnection());//sql语句
                SQLiteDataReader reader = sqlcmd.ExecuteReader();
                while (reader.Read())
                {
                    Column.Add(reader[0].ToString());
                }
                reader.Close();
                return Column;
            }
            catch (Exception ex)
            {
                //_ErrorLog.Insert("sqlcolumn(" + sql + ")Err:" + ex);
                return null;
            }
            finally
            {
                closeConn();
            }
        }
        /// <summary>
        /// 返回记录集查询
        /// </summary>
        /// <param name="sql">sql查询语言</param>
        /// <returns>返回查询结果集</returns>
        public static DataTable SqlTable(string sql)
        {
            try
            {
                SQLiteCommand sqlcmd = new SQLiteCommand(sql, dbConnection());//sql语句
                sqlcmd.CommandTimeout = 120;
                SQLiteDataReader reader = sqlcmd.ExecuteReader();
                DataTable dt = new DataTable();
                if (reader != null)
                {
                    dt.Load(reader, LoadOption.PreserveChanges, null);
                }
                return dt;
            }
            catch (Exception ex)
            {
                //_ErrorLog.Insert("SqlReader(" + sql + ")Err:" + ex);
                return null;
            }
            finally
            {
                closeConn();
            }
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public static void closeConn()
        {
            try
            {
                if (m_dbConnection.State == ConnectionState.Open)
                    m_dbConnection.Close();
                else if (m_dbConnection.State == ConnectionState.Broken)
                {
                    m_dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                //_ErrorLog.Insert("closeConnErr:" + ex);
            }
        }


    }
}
