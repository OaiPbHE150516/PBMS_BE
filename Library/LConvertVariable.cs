﻿using pbms_be.Configurations;
using pbms_be.Data.Custom;
using System.Globalization;

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

            var now = DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC);
            var minusTime = now - time;
            string? minusTimeNowString;
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
            RegionInfo vietnamRegion = new RegionInfo("vi-VN");
            result = result.Replace(",", ".") + " " + vietnamRegion.CurrencySymbol;
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

        // VN_MONDAY_SHORT_4
        public static string ConvertDayInWeekToVN_SHORT_4(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return Message.VN_MONDAY_SHORT_4;
                case DayOfWeek.Tuesday:
                    return Message.VN_TUESDAY_SHORT_4;
                case DayOfWeek.Wednesday:
                    return Message.VN_WEDNESDAY_SHORT_4;
                case DayOfWeek.Thursday:
                    return Message.VN_THURSDAY_SHORT_4;
                case DayOfWeek.Friday:
                    return Message.VN_FRIDAY_SHORT_4;
                case DayOfWeek.Saturday:
                    return Message.VN_SATURDAY_SHORT_4;
                case DayOfWeek.Sunday:
                    return Message.VN_SUNDAY_SHORT_4;
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
            var result = dayInWeek + ", " + day + " " + Message.VN_MONTH_SHORT + " " + month;
            //if (day == 1 || day == 2 || day == 3 || day == 28 || day == 29 || day == 30 || day == 31)
            //    result += " " + Message.VN_MONTH_SHORT + " " + month;
            return result;
        }

        internal static string ConvertDateTimeToDayOfWeekShort(DateTime dateTime)
        {
            return ConvertDayInWeekToVN_SHORT_3(dateTime.DayOfWeek);
        }

        internal static object ConvertDateTimeToDayOfWeekLong(DateTime dateTime)
        {
            return ConvertDayInWeekToVN_FULL(dateTime.DayOfWeek);
        }

        internal static object ConvertDateTimeToDayOfWeekMdl(DateTime dateTime)
        {
            return ConvertDayInWeekToVN_SHORT_4(dateTime.DayOfWeek);
        }

        internal static string ConvertDateToShortStr(DateTime dateTime)
        {
            var day = dateTime.ToString("dd");
            // remove 0 at the beginning of day and month if them < 10
            if (day[0] == '0') day = day.Remove(0, 1);
            var month = dateTime.ToString("MM");
            if (month[0] == '0') month = month.Remove(0, 1);
            return day + ConstantConfig.DEFAULT_DAY_THG_MONTH + month;
        }

        internal static DayDetail ConvertDateOnlyToDayDetail(DateOnly dateonly)
        {
            var dayDetail = new DayDetail
            {
                DayOfWeek = dateonly.DayOfWeek,
                Short_EN = dateonly.DayOfWeek.ToString()[..3],
                Full_EN = dateonly.DayOfWeek.ToString(),
                Short_VN = ConvertDayInWeekToVN_SHORT_3(dateonly.DayOfWeek),
                Full_VN = ConvertDayInWeekToVN_FULL(dateonly.DayOfWeek),
                ShortDate = ConvertDateOnlyToVN_ng_thg(dateonly),
                FullDate = ConvertDateOnlyToVN_ngay_thang(dateonly),
                DayStr = dateonly.Day.ToString(),
                MonthYearStr = $"tháng {dateonly.Month}, {dateonly.Year}"
            };
            return dayDetail;
        }

        //// get all week have inside from start date to end date
        //public static object GetAllWeeksFromStartDateToEndDate(DateOnly startDate, DateOnly endDate)
        //{
        //    var result = new Dictionary<string, WeeksInMonth>();
        //    // loop to Console.WriteLine all dayofweek from start date to end date
        //    var currentDate = startDate;
        //    var countWeek = 0;
        //    while (currentDate <= endDate)
        //    {
        //        Console.WriteLine(currentDate.ToString() + ": " + currentDate.DayOfWeek);

        //        if(currentDate.DayOfWeek is DayOfWeek.Sunday)
        //        {
        //            var weekNumber = currentDate.Day / 7 + 1;
        //            var week = new WeeksInMonth
        //            {
        //                WeekNumber = weekNumber,
        //                StartDate = currentDate.AddDays(-6),
        //                EndDate = currentDate
        //            };
        //            result.Add(weekNumber.ToString(), week);
        //        }

        //        currentDate = currentDate.AddDays(1);
        //        countWeek++;
        //    }
        //    return result;
        //}

        public static DateTime ConvertStringToDateTime(string timestring)
        {
            // return DateTime.ParseExact(time, ConstantConfig.DEFAULT_DATETIME_FORMAT, CultureInfo.InvariantCulture);
            // try to convert timestring to datetime utc kind, if fail return default datetime is utcnow
            try
            {
                // var format1 = "HH:mm, dd/MM/yyyy";
                // var format2 = "dd/MM/yyyy HH:mm";
                // var format3 = "dd/MM/yyyy";
                // var format4 = "dd-MM-yyyy";
                // var format5 = "yyyy-MM-dd";
                // var format6 = "yyyy-MM-dd HH:mm:ss";
                // var format7 = "yyyy-MM-ddTHH:mm:ss";
                // var format8 = "yyyy-MM-ddTHH:mm:ssZ";
                // var format9 = "yyyy-MM-ddTHH:mm:ss.fffZ";
                // var format10 = "yyyy-MM-ddTHH:mm:ss.fff";

                // // return DateTime.ParseExact(timestring, ConstantConfig.DEFAULT_DATETIME_FORMAT, CultureInfo.InvariantCulture);
                // if (DateTime.TryParseExact(timestring, ConstantConfig.DEFAULT_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
                // {
                //     return parsedDateTime;
                // } else if(DateTime.TryParseExact(timestring, ConstantConfig.DEFAULT_DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime)) {
                //     return parsedDateTime;
                // }
                // else
                // {
                //     // add 7 hours to convert from utc to local but still return utc kind
                //     return DateTime.ParseExact(timestring, ConstantConfig.DEFAULT_DATETIME_FORMAT, CultureInfo.InvariantCulture).AddHours(ConstantConfig.VN_TIMEZONE_UTC);
                // }

                // get hour, munite, day, month, year from string, if fail return default datetime is utcnow
                DateTime now = DateTime.Now;

                // Tách chuỗi thành các phần tử ngày, tháng, năm, giờ, phút
                string[] separators = ["/", "-", " ", ":"];
                string[] parts = timestring.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                int day = now.Day;
                int month = now.Month;
                int year = now.Year;
                int hour = now.Hour;
                int minute = now.Minute;

                // Thử chuyển đổi từng phần tử
                if (parts.Length >= 3)
                {
                    if (int.TryParse(parts[0], out int parsedDay))
                    {
                        day = parsedDay;
                    }
                    if (int.TryParse(parts[1], out int parsedMonth))
                    {
                        month = parsedMonth;
                    }
                    if (int.TryParse(parts[2], out int parsedYear))
                    {
                        year = parsedYear;
                    }
                }

                if (parts.Length >= 5)
                {
                    if (int.TryParse(parts[3], out int parsedHour))
                    {
                        hour = parsedHour;
                    }
                    if (int.TryParse(parts[4], out int parsedMinute))
                    {
                        minute = parsedMinute;
                    }
                }

                try
                {
                    // Thử tạo đối tượng DateTime từ các phần tử đã chuyển đổi
                    return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc).ToUniversalTime();
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Nếu không thể chuyển đổi, trả về DateTime hiện tại + vn hour kind utc
                    return DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC).ToUniversalTime();

                }
            }
            catch
            {
                return DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC).ToUniversalTime();
            }

        }
    }

    //public class WeeksInMonth
    //{
    //    public int WeekNumber { get; set; }
    //    public DateOnly StartDate { get; set; }
    //    public DateOnly EndDate { get; set; }
    //}
}
