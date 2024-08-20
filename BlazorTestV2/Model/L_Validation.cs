namespace BlazorTestV2.Model
{
    /// <summary>
    /// 驗證記錄(資料表)
    /// </summary>
    public class L_Validation
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int SN { get; set; }

        /// <summary>
        /// 會員代碼
        /// </summary>
        public int MemberSN { get; set; }

        /// <summary>
        /// 驗證方式
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 提示碼
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string TOTP { get; set; }

        /// <summary>
        /// 完成驗證
        /// </summary>
        public string IsValidate { get; set; }


        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateDate { get; set; }

    }
}
