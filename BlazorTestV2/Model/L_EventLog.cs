namespace BlazorTestV2.Model
{
    public class L_EventLog
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int SN { get; set; }

        /// <summary>
        /// 事件ID
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// 事件層級
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        /// 類別名稱
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 事件內容
        /// </summary>
        public string LogContent { get; set; }


        /// <summary>
        /// 事件時間
        /// </summary>
        public DateTime LogDate { get; set; }

    }
}
