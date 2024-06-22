using BlazorTestV2.Database;
using BlazorTestV2.Model;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace BlazorTestV2.Service
{
    public sealed class MyLogger : ILogger
    {
        IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
        private string logPath {
            get
            {
                string path = "";
                if (config != null && config.GetValue<string>("Logging:MyLoggerProvider:LogPath") != null)
                    return config.GetValue<string>("Logging:MyLoggerProvider:LogPath");
                else //預設路徑
                {
                    path = Environment.CurrentDirectory + @"\\Log\\";
                    return path;
                }
                    
            }
        }
        public IDisposable BeginScope<TState>(TState State)
        {
            return null;
        }

        public bool IsEnabled(LogLevel LogLevel)
        {
            LogLevel targetLevel = config.GetValue<LogLevel>("Logging:MyLoggerProvider:MinimumLogLevel");
            return (LogLevel.CompareTo(targetLevel) >= 0);
        }

        public void Log<TState>(LogLevel LogLevel, EventId EventId, TState State, Exception Exception, Func<TState, Exception, string> Formatter)
        {
           if (IsEnabled(LogLevel))
           {
                //寫入 Console
                //Console.WriteLine(formatter(state, exception));

                //寫入文字檔
                WriteTextLog<TState>(LogLevel, EventId, State, Exception);

                //寫入資料表
                WriteDBLog<TState>(LogLevel, EventId, State, Exception);
            }
        }

        private void WriteTextLog<TState>(LogLevel LogLevel, EventId EventId, TState State, Exception Exception)
        {
            //寫入文字 DateTime---->LogLevel<EventId>: TState; Exp: Exception
            string msg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + $"---->{LogLevel}<{EventId}>";
            if (State != null)
                msg += $": {State};";
            if (Exception != null)
                msg += $" Exp: {Exception};";

            WriteTextLog(msg);
        }

        private void WriteTextLog(string LogMsg)
        {
            //Log檔案名稱: 日期_log.txt
            string logFileName = DateTime.Now.ToString("yyyyMMdd") + "_log.txt";
            string logFileFullPath = Path.Combine(logPath, logFileName);

            if (!Directory.Exists(logPath))
            {
                //建立資料夾
                Directory.CreateDirectory(logPath);
            }

            if (!File.Exists(logFileFullPath))
            {
                //建立檔案
                File.Create(logFileFullPath).Close();
            }

            using (StreamWriter sw = File.AppendText(logFileFullPath))
            {
                //WriteLine為換行 
                sw.WriteLine(LogMsg);
                sw.WriteLine("");
            }
        }

        private void WriteDBLog<TState>(LogLevel LogLevel, EventId EventId, TState State, Exception Exception)
        {
            string msg = "";
            if (State != null)
                msg += $": {State};";
            if (Exception != null)
                msg += $" Exp: {Exception};";

            L_EventLog log = new()
            {
                LogLevel = LogLevel.ToString(),
                EventId = EventId.ToString(),
                ClassName = "",
                LogContent = msg
            };

            try
            {
                string sql = @"
INSERT INTO L_EventLog 
(
    EventId, 
    LogLevel, 
    ClassName, 
    LogContent, 
    LogDate
)
VALUES
( 
    @EventId,
    @LogLevel,
    @ClassName,
    @LogContent,
    DATETIME('NOW') 
);
        
SELECT last_insert_rowid(); ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    log.SN = conn.QuerySingle<int>(sql, log);
                }
            }
            catch (Exception ex) { }
        }

    }

    public sealed class MyLoggerConfiguration
    {
        LogLevel MinimumLogLevel { get; set; }
        string LogPath { get; set; }
    }
}
