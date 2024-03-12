using AutoMapper;
using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Custom;
using pbms_be.Data.Invo;
using pbms_be.Data.Trans;
using pbms_be.DTOs;
using pbms_be.Library;
using System.Collections.Generic;

namespace pbms_be.DataAccess
{
    public class TransactionDA
    {
        private readonly PbmsDbContext _context;
        public TransactionDA(PbmsDbContext context)
        {
            _context = context;
        }

        public List<Transaction> GetTransactions(string AccountID, int pageNumber, int pageSize, string sortType)
        {
            try
            {
                var now = DateTime.UtcNow;
                int skip = (pageNumber - 1) * pageSize;
                var result = _context.Transaction
                            .Where(t => t.AccountID == AccountID && t.TransactionDate <= now)
                            .Include(t => t.ActiveState)
                            .Include(t => t.Category)
                            .Include(t => t.Wallet)
                            .OrderByDescending(t => t.TransactionDate)
                            .ToList();
                if (result is null) throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);
                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID);
                    if (cateType is null) throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
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

        public Transaction GetTransaction(int TransactionID)
        {
            try
            {
                var result = _context.Transaction
                .Where(t => t.TransactionID == TransactionID)
                .Include(t => t.ActiveState)
                .Include(t => t.Category)
                .Include(t => t.Wallet)
                .FirstOrDefault();
                if (result is null) throw new Exception(Message.TRANSACTION_NOT_FOUND);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Transaction> GetTransactionsByCategory(int CategoryID)
        {
            var result = _context.Transaction
                .Where(t => t.CategoryID == CategoryID)
                .Include(t => t.ActiveState)
                .Include(t => t.Category)
                .Include(t => t.Wallet)
                .ToList();
            return result;
        }

        public List<Transaction> GetTransactionsByWallet(int WalletID)
        {
            var result = _context.Transaction
                .Where(t => t.WalletID == WalletID)
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
                                .Where(t => t.AccountID == accountID)
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

        internal object GetTransaction(int transactionID, string accountID)
        {
            try
            {
                var result = _context.Transaction
                            .Where(t => t.TransactionID == transactionID && t.AccountID == accountID)
                            .Include(t => t.ActiveState)
                            .Include(t => t.Category)
                            .Include(t => t.Wallet)
                            .FirstOrDefault();
                if (result is null) throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);
                result.Category.CategoryType = cateDA.GetCategoryType(result.Category.CategoryTypeID);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal Transaction CreateTransaction(Transaction transaction)
        {
            try
            {
                transaction.TransactionDate = DateTime.UtcNow;
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

        internal Transaction CreateTransactionRaw(Transaction transaction)
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
        internal List<Transaction> CreateTransactionsRaw(List<Transaction> transactions)
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

        internal bool IsTransactionExist(Transaction transaction)
        {
            try
            {
                transaction.TransactionDate = DateTime.UtcNow;
                transaction.ActiveStateID = ActiveStateConst.ACTIVE;
                var result = _context.Transaction
                            .Any(t => t.AccountID == transaction.AccountID
                             && t.WalletID == transaction.WalletID
                             && t.CategoryID == transaction.CategoryID
                             && t.TotalAmount == transaction.TotalAmount
                             && t.Note == transaction.Note
                             && t.TransactionDate == transaction.TransactionDate
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

        internal List<Transaction> GetTransactionsByMonth(string accountID, int month, int year)
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
                                    .ToList();
                if (result is null) throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);
                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    transaction.TransactionDate = LConvertVariable.ConvertUtcToLocalTime(transaction.TransactionDate);
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID);
                    if (cateType is null) throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    transaction.Category.CategoryType = cateType;
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Transaction> GetTransactionsByDay(string accountID, int day, int month, int year)
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
                                    .ToList();
                if (result is null) throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);
                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID);
                    if (cateType is null) throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    transaction.Category.CategoryType = cateType;
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Transaction> GetTransactionsByDateTimeRange(string accountID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var result = _context.Transaction
                                    .Where(t => t.AccountID == accountID
                                    && t.TransactionDate >= fromDate
                                    && t.TransactionDate <= toDate)
                                    .Include(t => t.ActiveState)
                                    .Include(t => t.Category)
                                    .Include(t => t.Wallet)
                                    .ToList();
                if (result is null) throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);
                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID);
                    if (cateType is null) throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
                    transaction.Category.CategoryType = cateType;
                }
                return result;
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
                            Transactions = new List<TransactionDetail_VM_DTO> { _mapper.Map<TransactionDetail_VM_DTO>(transaction) }
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
                var now = DateTime.UtcNow;
                var result = _context.Transaction
                            .Where(t => t.AccountID == accountID && t.TransactionDate <= now)
                            .Include(t => t.ActiveState)
                            .Include(t => t.Category)
                            .Include(t => t.Wallet)
                            .OrderByDescending(t => t.TransactionDate)
                            .Take(number)
                            .ToList();
                if (result is null) throw new Exception(Message.TRANSACTION_NOT_FOUND);
                var cateDA = new CategoryDA(_context);

                var cates = cateDA.GetCategoryTypes();
                foreach (var transaction in result)
                {
                    var cateType = cates.Find(c => c.CategoryTypeID == transaction.Category.CategoryTypeID);
                    if (cateType is null) throw new Exception(Message.CATEGORY_TYPE_NOT_FOUND);
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
                    if (transactionsDict.ContainsKey(dateonly))
                    {
                        if (tran.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                        {
                            transactionsDict[dateonly].TotalAmountIn += tran.TotalAmount;
                            transactionsDict[dateonly].NumberOfTransactionIn++;
                        }
                        else
                        {
                            transactionsDict[dateonly].TotalAmountOut += tran.TotalAmount;
                            transactionsDict[dateonly].NumberOfTransactionOut++;
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
                    if (transactionsDict.ContainsKey(dateonly))
                    {
                        // add transaction to list of transactions
                        transactionsDict[dateonly].Transactions.Add(_mapper.Map<TransactionInList_VM_DTO>(tran));
                        if (tran.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                        {
                            transactionsDict[dateonly].TotalAmountIn += tran.TotalAmount;
                            transactionsDict[dateonly].NumberOfTransactionIn++;
                        }
                        else
                        {
                            transactionsDict[dateonly].TotalAmountOut += tran.TotalAmount;
                            transactionsDict[dateonly].NumberOfTransactionOut++;
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
                    TransactionsByDay = new Dictionary<DateOnly, DayInByWeek>()
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
                    if (result.TransactionsByDay.ContainsKey(dateonly))
                    {
                        result.TransactionsByDay[dateonly].DayDetail = new DayDetail
                        {
                            DayOfWeek = dateonly.DayOfWeek,
                            Short_EN = dateonly.DayOfWeek.ToString().Substring(0, 3),
                            Full_EN = dateonly.DayOfWeek.ToString(),
                            Short_VN = LConvertVariable.ConvertDayInWeekToVN_SHORT_3(dateonly.DayOfWeek),
                            Full_VN = LConvertVariable.ConvertDayInWeekToVN_FULL(dateonly.DayOfWeek),
                            ShortDate = LConvertVariable.ConvertDateOnlyToVN_ng_thg(dateonly),
                            FullDate = LConvertVariable.ConvertDateOnlyToVN_ngay_thang(dateonly),
                        };
                        result.TransactionsByDay[dateonly].Transactions.Add(tran);
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
                            },
                            Transactions = new List<TransactionInList_VM_DTO> { tran }
                        });
                    }
                    if (transaction.Category.CategoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME)
                    {
                        result.TransactionsByDay[dateonly].TotalAmountIn += transaction.TotalAmount;
                        // LConvertVariable.ConvertToMoneyFormat
                        result.TransactionsByDay[dateonly].TotalAmountInStr = LConvertVariable.ConvertToMoneyFormat(result.TransactionsByDay[dateonly].TotalAmountIn);
                    }
                    else
                    {
                        result.TransactionsByDay[dateonly].TotalAmountOut += transaction.TotalAmount;
                        result.TransactionsByDay[dateonly].TotalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(result.TransactionsByDay[dateonly].TotalAmountOut);
                    }
                }
                result.TotalAmount = result.TotalAmountIn - result.TotalAmountOut;
                result.TransactionCount = result.NumberOfTransactionIn + result.NumberOfTransactionOut;
                result.TotalAmountStr = LConvertVariable.ConvertToMoneyFormat(result.TotalAmount);
                result.TotalAmountInStr = LConvertVariable.ConvertToMoneyFormat(result.TotalAmountIn);
                result.TotalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(result.TotalAmountOut);

                result.TransactionsByDay = result.TransactionsByDay.OrderByDescending(t => t.Key).ToDictionary(t => t.Key, t => t.Value);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            throw new NotImplementedException();
        }
    }
}
