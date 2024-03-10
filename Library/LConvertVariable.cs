﻿using pbms_be.Configurations;
using static Google.Cloud.DocumentAI.V1.Document.Types.Provenance.Types;

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

        public static string ConvertMinusTimeNowMonthString(DateTime time)
        {
            var now = DateTime.Now;
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
            return minusTimeNowString;
        }

        // convert utc time to local time
        public static DateTime ConvertUtcToLocalTime(DateTime utcTime)
        {
            return utcTime.AddHours(ConstantConfig.VN_TIMEZONE_UTC);
        }

        // convert local time to utc time
        public static DateTime ConvertLocalToUtcTime(DateTime localTime)
        {
            var utcdatetime = new DateTime(localTime.Year, localTime.Month, localTime.Day, localTime.Hour, localTime.Minute, localTime.Second, DateTimeKind.Utc);
            return utcdatetime.AddHours(-ConstantConfig.VN_TIMEZONE_UTC);
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

        public static string ConvertDateTimeToStringCustom(DateTime time, string format)
        {
            if (format == ConstantConfig.DEFAULT_DATE_FORMAT_SHORT)
            {
                return time.ToString("dd") + " thg " + time.ToString("MM");
            }
            return time.ToString(format);
        }


        // convert float of percent progress to string, làm tròn and add % at the end
        public static string ConvertPercentToString(float percent, int digits)
        {
            var result = Math.Round(percent, digits).ToString() + "%";
            return result;
        }

        // CalculatePercentProgress
        public static double CalculatePercentProgress(long currentAmount, long targetAmount, int digits)
        {
            if (targetAmount == 0)
                return 0;
            var per = (float)currentAmount / targetAmount * 100;
            return Math.Round(per, digits); ;
        }

        public static string GenerateRandomString(int length, string input)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringToUse = string.IsNullOrEmpty(input) ? chars : input + chars;
            var random = new Random();
            var result = new string(Enumerable.Repeat(stringToUse, length)
                             .Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }

        // get filename execpt file extension
        public static string GetFileNameWithoutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

        // i have year, month, day => return datetime with default time 00:00:00
        public static DateTime GetDateTimeFromYearMonthDay(int year, int month, int day)
        {
            //return datetime with default time 00:00:00
            return new DateTime(year, month, day);
        }

        // Convert day in week to vietnamese
        public static string ConvertDayInWeekToVN_SHORT_3(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return Message.VN_MONDAY_SHORT_3;
                case DayOfWeek.Tuesday:
                    return Message.VN_TUESDAY_SHORT_3;
                case DayOfWeek.Wednesday:
                    return Message.VN_WEDNESDAY_SHORT_3;
                case DayOfWeek.Thursday:
                    return Message.VN_THURSDAY_SHORT_3;
                case DayOfWeek.Friday:
                    return Message.VN_FRIDAY_SHORT_3;
                case DayOfWeek.Saturday:
                    return Message.VN_SATURDAY_SHORT_3;
                case DayOfWeek.Sunday:
                    return Message.VN_SUNDAY_SHORT_3;
                default:
                    return string.Empty;
            }
        }
        public static string ConvertDayInWeekToVN_FULL(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return Message.VN_MONDAY;
                case DayOfWeek.Tuesday:
                    return Message.VN_TUESDAY;
                case DayOfWeek.Wednesday:
                    return Message.VN_WEDNESDAY;
                case DayOfWeek.Thursday:
                    return Message.VN_THURSDAY;
                case DayOfWeek.Friday:
                    return Message.VN_FRIDAY;
                case DayOfWeek.Saturday:
                    return Message.VN_SATURDAY;
                case DayOfWeek.Sunday:
                    return Message.VN_SUNDAY;
                default:
                    return string.Empty;
            }
        }

        // convert dateonly to short string like DayOfWeek, "ngày" dd (+ " tháng" mm if day is 1,2,3, 28, 29, 30, 31)
        public static string ConvertDateOnlyToVN_ngay_thang(DateOnly date)
        {
            var dayInWeek = ConvertDayInWeekToVN_SHORT_3(date.DayOfWeek);
            var day = date.Day;
            var month = date.Month;
            var result = dayInWeek + ", " + Message.VN_DAY + " " + day;
            if (day == 1 || day == 2 || day == 3 || day == 28 || day == 29 || day == 30 || day == 31)
                result += " " + Message.VN_MONTH + " " + month;
            return result;
        }        
        public static string ConvertDateOnlyToVN_ng_thg(DateOnly date)
        {
            var dayInWeek = ConvertDayInWeekToVN_SHORT_3(date.DayOfWeek);
            var day = date.Day;
            var month = date.Month;
            var result = dayInWeek + ", " + Message.VN_DAY_SHORT + " " + day;
            if (day == 1 || day == 2 || day == 3 || day == 28 || day == 29 || day == 30 || day == 31)
                result += " " + Message.VN_MONTH_SHORT + " " + month;
            return result;
        }




    }
}
