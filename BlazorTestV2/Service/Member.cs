using BlazorTestV2.Database;
using BlazorTestV2.Model;
using Dapper;
using System.Transactions;

namespace BlazorTestV2.Service
{
    public class Member
    {
        readonly MyLogger logger;

        public Member()
        {
            logger = new Service.MyLogger();
        }

        /*public Member(ILogger<Member> Logger) 
        { 
            logger = Logger;
        }*/

        /// <summary>
        /// 新增 S_Member
        /// </summary>
        /// <param name="sMember"></param>
        /// <returns></returns>
        public bool Create(ref S_Member sMember)
        {
            try
            {
                string sql = @"
INSERT INTO S_Member 
(
    Nickname,
    EMail,
    Password,
    Status,
    CreateDate
) 
VALUES 
(
    @Nickname,
    @EMail,
    @Password,
    @Status,
    datetime('now')
);
        
SELECT last_insert_rowid(); ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    sMember.SN = conn.QuerySingle<int>(sql, sMember);
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.ToString(), "Create");
                return false;
            }
        }

        /// <summary>
        /// 更新 S_Member
        /// </summary>
        /// <param name="sMember"></param>
        /// <returns></returns>
        public bool Update(S_Member sMember)
        {
            try
            {
                string sql = @"
UPDATE S_Member
SET Nickname = @Nickname,
    EMail = @EMail,
    Password = @Password,
    Status = @Status
WHERE SN = @SN; ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    conn.Execute(sql, sMember);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 以 EMail 取得 S_Member
        /// </summary>
        /// <param name="EMail"></param>
        /// <returns></returns>
        public S_Member Get(string EMail)
        {
            S_Member member = null;
            try
            {
                string sql = @"
SELECT * FROM S_Member
WHERE EMail = @EMail; ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    member = conn.QueryFirst<S_Member>(sql, new { EMail = EMail });
                }
                return member;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool CheckEMailHasRegistered(string EMail)
        {
            try
            {
                string sql = @"
SELECT * FROM S_Member
WHERE EMail = @EMail; ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    var results = conn.QueryFirst<S_Member>(sql, new { EMail = EMail });
                    return (results != null);
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<S_Member> CheckLogonMember(string EMail, string Password)
        {
            string ErrorMsg = "";
            S_Member member;
            try
            {
                string sql = @"
SELECT * 
FROM S_Member
WHERE EMail = @EMail AND Password = @Password ; ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    member = await conn.QuerySingleOrDefaultAsync<S_Member>(sql, new S_Member()
                    {
                        EMail = EMail,
                        Password = Password
                    });
                    if (member != null)
                        return member;
                }
            }
            catch (Exception ex)
            {
                //Log exception later
            }
            return null;
        }

        /// <summary>
        /// 發送驗證信
        /// </summary>
        /// <param name="EMail"></param>
        /// <returns></returns>
        public async Task<L_Validation> SendValidationEMail(string EMail)
        {
            string Prefix = "";
            try
            {
                #region 確認 EMail 是否正確
                S_Member member = this.Get(EMail);
                if (member == null)
                    return null;
                #endregion

                #region 在 Transaction 中記錄 L_Validation 並 發送 EMail

                #region 取得 Totp
                MyTotp totp = new MyTotp();
                string code = totp.GetTotp(EMail, 60 * 10); //10分鐘內有效
                Guid guid = Guid.NewGuid();
                string prefix = guid.ToString().Substring(0,4);
                #endregion

                #region 寫入 L_Validation
                L_Validation validation = new L_Validation()
                {
                    MemberSN = member.SN,
                    Method = "F",
                    TOTP = code,
                    Prefix = prefix,
                    IsValidate = "N",
                    CreateDate = DateTime.Now
                };

                string sql = @"
INSERT INTO L_Validation
(MemberSN, Method, Prefix, TOTP, IsValidate, CreateDate)
VALUES(@MemberSN, @Method, @Prefix, @TOTP, @IsValidate, @CreateDate); 

SELECT last_insert_rowid();
            ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    validation.SN = conn.QuerySingle<int>(sql, validation);
                    conn.Close();
                }

                #endregion

                #region 發送 EMail
                Q_EMail mail = new Q_EMail()
                {
                    EMailTo = member.EMail,
                    EMailCC = "",
                    Subject = "One time code at ShortURL",
                    Body = $"Your one time code is {code}. Please validate betwen 10 minute.",
                    SendCount = 0,
                    IsSend = "N",
                    Sender = member.SN.ToString(),
                    CreateDate = DateTime.Now
                };

                try
                {
                    MySmtp smtp = new MySmtp();
                    smtp.Send(mail.EMailTo, mail.EMailTo, mail.Subject, mail.Body);
                    mail.SendCount++;
                    mail.IsSend = "Y";
                    mail.SendDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Smtp send fail.")
                    {
                        mail.IsSend = "N";
                        mail.SendCount++;
                    }
                    throw ex;
                }
                #endregion

                #region 寫入 Q_EMail
                string sql2 = @"
INSERT INTO Q_EMail
(EMailTo, EMailCC, Subject, Body, SendCount, IsSend, SendDate, Sender, CreateDate)
VALUES(@EMailTo, @EMailCC, @Subject, @Body, @SendCount, @IsSend, @SendDate, @Sender, @CreateDate);

SELECT last_insert_rowid();
            ";

                using (var conn2 = SQLiteHelper.dbConnection())
                {
                    mail.SN = conn2.QuerySingle<int>(sql2, mail);
                    conn2.Close();
                }

                #endregion
                //scope.Complete(); //Or it will automatically rollback 
                return validation;
                #endregion
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EMail"></param>
        /// <param name="Validation"></param>
        /// <param name="ValidationCode"></param>
        /// <returns></returns>
        public async Task<bool> CheckValidationCode(string EMail, L_Validation Validation, string ValidationCode)
        {
            S_Member member = this.Get(EMail);
            if (member != null && member.EMail == EMail && Validation.TOTP == ValidationCode)
                return true;
            else
                return false;
        }

        public async Task<bool> ResetPassword(string EMail, string Password)
        {
            S_Member member = this.Get(EMail);
            member.Password = Password;
            try
            {
                string sql = @"
UPDATE S_Member 
SET Password = @Password
WHERE SN = @SN; 
                ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    var affectedRows = await conn.ExecuteAsync(sql, member);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

    }
}
