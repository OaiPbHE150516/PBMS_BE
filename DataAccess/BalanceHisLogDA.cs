using AutoMapper;
using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Balance;
using pbms_be.Data.Custom;
using pbms_be.Data.WalletF;
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

                // get all wallet of account
                var listWallet = await _context.Wallet.Where(x => x.AccountID == accountID).ToListAsync();


                var listAfter = FilterData(listLogDTO, listWallet);
                var minBalance = listLogDTO.Min(x => x.Balance);
                var maxBalance = listLogDTO.Max(x => x.Balance);
                return new { minBalance, maxBalance, listAfter };
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
                // get all wallet of account
                var listWallet = await _context.Wallet.Where(x => x.AccountID == accountID).ToListAsync();
                var listLogDTO = mapper.Map<List<BalanceHisLog_VM_DTO>>(listLog);
                var listAfter = FilterData(listLogDTO, listWallet);
                // get min balance and max balance in list after
                var minBalance = listLogDTO.Min(x => x.Balance);
                var maxBalance = listLogDTO.Max(x => x.Balance);
                return new { minBalance, maxBalance, listAfter };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            throw new NotImplementedException();
        }

        private object FilterData(List<BalanceHisLog_VM_DTO> listLogDTO, List<Data.WalletF.Wallet> listWallet)
        {
            try
            {
                // filer all log by date of each day
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

                // filter last log of each wallet in each day
                foreach (var item in balanceLogDict)
                {
                    var balanceLog = item.Value.BalanceHistoryLogs;
                    var balanceLogDictByWalletID = new Dictionary<int, BalanceHisLog_VM_DTO>();
                    foreach (var log in balanceLog)
                    {
                        if (balanceLogDictByWalletID.TryGetValue(log.WalletID, out BalanceHisLog_VM_DTO? value))
                        {
                            if (value.HisLogDate < log.HisLogDate)
                            {
                                balanceLogDictByWalletID[log.WalletID] = log;
                            }
                        }
                        else
                        {
                            balanceLogDictByWalletID.Add(log.WalletID, log);
                        }
                    }
                    item.Value.BalanceHistoryLogs = [.. balanceLogDictByWalletID.Values];
                }

                // loop through balance log dict each day, 
                foreach (var item in balanceLogDict)
                {
                    var balanceLogs = item.Value.BalanceHistoryLogs;
                    var listWalletNotInLog = listWallet.Where(x => !balanceLogs.Any(y => y.WalletID == x.WalletID));
                    var listLogBalanceOfLostWalletLog = new List<BalanceHisLog_VM_DTO>();
                    foreach (var wallet in listWalletNotInLog)
                    {
                        var datetime = new DateTime(item.Key.Year, item.Key.Month, item.Key.Day).ToUniversalTime();

                        // get closest balance history log of a wallet by date
                        var closestBalanceHistoryLog = _context.BalanceHistoryLogs
                                                    .Where(x => x.WalletID == wallet.WalletID
                                                    && x.HisLogDate <= datetime)
                                                    .OrderByDescending(x => x.HisLogDate)
                                                    .FirstOrDefault();
                        if (closestBalanceHistoryLog is null) continue;

                        // log datetime to console
                        Console.WriteLine($"Date: {datetime}, WalletID: {wallet.WalletID}, ClosestBalanceHistoryLog: {closestBalanceHistoryLog.HisLogDate}");

                        listLogBalanceOfLostWalletLog.Add(new BalanceHisLog_VM_DTO
                        {
                            AccountID = closestBalanceHistoryLog.AccountID,
                            WalletID = closestBalanceHistoryLog.WalletID,
                            Balance = closestBalanceHistoryLog.Balance,
                            BalanceStr = LConvertVariable.ConvertToMoneyFormat(closestBalanceHistoryLog.Balance),
                            TransactionID = closestBalanceHistoryLog.TransactionID,
                            HisLogDate = closestBalanceHistoryLog.HisLogDate,
                            HisLogDateStr = LConvertVariable.ConvertDateTimeToString(closestBalanceHistoryLog.HisLogDate)
                        });
                    }
                    item.Value.BalanceHistoryLogs.AddRange(listLogBalanceOfLostWalletLog);
                }

                // re calculate total amount of each day
                foreach (var item in balanceLogDict)
                {
                    var balanceLog = item.Value.BalanceHistoryLogs;
                    var totalAmount = balanceLog.Sum(x => x.Balance);
                    item.Value.TotalAmount = totalAmount;
                    item.Value.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount);
                }

                var result = balanceLogDict.Values.ToList();
                result.Sort((x, y) => x.Date.CompareTo(y.Date));
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //// function get closest balance history log of a wallet by date
        //public async Task<BalanceHistoryLog> GetClosestBalanceHistoryLogByDate(int walletID, DateTime date)
        //{
        //    try
        //    {
        //        var result = await _context.BalanceHistoryLogs
        //            .Where(x => x.WalletID == walletID && x.HisLogDate <= date)
        //            .OrderByDescending(x => x.HisLogDate)
        //            .FirstOrDefaultAsync();
        //        return result is null ? throw new Exception(Message.BALANCE_HISTORY_LOG_NOT_FOUND) : result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
    }
}
