using Blazorise;

namespace BlazorTestV2.Model
{
    public class S_Member
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int SN { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// EMail
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 狀態(A=Active, D=Dormant, S=Stop )
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime CreateDate { get; set; }

    }
}
