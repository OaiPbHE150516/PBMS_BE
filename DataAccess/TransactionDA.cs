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

                // get transactions by account id with paging and sorting ascending or descending by using Skip and Take
                int skip = (pageNumber - 1) * pageSize;
                var result = _context.Transaction
                            .Where(t => t.AccountID == AccountID)
                            .Include(t => t.ActiveState)
                            .Include(t => t.Category)
                            .Include(t => t.Wallet)
                            .OrderBy(t => t.TransactionID)
                            .Skip(skip)
                            .Take(pageSize)
                            .ToList();
                sortType = sortType ?? ConstantConfig.ASCENDING_SORT;
                switch (sortType.ToLower())
                {
                    case ConstantConfig.ASCENDING_SORT:
                        result = result.OrderBy(t => t.TransactionDate).ToList();
                        break;
                    case ConstantConfig.DESCENDING_SORT:
                        result = result.OrderByDescending(t => t.TransactionDate).ToList();
                        break;
                    default:
                        result = result.OrderBy(t => t.TransactionDate).ToList();
                        break;
                }
                //var result = _context.Transaction
                //            .Where(t => t.AccountID == AccountID)
                //            .Include(t => t.ActiveState)
                //            .Include(t => t.Category)
                //            .Include(t => t.Wallet)
                //            .ToList();
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
                var result = _context.Transaction
                            .Where(t => t.AccountID == accountID)
                            .Include(t => t.ActiveState)
                            .Include(t => t.Category)
                            .Include(t => t.Wallet)
                            .OrderByDescending(t => t.TransactionDate)
                            .Take(number)
                            .ToList();
                if (result is null) throw new Exception(Message.TRANSACTION_NOT_FOUND);
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
                var transactionsDict = new Dictionary<DateOnly, object>();
                var transactions = GetTransactionsByDateTimeRange(accountID, DateTime.UtcNow.AddDays(-ConstantConfig.NUMBER_LAST_DAYS), DateTime.UtcNow);
                for (int i = 0; i < ConstantConfig.NUMBER_LAST_DAYS; i++)
                {
                    var date = DateTime.UtcNow.Date.AddDays(-i);
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
                    var dateonly = new DateOnly(date.Year, date.Month, date.Day);
                    transactionsDict.Add(dateonly, new
                    {
                        dayInWeekNum = date.DayOfWeek,
                        dayInWeekStr = new
                        {
                            short_EN = date.DayOfWeek.ToString().Substring(0, 3),
                            full_EN = date.DayOfWeek.ToString(),
                            short_VN = LConvertVariable.ConvertDayInWeekToVN_SHORT_3(date.DayOfWeek),
                            full_VN = LConvertVariable.ConvertDayInWeekToVN_FULL(date.DayOfWeek),
                            shortDate = LConvertVariable.ConvertDateOnlyToVN_ng_thg(dateonly),
                            fullDate = LConvertVariable.ConvertDateOnlyToVN_ngay_thang(dateonly),

                        },
                        numberTransIn,
                        totalAmountIn,
                        totalAmountInStr = LConvertVariable.ConvertToMoneyFormat(totalAmountIn),
                        numberTransOut,
                        totalAmountOut,
                        totalAmountOutStr = LConvertVariable.ConvertToMoneyFormat(totalAmountOut),
                        totalAmount,
                        totalNumberTrans = numberTransIn + numberTransOut,
                        totalAmountStr = LConvertVariable.ConvertToMoneyFormat(totalAmount)
                    });
                }
                transactionsDict = transactionsDict.OrderByDescending(t => t.Key).ToDictionary(t => t.Key, t => t.Value);
                return transactionsDict;

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
    }
}
