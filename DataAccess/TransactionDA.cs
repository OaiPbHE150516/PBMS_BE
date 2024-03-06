using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Invo;
using pbms_be.Data.Trans;
using pbms_be.DTOs;

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
                foreach (var transaction in result)
                {
                    transaction.Category.CategoryType = cateDA.GetCategoryType(transaction.Category.CategoryTypeID);
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

        internal object GetTransactionsByDateTime(string accountID, int month, int year)
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
                foreach (var transaction in result)
                {
                    transaction.Category.CategoryType = cateDA.GetCategoryType(transaction.Category.CategoryTypeID);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
