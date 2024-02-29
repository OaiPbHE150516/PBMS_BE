using pbms_be.Configurations;

namespace pbms_be.Library
{
    public class LConvertVariable
    {
        public static string ConvertMinusTimeNowString(DateTime time)
        {
            /*
             * example: 1 minute ago, 1 hour and 30 minutes ago
             * if now - time < 1 minute => return "just now"
             * if now - time < 1 hour => return "x minutes ago"
             * if now - time < 1 day => return "x hours ago"
             * if now - time < 1 month => return "x days ago"
             * default: return "x months ago"
             */

            var now = DateTime.UtcNow;
            var minusTime = now - time;
            var minusTimeNowString = string.Empty;
            if (minusTime.TotalMinutes < ConstantConfig.MIN_MINUTES_AGO)
                minusTimeNowString = Message.VN_JUST_NOW;
            else if (minusTime.TotalMinutes < ConstantConfig.MIN_HOURS_AGO)
                minusTimeNowString = minusTime.Minutes + Message.VN_MINUTES_AGO;
            else if (minusTime.TotalHours < ConstantConfig.MIN_DAYS_AGO)
                minusTimeNowString = minusTime.Hours + Message.VN_HOURS_AGO;
            else if (minusTime.TotalDays < ConstantConfig.MIN_MONTHS_AGO)
                minusTimeNowString = minusTime.Days + Message.VN_DAYS_AGO;
            else
                minusTimeNowString = minusTime.Days / ConstantConfig.MIN_MONTHS_AGO + Message.VN_MONTHS_AGO;
            return minusTimeNowString;
        }

        // convert utc time to local time
        public static DateTime ConvertUtcToLocalTime(DateTime utcTime)
        {
            return utcTime.AddHours(ConstantConfig.VN_TIMEZONE_UTC);
        }

        public static string ConvertToMoneyFormat(long number)
        {
            var result = number.ToString("N0");
            result = result.Replace(",", ".") + " đ";
            return result;
        }

        // convert datetime to string like "HH:mm, dd/MM/yyyy"
        public static string ConvertDateTimeToString(DateTime time)
        {
            return time.ToString(ConstantConfig.DEFAULT_DATETIME_FORMAT);
        }
    }
}
