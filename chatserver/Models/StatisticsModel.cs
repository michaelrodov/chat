using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace chatserver.Models
{
    public class StatisticsModel
    {

        private dbEntities db = new dbEntities();

        private static String MESSAGE_PER_HOUR =
            "SELECT CONVERT(bigint, COUNT(*)) AS V, CONVERT(bigint, DATEDIFF_BIG(hh, GETUTCDATE(), [SENDTIME])) AS K " 
            + " FROM chattar.dbo.MESSAGES "
            + " GROUP BY DATEDIFF_BIG(hh, GETUTCDATE(), [SENDTIME])"
            + " HAVING DATEDIFF_BIG(hh, GETUTCDATE(), [SENDTIME]) < @hours";

        private static String WORDS_PER_HOUR =
            "SELECT CONVERT(bigint, SUM([WPH].[WORDS])) AS V, CONVERT(bigint, [WPH].[HOUR]) AS K "
            + " FROM ( "
                + " SELECT (SELECT COUNT(*) FROM STRING_SPLIT(MSG.[TEXT], ' ')) AS [WORDS], DATEDIFF_BIG(hh,GETUTCDATE(),MSG.[SENDTIME]) AS [HOUR] "
                + " FROM MESSAGES AS MSG) AS [WPH] "
                + " GROUP BY [WPH].[HOUR]"
                + " HAVING [WPH].[HOUR] < @hours";

        private static String LETTERS_AVG = "SELECT CONVERT(bigint,ISNULL(AVG(LEN(REPLACE([TEXT],' ',''))),0)) FROM [MESSAGES] ";

        //private static String MESSAGE_PER_HOUR = "SELECT '1' AS k, '2' AS v FROM chattar.dbo.MESSAGES GROUP BY DATEDIFF_BIG(hh, '1970-01-01 00:00:00', [SENDTIME]);";

        public long getAvgLetters(String username)
        {
            try
            {
                if(String.IsNullOrEmpty(username)){
                    return db.Database.SqlQuery<long>(LETTERS_AVG).Single<long>();
                }else{
                    SqlParameter userParam = new SqlParameter("@userParam", username);
                    return db.Database.SqlQuery<long>(LETTERS_AVG+" WHERE [FROM] = @userParam", userParam).Single<long>();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<NumericChartOutput> getWordsPerHour(long hours)
        {

            try
            {
                SqlParameter hoursParam = new SqlParameter("@hours", hours);
                var res = db.Database.SqlQuery<NumericChartOutput>(WORDS_PER_HOUR, hoursParam).ToList();

                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<NumericChartOutput> getMessagesPerHour(long hours)
        {
            
            try
            {
                SqlParameter hoursParam = new SqlParameter("@hours", hours);
                var res = db.Database.SqlQuery<NumericChartOutput>(MESSAGE_PER_HOUR, hoursParam).ToList();
                
                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}