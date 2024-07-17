using System.Data;

namespace BlazorTestV2.Database
{
    public class Startup
    {
        public static string CreateDatabase()
        {
            try
            {
                #region Create Database "ShortURL"
                SQLiteHelper.CreateDBFile("ShortURL.sqlite");
                return $" Database [ShortURL] has been created.";
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string CreateTable(string TableName)
        {
            string returnMsg = "";
            try
            {
                #region Script of Create
                string sql = "";

                if (TableName == "M_URL")
                {
                    #region Create Table M_URL
                    sql = @"
CREATE TABLE IF NOT EXISTS M_URL  (
    SN INTEGER PRIMARY KEY AUTOINCREMENT,
    URL NVARCHAR(300),
    ShortCode NVARCHAR(20) ); ";
                    #endregion
                }
                else if (TableName == "L_EventLog")
                {
                    #region Create Table L_EventLog
                    sql = @"
CREATE TABLE IF NOT EXISTS L_EventLog  (
    SN INTEGER PRIMARY KEY AUTOINCREMENT,
    EventId VARCHAR(50),
    LogLevel  VARCHAR(50),
    ClassName  VARCHAR(50),
    LogContent NVARCHAR(1000),
    LogDate Datetime ); ";
                    #endregion
                }
                else if (TableName == "S_Member")
                {
                    #region Create Table S_Member
                    sql = @"
CREATE TABLE IF NOT EXISTS S_Member  (
    SN INTEGER PRIMARY KEY AUTOINCREMENT,
    Nickname NVARCHAR(50),
    EMail  NVARCHAR(100),
    Password  VARCHAR(50),
    Status VARCHAR(10),
    Enable VARCHAR(1),
    CreateDate Datetime ); ";
                    #endregion
                }
                #endregion

                // Create Table
                SQLiteHelper.CreateTable(sql);
                returnMsg += $" Table [{TableName}] has been created.";
                return returnMsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static string GetDatatableStatus(string TableName)
        {
            try
            {
                string sql = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{TableName}' ";
                DataTable dtTable = SQLiteHelper.SqlTable(sql);

                if (dtTable != null && dtTable.Rows.Count > 0)
                    return "Created";
                else
                    return "Not Exists";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetDatatableCount(string TableName)
        {
            try
            {
                string sql = $"SELECT Count(1) as RowCount FROM {TableName} ";
                DataTable dtTable = SQLiteHelper.SqlTable(sql);

                if (dtTable != null && dtTable.Rows.Count > 0)
                    return dtTable.Rows[0]["RowCount"].ToString();
                else
                    return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
