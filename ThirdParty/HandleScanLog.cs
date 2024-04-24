using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Custom;
using pbms_be.Data.LogModel;
using System.Globalization;

namespace pbms_be.ThirdParty
{
    public class HandleScanLog(PbmsDbContext context)
    {
        private readonly PbmsDbContext _context = context;

        // a function to handle scan log
        public ScanLog CreateScanLog(string accountID, string scanType, string scanResult)
        {
            try
            {
                // create a new instance of scan log
                ScanLog scanLog = new()
                {
                    AccountID = accountID,
                    ScanType = scanType,
                    ScanResult = scanResult,
                    ScanTime = DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC).ToUniversalTime()
                };
                // add the scan log to the database
                _context.ScanLogs.Add(scanLog);
                // save the changes
                _context.SaveChanges();
                // return the scan log
                return scanLog;
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetLastNumbersDaysLog(int day)
        {
            try
            {
                var scanLogs = _context.ScanLogs
                    .Where(s => s.ScanTime.Date >= DateTime.UtcNow.AddDays(day * -1).Date)
                    .Include(s => s.Account)
                    .ToList();

                var totalLog = scanLogs.Count;
                var countID = 0;

                // group by each day, then count the number of logs of each day, using LogWithDayDetail
                var scanLogByDay = scanLogs
                    .GroupBy(s => s.ScanTime.Date)
                    .ToDictionary(g => g.Key, g => new LogWithDayDetail
                    {
                        Date = new DateOnly(g.Key.Year, g.Key.Month, g.Key.Day),
                        NumberOfLogs = g.Count(),
                        ID = countID++,
                        ScanLogs = [.. g]
                    });

                // sort by Date
                scanLogByDay = scanLogByDay
                    .OrderByDescending(s => s.Key)
                    .ToDictionary(s => s.Key, s => s.Value);

                return new { 
                    totalLog, 
                    List = scanLogByDay.Values.ToList()
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetScanLogs(string date)
        {
            try
            {
                DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToUniversalTime();

                var scanLogs = _context.ScanLogs
                    .Where(s => s.ScanTime.Date.Date == dateTime.Date.Date)
                    .Include(s => s.Account)
                    .ToList();

                var totalLog = scanLogs.Count;
                var countID = 0;
                var scanLogByAccountID = scanLogs
                    .GroupBy(s => s.AccountID)
                    .ToDictionary(g => g.Key, g => new LogWithDetailAccount
                    {
                        AccountID = g.Key,
                        Account = _context.Account.FirstOrDefault(a => a.AccountID == g.Key),
                        ID = countID++, 
                        NumberOfLogs = g.Count(),
                        ScanLogs = [.. g]
                    });
                // sort by NumberOfLogs
                scanLogByAccountID = scanLogByAccountID
                    .OrderByDescending(s => s.Value.NumberOfLogs)
                    .ToDictionary(s => s.Key, s => s.Value);

                return new { totalLog, List = scanLogByAccountID.Values.ToList() };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetScanLogs()
        {
            try
            {
                var scanLogs = _context.ScanLogs
                    .Include(s => s.Account)
                    .ToList();

                var totalLog = scanLogs.Count;
                var countID = 0;

                var scanLogByAccountID = scanLogs
                    .GroupBy(s => s.AccountID)
                    .ToDictionary(g => g.Key, g => new LogWithDetailAccount
                    {
                        AccountID = g.Key,
                        Account = _context.Account.FirstOrDefault(a => a.AccountID == g.Key),
                        ID = countID++, 
                        NumberOfLogs = g.Count(),
                        ScanLogs = [.. g]
                    });
                // sort by NumberOfLogs
                scanLogByAccountID = scanLogByAccountID
                    .OrderByDescending(s => s.Value.NumberOfLogs)
                    .ToDictionary(s => s.Key, s => s.Value);

                return new { totalLog, List = scanLogByAccountID.Values.ToList() };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetScanLogsAllByDate(string date)
        {
            try
            {
                DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToUniversalTime();

                var scanLogs = _context.ScanLogs
                    .Where(s => s.ScanTime.Date.Date == dateTime.Date.Date)
                    .Include(s => s.Account)
                    .ToList();

                var totalLog = scanLogs.Count;
                //var countID = 0;
                //var scanLogByAccountID = scanLogs
                //    .GroupBy(s => s.AccountID)
                //    .ToDictionary(g => g.Key, g => new LogWithDetailAccount
                //    {
                //        AccountID = g.Key,
                //        Account = _context.Account.FirstOrDefault(a => a.AccountID == g.Key),
                //        ID = countID++,
                //        NumberOfLogs = g.Count(),
                //        ScanLogs = [.. g]
                //    });
                //// sort by NumberOfLogs
                //scanLogByAccountID = scanLogByAccountID
                //    .OrderByDescending(s => s.Value.NumberOfLogs)
                //    .ToDictionary(s => s.Key, s => s.Value);

                return new { 
                    totalLog,
                    //List = scanLogByAccountID.Values.ToList() 
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
