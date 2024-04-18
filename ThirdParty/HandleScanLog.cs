using Microsoft.Data.SqlClient;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Log;

namespace pbms_be.ThirdParty
{
    public class HandleScanLog
    {
        private readonly PbmsDbContext _context;

        public HandleScanLog(PbmsDbContext context)
        {
            _context = context;
        }

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
    }
}
