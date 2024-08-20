using Blazorise;
using OtpNet;
using System.Text;

namespace BlazorTestV2.Service
{
    public class MyTotp
    {
        /// <summary>
        /// 取得 TOTP
        /// </summary>
        /// <param name="SecretKey">金鑰</param>
        /// <param name="Period">效期(以秒計)</param>
        /// <returns></returns>
        public string GetTotp(string SecretKey, int Period)
        {
            string code = "";
            try
            {
                byte[] secret = Encoding.UTF8.GetBytes(SecretKey);
                Totp totp = new Totp(secret, Period);
                code = totp.ComputeTotp();
            }
            catch (Exception ex) { }
            return code;
        }

        /// <summary>
        /// 驗證 TOTP
        /// </summary>
        /// <param name="SecretKey">金鑰</param>
        /// <param name="TotpCode">TOTP</param>
        /// <param name="TimeUsed">持續時間(以秒計)</param>
        /// <returns></returns>

        public bool Check(string SecretKey, string TotpCode, out long TimeUsed)
        {
            byte[] secret = Encoding.UTF8.GetBytes(SecretKey);
            Totp totp = new Totp(secret);
            return totp.VerifyTotp(TotpCode, out TimeUsed);
        }
    }
}
