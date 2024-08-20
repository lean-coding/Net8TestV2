using Blazorise;
using BlazorTestV2.Database;
using BlazorTestV2.Service;
using Dapper;

namespace BlazorTestV2.Model
{
    /// <summary>
    /// EMail 發送佇列(資料表)
    /// </summary>
    public class Q_EMail
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int SN { get; set; }

        /// <summary>
        /// 發送對象
        /// </summary>
        public string EMailTo { get; set; }

        /// <summary>
        /// 副本發送對象
        /// </summary>
        public string EMailCC { get; set; }

        /// <summary>
        /// 信件標題
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 信件內容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 發送次數
        /// </summary>
        public int SendCount { get; set; }

        /// <summary>
        /// 已發送
        /// </summary>
        public string IsSend { get; set; }

        /// <summary>
        /// 發送日期
        /// </summary>
        public DateTime SendDate { get; set; }

        /// <summary>
        /// 發送人
        /// </summary>
        public string Sender { get; set; }


        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        public bool Update()
        {
            try
            {
                string sql = @"
UPDATE Q_EMail
SET EMailTo = @EMailTo,
    EMailCC = @EMailCC,
    Subject = @Subject,
    Body = @Body,
    SendCount = @SendCount,
    IsSend = @IsSend,
    SendDate = @SendDate,
    Sender = @Sender,
    CreateDate = @CreateDate,
WHERE SN = @SN; ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    conn.Execute(sql, this);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
