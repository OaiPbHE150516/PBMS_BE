﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Balance;
using pbms_be.Data.Custom;
using pbms_be.Data.Filter;
using pbms_be.Data.Invo;
using pbms_be.Data.Trans;
using pbms_be.DTOs;
using pbms_be.Library;
using System.Collections.Generic;
using System.Transactions;

namespace pbms_be.DataAccess
{
    public class TransactionDA
    {
        private readonly PbmsDbContext _context;
        public TransactionDA(PbmsDbContext context)
        {
            _context = context;
        }

        public List<Data.Trans.Transaction> GetTransactions(string AccountID, int pageNumber, int pageSize, string sortType)
        {
            try
            {
                var now = DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC).ToUniversalTime();
                int skip = (pageNumber - 1) * pageSize;
                var result = _context.Transaction
                            .Where(t => t.AccountID == AccountID && t.TransactionDate <= now && t.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(t => t.ActiveState)
                            .Include(t => t.Category)
                            .Include(t => t.Wallet)
                            .OrderByDescending(t => t.TransactionDate)
                            .ToList() ?? throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);
                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID) ?? throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    transaction.Category.CategoryType = cateType;
                }
                result = result.Skip(skip).Take(pageSize).ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Data.Trans.Transaction GetTransaction(int TransactionID)
        {
            try
            {
                var result = _context.Transaction
                .Where(t => t.TransactionID == TransactionID && t.ActiveStateID == ActiveStateConst.ACTIVE)
                .Include(t => t.ActiveState)
                .Include(t => t.Category)
                .Include(t => t.Wallet)
                .FirstOrDefault();
                return result is null ? throw new Exception(Message.TRANSACTION_NOT_FOUND) : result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Data.Trans.Transaction> GetTransactionsByCategory(int CategoryID)
        {
            var result = _context.Transaction
                .Where(t => t.CategoryID == CategoryID && t.ActiveStateID == ActiveStateConst.ACTIVE)
                .Include(t => t.ActiveState)
                .Include(t => t.Category)
                .Include(t => t.Wallet)
                .ToList();
            return result;
        }

        public List<Data.Trans.Transaction> GetTransactionsByWallet(int WalletID)
        {
            var result = _context.Transaction
                .Where(t => t.WalletID == WalletID && t.ActiveStateID == ActiveStateConst.ACTIVE)
                .Include(t => t.ActiveState)
                .Include(t => t.Category)
                .Include(t => t.Wallet)
                .ToList();
            return result;
        }

        internal object GetTotalPage(string accountID, int pageSize)
        {
            try
            {
                var totalRecord = _context.Transaction
                                .Where(t => t.AccountID == accountID && t.ActiveStateID == ActiveStateConst.ACTIVE)
                                .Count();
                var totalPage = totalRecord / pageSize;
                if (totalRecord % pageSize != 0) totalPage++;
                return totalPage;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetTransaction(int transactionID, string accountID, IMapper? _mapper)
        {
            try
            {
                var result = _context.Transaction
                            .Where(t => t.TransactionID == transactionID && t.AccountID == accountID && t.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(t => t.ActiveState)
                            .Include(t => t.Category)
                            .Include(t => t.Wallet)
                            .FirstOrDefault() ?? throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);
                result.Category.CategoryType = cateDA.GetCategoryType(result.Category.CategoryTypeID);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<TransactionDetail_VM_DTO>(result);
                // get invoice of transaction
                var invoice = _context.Invoice
                            .Where(i => i.TransactionID == transactionID && i.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(i => i.Currency)
                            .Include(i => i.ActiveState)
                            .Include(i => i.ProductInInvoices)
                            .FirstOrDefault();
                if (invoice is not null) resultDTO.Invoice = _mapper.Map<Invoice_VM_DTO>(invoice);
                return resultDTO;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //internal Transaction CreateTransaction(Transaction transaction)
        //{
        //    try
        //    {
        //        transaction.TransactionDate = DateTime.UtcNow;
        //        transaction.ActiveStateID = ActiveStateConst.ACTIVE;
        //        _context.Transaction.Add(transaction);
        //        _context.SaveChanges();
        //        return transaction;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}
        internal Data.Trans.Transaction CreateTransactionV2(Data.Trans.Transaction transaction, DateTime transactionDate)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                if (transaction is null) throw new Exception(Message.TRANSACTION_IS_NULL);
                var wallet = _context.Wallet
                            .Where(w => w.WalletID == transaction.WalletID && w.AccountID == transaction.AccountID && w.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(w => w.ActiveState)
                            .FirstOrDefault() ?? throw new Exception(Message.WALLET_NOT_BELONG_ACCOUNT + ": " + transaction.AccountID);
                var category = _context.Category
                            .Where(c => c.CategoryID == transaction.CategoryID && c.AccountID == transaction.AccountID && c.ActiveStateID == ActiveStateConst.ACTIVE)
                            .Include(c => c.ActiveState)
                            .Include(c => c.CategoryType)
                            .FirstOrDefault() ?? throw new Exception(Message.CATEGORY_NOT_BELONG_ACCOUNT + ": " + transaction.AccountID);

                // transactionDate - 7 hours to get correct date
                transaction.TransactionDate = transactionDate.ToUniversalTime();
                transaction.ActiveStateID = ActiveStateConst.ACTIVE;
                _context.Transaction.Add(transaction);
                _context.SaveChanges();

                var walletDA = new WalletDA(_context);
                walletDA.UpdateWalletAmount(transaction.WalletID, transaction.TotalAmount, category.CategoryTypeID);
                
                var balanceHisLogDA = new BalanceHisLogDA(_context);
                var balancehislog = new BalanceHistoryLog
                {
                    AccountID = transaction.AccountID,
                    WalletID = transaction.WalletID,
                    Balance = wallet.Balance,
                    TransactionID = transaction.TransactionID,
                    HisLogDate = DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC),
                    ActiveStateID = ActiveStateConst.ACTIVE
                };
                 balanceHisLogDA.CreateBalanceHistoryLog(balancehislog);
                
                var budgetDA = new BudgetDA(_context);
                budgetDA.UpdateBudgetAmount(transaction.AccountID, transaction.TotalAmount, category.CategoryID);
                
                scope.Complete();
                return transaction;
            }
            catch (Exception e)
            {
                scope.Dispose();
                throw new Exception(e.Message);
            }
        }

        internal Data.Trans.Transaction CreateTransactionRaw(Data.Trans.Transaction transaction)
        {
            try
            {
                transaction.ActiveStateID = ActiveStateConst.ACTIVE;
                _context.Transaction.Add(transaction);
                _context.SaveChanges();
                return transaction;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // create transaction by list of transactions raw
        internal List<Data.Trans.Transaction> CreateTransactionsRaw(List<Data.Trans.Transaction> transactions)
        {
            try
            {
                _context.Transaction.AddRange(transactions);
                _context.SaveChanges();
                return transactions;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal bool IsTransactionExist(Data.Trans.Transaction transaction)
        {
            try
            {
                transaction.TransactionDate = DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC).ToUniversalTime();
                transaction.ActiveStateID = ActiveStateConst.ACTIVE;
                var result = _context.Transaction
                            .Any(t => t.AccountID == transaction.AccountID
                             && t.WalletID == transaction.WalletID
                             && t.CategoryID == transaction.CategoryID
                             && t.TotalAmount == transaction.TotalAmount
                             && t.Note == transaction.Note
                             && t.TransactionDate.Minute == transaction.TransactionDate.Minute
                             && t.FromPerson == transaction.FromPerson
                             && t.ToPerson == transaction.ToPerson
                             && t.ImageURL == transaction.ImageURL);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Data.Trans.Transaction> GetTransactionsByMonth(string accountID, int month, int year)
        {
            try
            {
                var result = _context.Transaction
                                    .Where(t => t.AccountID == accountID
                                    && t.TransactionDate.Month == month
                                    && t.TransactionDate.Year == year)
                                    .Include(t => t.ActiveState)
                                    .Include(t => t.Category)
                                    .Include(t => t.Wallet)
                                    .ToList() ?? throw new Exception(Message.TRANSACTION_NOT_FOUND);
                // remove list transaction that is not active
                result = result.FindAll(t => t.ActiveStateID == ActiveStateConst.ACTIVE);
                var cateDA = new CategoryDA(_context);
                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    transaction.TransactionDate = transaction.TransactionDate;
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID) ?? throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    transaction.Category.CategoryType = cateType;
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Data.Trans.Transaction> GetTransactionsByDay(string accountID, int day, int month, int year)
        {
            try
            {
                var result = _context.Transaction
                                    .Where(t => t.AccountID == accountID
                                    && t.TransactionDate.Day == day
                                    && t.TransactionDate.Month == month
                                    && t.TransactionDate.Year == year)
                                    .Include(t => t.ActiveState)
                                    .Include(t => t.Category)
                                    .Include(t => t.Wallet)
                                    .ToList() ?? throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);
                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID) ?? throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    transaction.Category.CategoryType = cateType;
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Data.Trans.Transaction> GetTransactionsByDateTimeRange(string accountID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                //var newFromDate = fromDate.AddDays(-1).ToUniversalTime();
                //var newToDate = toDate.AddDays(1).ToUniversalTime();
                //int toDay = toDate.Day + 1;


                //var result = _context.Transaction
                //                    .Where(t => t.AccountID == accountID
                //                    && t.TransactionDate >= newFromDate
                //                    && t.TransactionDate <= newToDate)
                //                    .Include(t => t.ActiveState)
                //                    .Include(t => t.Category)
                //                    .Include(t => t.Wallet)
                //                    .ToList() ?? throw new Exception(Message.TRANSACTION_NOT_FOUND);
                //// remove transaction that is not in range of fromDate and toDate by using CompareTo method
                //var listTran = new List<Transaction>();
                //foreach (var tran in result)
                //{
                //    int tranDay = tran.TransactionDate.Day;
                //    if (tranDay < toDay)
                //    {
                //        listTran.Add(tran);
                //    }                    
                //}
                var fromDateTimeUtc = fromDate.ToUniversalTime();
                var toDateTimeUtc = toDate.ToUniversalTime();
                // log fromDateTimeUtc and toDateTimeUtc
                Console.WriteLine("fromDateTimeUtc: " + fromDateTimeUtc);
                Console.WriteLine("toDateTimeUtc: " + toDateTimeUtc);

                //var fromDateStr = fromDate.ToString("yyyy-MM-dd");
                //var toDateStr = toDate.ToString("yyyy-MM-dd");

                // var listTran is by excute sql query raw to get transactions in range of fromDate and toDate
                var listTran = _context.Transaction
                                    .FromSql($"SELECT * FROM transaction WHERE account_id = {accountID} AND transaction_date BETWEEN {fromDateTimeUtc} AND {toDateTimeUtc}")
                                    .Include(t => t.ActiveState)
                                    .Include(t => t.Category)
                                    .Include(t => t.Wallet)
                                    .ToList();
                // remove which transaction not have activestateID = 1
                listTran = listTran.FindAll(t => t.ActiveStateID == ActiveStateConst.ACTIVE);

                // sort listTran by date
                listTran = [.. listTran.OrderByDescending(t => t.TransactionDate)];
                var cateDA = new CategoryDA(_context);
                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in listTran)
                {
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID) ?? throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    transaction.Category.CategoryType = cateType;
                }
                return listTran;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal Dictionary<int, TransactionInDayCalendar> GetTransactionsByMonthCalendar(string accountID, int month, int year, IMapper? _mapper)
        {
            try
            {
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var transactions = GetTransactionsByMonth(accountID, month, year);
                var transactionsByDay = new Dictionary<int, TransactionInDayCalendar>();
                var daysInMonth = DateTime.DaysInMonth(year, month);
                foreach (var transaction in transactions)
                {
                    var day = transaction.TransactionDate.Day;
                    if (transactionsByDay.ContainsKey(day))
                    {
                        transactionsByDay[day].TotalAmount += transaction.TotalAmount;
                        transactionsByDay[day].TransactionCount++;
                        transactionsByDay[day].Transactions.Add(_mapper.Map<TransactionDetail_VM_DTO>(transaction));
                    }
                    else
                    {
                        TransactionInDayCalendar transactionInDayCalendar = new TransactionInDayCalendar
                        {
                            Date = new DateOnly(transaction.TransactionDate.Year, transaction.TransactionDate.Month, transaction.TransactionDate.Day),
                            isHasTransaction = true,
                            TransactionCount = 1,
                            Transactions = [_mapper.Map<TransactionDetail_VM_DTO>(transaction)]
                        };
                        transactionsByDay.Add(day, transactionInDayCalendar);
                    }
                    if (transaction.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                    {
                        transactionsByDay[day].isHasTransactionIn = true;
                        transactionsByDay[day].TotalAmountIn += transaction.TotalAmount;
                    }
                    else
                    {
                        transactionsByDay[day].isHasTransactionOut = true;
                        transactionsByDay[day].TotalAmountOut += transaction.TotalAmount;
                    }
                }
                foreach (var item in transactionsByDay)
                {
                    item.Value.TotalAmount = item.Value.TotalAmountIn - item.Value.TotalAmountOut;
                    item.Value.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmount);
                    item.Value.TotalAmountInStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmountIn);
                    item.Value.TotalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmountOut);
                }
                transactionsByDay = transactionsByDay.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                return transactionsByDay;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetRecentlyTransactions(string accountID, int number)
        {
            try
            {
                var now = DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC).ToUniversalTime();
                var result = _context.Transaction
                            .Where(t => t.AccountID == accountID && t.TransactionDate <= now)
                            .Include(t => t.ActiveState)
                            .Include(t => t.Category)
                            .Include(t => t.Wallet)
                            .OrderByDescending(t => t.TransactionDate)
                            .Take(number)
                            .ToList() ?? throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);

                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID) ?? throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    transaction.Category.CategoryType = cateType;
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetTotalAmountInAndOutByMonth(string accountID, int month, int year)
        {
            try
            {
                var transactions = GetTransactionsByMonth(accountID, month, year);
                long totalAmountIn = 0;
                int numberTransIn = 0;
                long totalAmountOut = 0;
                int numberTransOut = 0;
                foreach (var transaction in transactions)
                {
                    if (transaction.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                    {
                        totalAmountIn += transaction.TotalAmount;
                        numberTransIn++;
                    }
                    else
                    {
                        totalAmountOut += transaction.TotalAmount;
                        numberTransOut++;
                    }
                }
                var totalAmount = totalAmountIn - totalAmountOut;
                return new
                {
                    numberTransIn,
                    totalAmountIn,
                    totalAmountInStr = LConvertVariable.ConvertToMoneyFormat(totalAmountIn),
                    numberTransOut,
                    totalAmountOut,
                    totalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(totalAmountOut),
                    totalAmount,
                    totalNumberTrans = numberTransIn + numberTransOut,
                    totalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount)
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetTotalAmountInAndOutByLastDays(string accountID)
        {
            try
            {
                var fromDate = DateTime.UtcNow.AddDays(-ConstantConfig.NUMBER_LAST_DAYS);
                var toDate = DateTime.UtcNow;
                var transactions = GetTransactionsByDateTimeRange(accountID, fromDate, toDate);
                var transactionsDict = new Dictionary<DateOnly, TransactionInLastDays>();

                foreach (var tran in transactions)
                {
                    var dateonly = new DateOnly(tran.TransactionDate.Year, tran.TransactionDate.Month, tran.TransactionDate.Day);
                    if (transactionsDict.TryGetValue(dateonly, out TransactionInLastDays? value))
                    {
                        if (tran.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                        {
                            value.TotalAmountIn += tran.TotalAmount;
                            value.NumberOfTransactionIn++;
                        }
                        else
                        {
                            value.TotalAmountOut += tran.TotalAmount;
                            value.NumberOfTransactionOut++;
                        }
                    }
                    else
                    {
                        var transin = new TransactionInLastDays
                        {
                            DayDetail = new DayDetail
                            {
                                DayOfWeek = dateonly.DayOfWeek,
                                Short_EN = dateonly.DayOfWeek.ToString().Substring(0, 3),
                                Full_EN = dateonly.DayOfWeek.ToString(),
                                Short_VN = LConvertVariable.ConvertDayInWeekToVN_SHORT_3(dateonly.DayOfWeek),
                                Full_VN = LConvertVariable.ConvertDayInWeekToVN_FULL(dateonly.DayOfWeek),
                                ShortDate = LConvertVariable.ConvertDateOnlyToVN_ng_thg(dateonly),
                                FullDate = LConvertVariable.ConvertDateOnlyToVN_ngay_thang(dateonly),
                                DayStr = dateonly.Day.ToString(),
                                MonthYearStr = $"tháng {dateonly.Month}, {dateonly.Year}"
                            }
                        };
                        if (tran.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                        {
                            transin.TotalAmountIn += tran.TotalAmount;
                            transin.NumberOfTransactionIn++;
                        }
                        else
                        {
                            transin.TotalAmountOut += tran.TotalAmount;
                            transin.NumberOfTransactionOut++;
                        }
                        transactionsDict.Add(dateonly, transin);
                    }
                }
                foreach (var item in transactionsDict)
                {
                    item.Value.TotalAmount = item.Value.TotalAmountIn - item.Value.TotalAmountOut;
                    item.Value.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmount);
                    item.Value.TotalAmountInStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmountIn);
                    item.Value.TotalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmountOut);
                }

                var listValue = new List<long>() {
                    transactionsDict.Values.Max(t => t.TotalAmountIn),
                    transactionsDict.Values.Max(t => t.TotalAmountOut),
                    transactionsDict.Values.Min(t => t.TotalAmountIn),
                    transactionsDict.Values.Min(t => t.TotalAmountOut)
                };

                // loop from fromDate to toDate, if not exist in transactionsDict, add new TransactionInLastDays with TotalAmount = 0
                for (var date = fromDate; date <= toDate; date = date.AddDays(1))
                {
                    var dateonly = new DateOnly(date.Year, date.Month, date.Day);
                    if (!transactionsDict.ContainsKey(dateonly))
                    {
                        var transin = new TransactionInLastDays
                        {
                            DayDetail = new DayDetail
                            {
                                DayOfWeek = dateonly.DayOfWeek,
                                Short_EN = dateonly.DayOfWeek.ToString().Substring(0, 3),
                                Full_EN = dateonly.DayOfWeek.ToString(),
                                Short_VN = LConvertVariable.ConvertDayInWeekToVN_SHORT_3(dateonly.DayOfWeek),
                                Full_VN = LConvertVariable.ConvertDayInWeekToVN_FULL(dateonly.DayOfWeek),
                                ShortDate = LConvertVariable.ConvertDateOnlyToVN_ng_thg(dateonly),
                                FullDate = LConvertVariable.ConvertDateOnlyToVN_ngay_thang(dateonly),
                                DayStr = dateonly.Day.ToString(),
                                MonthYearStr = $"tháng {dateonly.Month}, {dateonly.Year}"
                            }
                        };
                        transactionsDict.Add(dateonly, transin);
                    }
                }
                transactionsDict = transactionsDict.OrderByDescending(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                return new
                {
                    minValue = listValue.Min() == 0 ? listValue.OrderBy(v => v).Skip(1).First() : listValue.Min(),
                    maxValue = listValue.Max(),
                    transactionsDict
                };


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            /*
              dayInWeekShortTypeVN = date.DayOfWeek.ToString().Substring(0, 3) switch
                        {
                            "Mon" => "T2",
                            "Tue" => "T3",
                            "Wed" => "T4",
                            "Thu" => "T5",
                            "Fri" => "T6",
                            "Sat" => "T7",
                            "Sun" => "CN",
                            _ => "CN"
                        },
             */
        }

        internal object GetTransactionsDayByDay(string accountID, DateTime fromDateTime, DateTime toDateTime, IMapper? _mapper)
        {
            try
            {
                var transactions = GetTransactionsByDateTimeRange(accountID, fromDateTime, toDateTime);
                var transactionsDict = new Dictionary<DateOnly, TransactionDayByDay>();
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);

                foreach (var tran in transactions)
                {
                    var dateonly = new DateOnly(tran.TransactionDate.Year, tran.TransactionDate.Month, tran.TransactionDate.Day);
                    if (transactionsDict.TryGetValue(dateonly, out TransactionDayByDay? value))
                    {
                        value.Transactions.Add(_mapper.Map<TransactionInList_VM_DTO>(tran));
                        if (tran.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                        {
                            value.TotalAmountIn += tran.TotalAmount;
                            value.NumberOfTransactionIn++;
                        }
                        else
                        {
                            value.TotalAmountOut += tran.TotalAmount;
                            value.NumberOfTransactionOut++;
                        }
                    }
                    else
                    {
                        var transin = new TransactionDayByDay
                        {
                            DayDetail = new DayDetail
                            {
                                DayOfWeek = dateonly.DayOfWeek,
                                Short_EN = dateonly.DayOfWeek.ToString().Substring(0, 3),
                                Full_EN = dateonly.DayOfWeek.ToString(),
                                Short_VN = LConvertVariable.ConvertDayInWeekToVN_SHORT_3(dateonly.DayOfWeek),
                                Full_VN = LConvertVariable.ConvertDayInWeekToVN_FULL(dateonly.DayOfWeek),
                                ShortDate = LConvertVariable.ConvertDateOnlyToVN_ng_thg(dateonly),
                                FullDate = LConvertVariable.ConvertDateOnlyToVN_ngay_thang(dateonly),
                                DayStr = dateonly.Day.ToString(),
                                MonthYearStr = $"tháng {dateonly.Month}, {dateonly.Year}"
                            }
                        };
                        if (tran.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                        {
                            transin.TotalAmountIn += tran.TotalAmount;
                            transin.NumberOfTransactionIn++;
                        }
                        else
                        {
                            transin.TotalAmountOut += tran.TotalAmount;
                            transin.NumberOfTransactionOut++;
                        }
                        transin.Transactions = new List<TransactionInList_VM_DTO> { _mapper.Map<TransactionInList_VM_DTO>(tran) };
                        transactionsDict.Add(dateonly, transin);
                    }
                }
                foreach (var item in transactionsDict)
                {
                    item.Value.TotalAmount = item.Value.TotalAmountIn - item.Value.TotalAmountOut;
                    item.Value.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmount);
                    item.Value.TotalAmountInStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmountIn);
                    item.Value.TotalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(item.Value.TotalAmountOut);
                }
                transactionsDict = transactionsDict.OrderByDescending(t => t.Key).ToDictionary(t => t.Key, t => t.Value);
                return transactionsDict;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        internal object GetTransactionsWeekByWeek(string accountID, DateTime fromDateTime, DateTime toDateTime, IMapper? mapper)
        {
            try
            {
                var result = new TransactionWeekByWeek
                {
                    WeekDetail = new WeekDetail
                    {
                        StartDate = new DateOnly(fromDateTime.Year, fromDateTime.Month, fromDateTime.Day),
                        StartDateStrShort = LConvertVariable.ConvertDateToShortStr(fromDateTime),
                        StartDateStrFull = fromDateTime.ToString(ConstantConfig.DEFAULT_DATE_FORMAT),
                        DayOfWeekStartStr = LConvertVariable.ConvertDayInWeekToVN_SHORT_4(fromDateTime.DayOfWeek),
                        EndDate = new DateOnly(toDateTime.Year, toDateTime.Month, toDateTime.Day),
                        EndDateStrShort = LConvertVariable.ConvertDateToShortStr(toDateTime),
                        EndDateStrFull = toDateTime.ToString(ConstantConfig.DEFAULT_DATE_FORMAT),
                        DayOfWeekEndStr = LConvertVariable.ConvertDayInWeekToVN_SHORT_4(toDateTime.DayOfWeek)
                    },
                    TransactionsByDay = []
                };
                if (mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var transactions = GetTransactionsByDateTimeRange(accountID, fromDateTime, toDateTime);
                foreach (var transaction in transactions)
                {
                    if (transaction.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                    {
                        result.TotalAmountIn += transaction.TotalAmount;
                        result.NumberOfTransactionIn++;
                    }
                    else
                    {
                        result.TotalAmountOut += transaction.TotalAmount;
                        result.NumberOfTransactionOut++;
                    }
                    var tran = mapper.Map<TransactionInList_VM_DTO>(transaction);
                    var dateonly = new DateOnly(tran.TransactionDate.Year, tran.TransactionDate.Month, tran.TransactionDate.Day);
                    if (result.TransactionsByDay.TryGetValue(dateonly, out DayInByWeek? value))
                    {
                        value.DayDetail = new DayDetail
                        {
                            DayOfWeek = dateonly.DayOfWeek,
                            Short_EN = dateonly.DayOfWeek.ToString().Substring(0, 3),
                            Full_EN = dateonly.DayOfWeek.ToString(),
                            Short_VN = LConvertVariable.ConvertDayInWeekToVN_SHORT_3(dateonly.DayOfWeek),
                            Full_VN = LConvertVariable.ConvertDayInWeekToVN_FULL(dateonly.DayOfWeek),
                            ShortDate = LConvertVariable.ConvertDateOnlyToVN_ng_thg(dateonly),
                            FullDate = LConvertVariable.ConvertDateOnlyToVN_ngay_thang(dateonly),
                            DayStr = dateonly.Day.ToString(),
                            MonthYearStr = $"tháng {dateonly.Month}, {dateonly.Year}"
                        };
                        value.Transactions.Add(tran);
                    }
                    else
                    {
                        result.TransactionsByDay.Add(dateonly, new DayInByWeek
                        {
                            DayDetail = new DayDetail
                            {
                                DayOfWeek = dateonly.DayOfWeek,
                                Short_EN = dateonly.DayOfWeek.ToString().Substring(0, 3),
                                Full_EN = dateonly.DayOfWeek.ToString(),
                                Short_VN = LConvertVariable.ConvertDayInWeekToVN_SHORT_3(dateonly.DayOfWeek),
                                Full_VN = LConvertVariable.ConvertDayInWeekToVN_FULL(dateonly.DayOfWeek),
                                ShortDate = LConvertVariable.ConvertDateOnlyToVN_ng_thg(dateonly),
                                FullDate = LConvertVariable.ConvertDateOnlyToVN_ngay_thang(dateonly),
                                DayStr = dateonly.Day.ToString(),
                                MonthYearStr = $"tháng {dateonly.Month}, {dateonly.Year}"
                            },
                            Transactions = [tran]
                        });
                    }
                    if (transaction.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                    {
                        result.TransactionsByDay[dateonly].TotalAmountIn += transaction.TotalAmount;
                        result.TransactionsByDay[dateonly].TotalAmountInStr = LConvertVariable.ConvertToMoneyFormat(result.TransactionsByDay[dateonly].TotalAmountIn);
                    }
                    else
                    {
                        result.TransactionsByDay[dateonly].TotalAmountOut += transaction.TotalAmount;
                        result.TransactionsByDay[dateonly].TotalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(result.TransactionsByDay[dateonly].TotalAmountOut);
                    }
                    result.TransactionsByDay[dateonly].TotalAmount = result.TransactionsByDay[dateonly].TotalAmountIn - result.TransactionsByDay[dateonly].TotalAmountOut;
                    result.TransactionsByDay[dateonly].TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(result.TransactionsByDay[dateonly].TotalAmount);
                }
                result.TotalAmount = result.TotalAmountIn - result.TotalAmountOut;
                result.TransactionCount = result.NumberOfTransactionIn + result.NumberOfTransactionOut;
                result.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(result.TotalAmount);
                result.TotalAmountInStr = LConvertVariable.ConvertToMoneyFormat(result.TotalAmountIn);
                result.TotalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(result.TotalAmountOut);

                result.TransactionsByDay = result.TransactionsByDay.OrderByDescending(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                // var listTran = result.TransactionsByDay.Values.ToList();

                var result2 = new TransactionWeekByWeek2
                {
                    WeekDetail = result.WeekDetail,
                    NumberOfTransactionIn = result.NumberOfTransactionIn,
                    TotalAmountIn = result.TotalAmountIn,
                    TotalAmountInStr = result.TotalAmountInStr,
                    NumberOfTransactionOut = result.NumberOfTransactionOut,
                    TotalAmountOut = result.TotalAmountOut,
                    TotalAmountOutStr = result.TotalAmountOutStr,
                    TransactionCount = result.TransactionCount,
                    TotalAmount = result.TotalAmount,
                    TotalAmountStr = result.TotalAmountStr,
                    TransactionByDayW = result.TransactionsByDay.Values.ToList()
                };

                foreach (var tran in result2.TransactionByDayW)
                {
                    tran.Transactions = [.. tran.Transactions.OrderByDescending(t => t.TransactionDate.TimeOfDay)];
                }
                return result2;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            throw new NotImplementedException();
        }

        //         internal Transaction CreateTransactionV2(Transaction transaction, DateTime transactionDate)
        internal object CreateTransactionWithImage(Data.Trans.Transaction transaction, TransactionCreateWithImageDTO transactionDTO, IFormFile image)
        {
            try
            {
                var filename = LConvertVariable.GenerateRandomString(CloudStorageConfig.DEFAULT_FILE_NAME_LENGTH, Path.GetFileNameWithoutExtension(image.FileName));
                var fileURL = GCP_BucketDA.UploadFileCustom(image, CloudStorageConfig.PBMS_BUCKET_NAME, CloudStorageConfig.INVOICE_FOLDER,
                                                                               "invoice", filename, "file", true);
                transaction.ImageURL = fileURL;
                transaction.TransactionDate = transactionDTO.TransactionDate;
                transaction.ActiveStateID = ActiveStateConst.ACTIVE;
                CreateTransactionV2(transaction, transaction.TransactionDate);
                return transaction;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            throw new NotImplementedException();
        }

        internal object GetExpensesByLastDays(string accountID, int numdays, IMapper? _mapper)
        {
            try
            {
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var fromDate = DateTime.UtcNow.AddDays(-numdays);
                var toDate = DateTime.UtcNow.AddHours(ConstantConfig.VN_TIMEZONE_UTC).ToUniversalTime();
                var transactions = GetTransactionsByDateTimeRange(accountID, fromDate, toDate);
                var transactionsExpenses = transactions.Where(t => t.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_EXPENSE).ToList();
                // group by day
                var transactionsDict = new Dictionary<DateOnly, List<TransactionInList_VM_DTO>>();
                foreach (var tran in transactionsExpenses)
                {
                    var dateonly = new DateOnly(tran.TransactionDate.Year, tran.TransactionDate.Month, tran.TransactionDate.Day);
                    var transDTO = _mapper.Map<TransactionInList_VM_DTO>(tran);
                    if (transactionsDict.TryGetValue(dateonly, out List<TransactionInList_VM_DTO>? value))
                    {
                        value.Add(transDTO);
                    }
                    else
                    {
                        transactionsDict.Add(dateonly, [transDTO]);
                    }
                }
                // sort by date
                transactionsDict = transactionsDict.OrderByDescending(t => t.Key).ToDictionary(t => t.Key, t => t.Value);

                // new dictionary, key is int index, value is new object {DateOnlt, List<TransactionInList_VM_DTO>}
                //var transactionsDict2 = new Dictionary<KeyExtractor, object>();
                //var index = 0;
                //foreach (var item in transactionsDict)
                //{
                //    transactionsDict2.Add(new KeyExtractor
                //    {
                //        Key = index.ToString()
                //    }, new
                //    {
                //        DayDetail = LConvertVariable.ConvertDateOnlyToDayDetail(item.Key),
                //        Transactions = item.Value
                //    });
                //    index++;
                //}
                //return transactionsDict2;

                var listResult = new List<object>();
                var index = 1;
                foreach (var item in transactionsDict)
                {
                    listResult.Add(new
                    {
                        KeyExtractor = index,
                        DayDetail = LConvertVariable.ConvertDateOnlyToDayDetail(item.Key),
                        Transactions = item.Value
                    });
                    index++;
                }
                return listResult;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetAllTransactionFilterCategory(int month, int year, string accountID, IMapper? mapper)
        {
            try
            {
                // 1. check that accountID is exist
                var account = _context.Account
                            .Where(a => a.AccountID == accountID)
                            .FirstOrDefault() ?? throw new Exception(Message.ACCOUNT_NOT_FOUND);
                // 3. get all transactions by month and year
                var transactionsByMonth = _context.Transaction
                            .Where(t => t.AccountID == accountID && t.TransactionDate.Month == month && t.TransactionDate.Year == year)
                            .ToList();
                // 4. group by category
                var transactionsByCategory = transactionsByMonth.GroupBy(t => t.CategoryID).ToList();
                // 5. return list of category with total amount of each category and number of transaction of each category
                var result = new List<CategoryWithTransactionData>();
                long totalAmountOfMonth = 0;
                var countCate = 1;
                foreach (var tran in transactionsByCategory)
                {
                    var category = _context.Category
                                .Where(c => c.CategoryID == tran.Key)
                                .Include(c => c.CategoryType)
                                .FirstOrDefault() ?? throw new Exception(Message.CATEGORY_NOT_FOUND);
                    var totalAmount = tran.Sum(t => t.TotalAmount);
                    var totalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount);
                    var numberOfTransaction = tran.Count();
                    if (mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                    var CategoryWithAllTransaction = new CategoryWithTransactionData
                    {
                        CategoryNumber = countCate++,
                        Category = mapper.Map<CategoryDetail_VM_DTO>(category),
                        TotalAmount = totalAmount,
                        TotalAmountStr = totalAmountStr,
                        NumberOfTransaction = numberOfTransaction
                    };
                    result.Add(CategoryWithAllTransaction);
                    totalAmountOfMonth += totalAmount;
                }
                // foreach to calculate percentage of each category
                foreach (var item in result)
                {
                    // calculate percentage to 2 decimal places
                    item.Percentage = ((double)item.TotalAmount / totalAmountOfMonth * 100);
                    item.PercentageStr = item.Percentage.ToString("0.00") + "%";
                }
                return new
                {
                    TotalAmountOfMonth = totalAmountOfMonth,
                    TotalAmountOfMonthStr = LConvertVariable.ConvertToMoneyFormat(totalAmountOfMonth),
                    TotalNumberOfTransaction = transactionsByMonth.Count,
                    TotalNumberOfCategory = transactionsByCategory.Count,
                    CategoryWithTransactionData = result
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetAllTransactionFilterType(int month, int year, string accountID, IMapper? mapper)
        {
            try
            {
                // 1. check that accountID is exist
                var account = _context.Account
                            .Where(a => a.AccountID == accountID)
                            .FirstOrDefault() ?? throw new Exception(Message.ACCOUNT_NOT_FOUND);
                // 2. get all transactions by month and year, include category
                var transactionsByMonth = _context.Transaction
                            .Where(t => t.AccountID == accountID 
                            && t.ActiveStateID == ActiveStateConst.ACTIVE
                            && t.TransactionDate.Month == month 
                            && t.TransactionDate.Year == year)
                            .Include(t => t.Category)
                            .ToList();
                // 3. group by category type
                var transactionsByType = transactionsByMonth.GroupBy(t => t.Category.CategoryTypeID).ToList();
                // 4. return list of category type with total amount of each category type and number of transaction of each category type
                var result = new List<CategoryWithTransactionData2>();
                long totalAmountOfMonth = 0;
                var countType = 1;
                foreach (var tran in transactionsByType)
                {
                    var categoryType = _context.CategoryType
                                .Where(c => c.CategoryTypeID == tran.Key)
                                .FirstOrDefault() ?? throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    var totalAmount = tran.Sum(t => t.TotalAmount);
                    var totalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount);
                    var numberOfTransaction = tran.Count();
                    if (mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                    var CategoryTypeWithAllTransaction = new CategoryWithTransactionData2
                    {
                        CategoryTypeNumber = countType++,
                        CategoryType = categoryType,
                        TotalAmount = totalAmount,
                        TotalAmountStr = totalAmountStr,
                        NumberOfTransaction = numberOfTransaction
                    };
                    result.Add(CategoryTypeWithAllTransaction);
                    totalAmountOfMonth += totalAmount;
                }   
                // foreach to calculate percentage of each category type
                foreach (var item in result)
                {
                    // calculate percentage to 2 decimal places
                    item.Percentage = ((double)item.TotalAmount / totalAmountOfMonth * 100);
                    item.PercentageStr = item.Percentage.ToString("0.00") + "%";
                }
                return new
                {
                    TotalAmountOfMonth = totalAmountOfMonth,
                    TotalAmountOfMonthStr = LConvertVariable.ConvertToMoneyFormat(totalAmountOfMonth),
                    TotalNumberOfTransaction = transactionsByMonth.Count,
                    TotalNumberOfCategoryType = transactionsByType.Count,
                    CategoryWithTransactionData = result
                };
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
