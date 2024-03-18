using AutoMapper;
using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Balance;
using pbms_be.Data.Custom;
using pbms_be.DTOs;
using pbms_be.Library;

namespace pbms_be.DataAccess
{
    public class BalanceHisLogDA
    {
        private readonly PbmsDbContext _context;

        public BalanceHisLogDA(PbmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<BalanceHistoryLog>> GetBalanceHistoryLog(string accountID)
        {
            try
            {
                var result = await _context.BalanceHistoryLogs.Where(x => x.AccountID == accountID).ToListAsync();
                // check if result is null or empty or not found by accountID, then throw exception
                if (result == null || result.Count == 0)
                {
                    throw new Exception(Message.BALANCE_HISTORY_LOG_NOT_FOUND);
                }
                // sort by date
                result.Sort((x, y) => x.HisLogDate.CompareTo(y.HisLogDate));
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // create new balance history log
        public BalanceHistoryLog CreateBalanceHistoryLog(BalanceHistoryLog balanceHistoryLog)
        {
            try
            {
                //// check if balance history log is valid
                //if (!IsBalanceHistoryLogValid(balanceHistoryLog))
                //{
                //    throw new Exception(Message.BALANCE_HISTORY_LOG_INVALID);
                //}
                _context.BalanceHistoryLogs.Add(balanceHistoryLog);
                _context.SaveChanges();
                return balanceHistoryLog;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // check if balance history log is valid
        public bool IsBalanceHistoryLogValid(BalanceHistoryLog balanceHistoryLog)
        {
            try
            {
                var account = _context.Account.FirstOrDefaultAsync(x => x.AccountID == balanceHistoryLog.AccountID) ?? throw new Exception(Message.ACCOUNT_NOT_FOUND);
                var wallet = _context.Wallet.FirstOrDefaultAsync(x => x.WalletID == balanceHistoryLog.WalletID && x.AccountID == balanceHistoryLog.AccountID) ?? throw new Exception(Message.WALLET_NOT_FOUND);
                var transaction = _context.Transaction.FirstOrDefaultAsync(x => x.TransactionID == balanceHistoryLog.TransactionID) ?? throw new Exception(Message.TRANSACTION_NOT_FOUND);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        internal async Task<object> GetBalanceHistoryLog(string accountID, int walletID, AutoMapper.IMapper? _mapper)
        {
            try
            {
                // get all balance history log by accountID and walletID
                var listlog = await _context.BalanceHistoryLogs.Where(x => x.AccountID == accountID && x.WalletID == walletID).ToListAsync();
                // check if result is null or empty or not found by accountID and walletID, then throw exception
                if (listlog is null || listlog.Count == 0)
                {
                    throw new Exception(Message.BALANCE_HISTORY_LOG_NOT_FOUND);
                }
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var listLogDTO = _mapper.Map<List<BalanceHisLog_VM_DTO>>(listlog);
                var balanceLogDict = new Dictionary<DateOnly, CustomBalanceHisLogByDate>();
                // group by date, if same date then sum the balance
                foreach (var log in listLogDTO)
                {
                    //var logDateOnly = new DateOnly();
                    // create new date only by log date day, month, year
                    var logDateOnly = new DateOnly(log.HisLogDate.Year, log.HisLogDate.Month, log.HisLogDate.Day);
                    if (balanceLogDict.TryGetValue(logDateOnly, out CustomBalanceHisLogByDate? value))
                    {
                        value.TotalAmount += log.Balance;
                        value.TransactionCount++;
                        value.BalanceHistoryLogs.Add(log);
                        value.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(value.TotalAmount);
                    }
                    else
                    {
                        balanceLogDict.Add(logDateOnly, new CustomBalanceHisLogByDate
                        {
                            Date = logDateOnly,
                            TotalAmount = log.Balance,
                            TransactionCount = 1,
                            BalanceHistoryLogs = [log],
                            TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(log.Balance)
                        });
                    }
                }
                // convert balance log dict to list
                var result = balanceLogDict.Values.ToList();
                // sort by date
                result.Sort((x, y) => x.Date.CompareTo(y.Date));
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            throw new NotImplementedException();
        }

        internal async Task<object> GetBalanceHistoryLogByDay(string accountID, AutoMapper.IMapper? _mapper)
        {
            try
            {
                var listlog = await _context.BalanceHistoryLogs.Where(x => x.AccountID == accountID).ToListAsync();
                if (listlog is null || listlog.Count == 0)
                {
                    throw new Exception(Message.BALANCE_HISTORY_LOG_NOT_FOUND);
                }
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var listLogDTO = _mapper.Map<List<BalanceHisLog_VM_DTO>>(listlog);
                var listAfter = FilterData(listLogDTO);
                return listAfter;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            throw new NotImplementedException();
        }

        internal async Task<object> GetBalanceHistoryLogByDate(string accountID, DateOnly fromDateOnly, DateOnly toDateOnly, IMapper? mapper)
        {
            try
            {
                var fromDate = new DateTime(fromDateOnly.Year, fromDateOnly.Month, fromDateOnly.Day).ToUniversalTime();
                var toDate = new DateTime(toDateOnly.Year, toDateOnly.Month, toDateOnly.Day, 23, 59, 59).ToUniversalTime();
                var listLog = await _context.BalanceHistoryLogs.Where(x => x.AccountID == accountID && x.HisLogDate >= fromDate && x.HisLogDate <= toDate).ToListAsync();
                if (listLog is null || listLog.Count == 0)
                {
                    throw new Exception(Message.BALANCE_HISTORY_LOG_NOT_FOUND);
                }
                if (mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var listLogDTO = mapper.Map<List<BalanceHisLog_VM_DTO>>(listLog);
                var listAfter = FilterData(listLogDTO);
                return listAfter;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            throw new NotImplementedException();
        }

        private static object FilterData(List<BalanceHisLog_VM_DTO> listLogDTO)
        {
            try
            {
                var balanceLogDict = new Dictionary<DateOnly, CustomBalanceHisLogByDate>();
                foreach (var log in listLogDTO)
                {
                    var logDateOnly = new DateOnly(log.HisLogDate.Year, log.HisLogDate.Month, log.HisLogDate.Day);
                    if (balanceLogDict.TryGetValue(logDateOnly, out CustomBalanceHisLogByDate? value))
                    {
                        value.TotalAmount += log.Balance;
                        value.TransactionCount++;
                        value.BalanceHistoryLogs.Add(log);
                        value.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(value.TotalAmount);
                    }
                    else
                    {
                        balanceLogDict.Add(logDateOnly, new CustomBalanceHisLogByDate
                        {
                            Date = logDateOnly,
                            TotalAmount = log.Balance,
                            TransactionCount = 1,
                            BalanceHistoryLogs = [log],
                            TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(log.Balance)
                        });
                    }
                }
                var result = balanceLogDict.Values.ToList();
                result.Sort((x, y) => x.Date.CompareTo(y.Date));
                return result;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
