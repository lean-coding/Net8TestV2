using BlazorTestV2.Database;
using BlazorTestV2.Model;
using Dapper;

namespace BlazorTestV2.Service
{
    public class Member
    {
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

                return false;
            }
        }

        /// <summary>
        /// 更新短網址
        /// </summary>
        /// <param name="mURL"></param>
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

    }
}
