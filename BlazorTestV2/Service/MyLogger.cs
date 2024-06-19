namespace BlazorTestV2.Service
{
    public class MyLogger : ILogger
    {
        IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

        private string logPath
        {
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
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //寫入 Console
            Console.WriteLine(formatter(state, exception));

            //寫入文字檔
            WriteTextLog<TState>(logLevel, eventId, state, exception);
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
    }
}