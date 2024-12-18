using BlazorTestV2.Database;
using BlazorTestV2.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Reflection.Metadata;

namespace BlazorTestV2.Service
{
    public class ShortURL
    {
        /// <summary>
        /// 新增短網址
        /// </summary>
        /// <param name="parameter">參數</param>
        /// <returns></returns>
        public bool Create(ref M_URL mURL)
        {
            try
            {
                string sql = @"
INSERT INTO M_URL 
(
    URL,
    ShortCode
) 
VALUES 
(
    @URL,
    @ShortCode
);
        
SELECT last_insert_rowid(); ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    mURL.SN = conn.QuerySingle<int>(sql, mURL);
                    if (mURL.ShortCode == null)
                    {
                        mURL.ShortCode = GetShortCode(mURL.SN);
                        Update(mURL);
                    }
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
        public bool Update(M_URL mURL)
        {
            try
            {
                string sql = @"
UPDATE M_URL
SET URL = @URL,
      ShortCode = @ShortCode
WHERE SN = @SN; ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    conn.Execute(sql, mURL);
                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public M_URL GetByShortCode(string ShortCode)
        {
            try
            {
                string sql = @"
SELECT * 
FROM M_URL
WHERE ShortCode = @ShortCode; ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    var mURL = conn.QueryFirstOrDefault<M_URL>(sql, new { ShortCode = ShortCode });
                    if (mURL != null)
                        return mURL;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 以 ShortCode 反查原始網址
        /// </summary>
        /// <param name="ShortCode"></param>
        /// <returns></returns>
        public string GetURLbyShortCode(string ShortCode)
        {
            try
            {
                //M_URL mURL = new();
                string sql = @"
SELECT * 
FROM M_URL
WHERE ShortCode = @ShortCode; ";

                using (var conn = SQLiteHelper.dbConnection())
                {
                    var mURL = conn.QueryFirstOrDefault<M_URL>(sql, new { ShortCode= ShortCode });
                    if (mURL != null)
                        return mURL.URL;
                    else
                        return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string GetShortCode(int SN)
        {
            return "S" + SN.ToString("D6"); //Sample S000002
        }

    }
}
