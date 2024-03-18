using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Balance;

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
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // create new balance history log
        public async Task<BalanceHistoryLog> CreateBalanceHistoryLog(BalanceHistoryLog balanceHistoryLog)
        {
            try
            {
                // check if balance history log is valid
                if (!await IsBalanceHistoryLogValid(balanceHistoryLog))
                {
                    throw new Exception(Message.BALANCE_HISTORY_LOG_INVALID);
                }
                _context.BalanceHistoryLogs.Add(balanceHistoryLog);
                await _context.SaveChangesAsync();
                return balanceHistoryLog;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // check if balance history log is valid
        public async Task<bool> IsBalanceHistoryLogValid(BalanceHistoryLog balanceHistoryLog)
        {
            try
            {
                var accountID = await _context.Account.FirstOrDefaultAsync(x => x.AccountID == balanceHistoryLog.AccountID) ?? throw new Exception(Message.ACCOUNT_NOT_FOUND);
                var wallet = await _context.Wallet.FirstOrDefaultAsync(x => x.WalletID == balanceHistoryLog.WalletID && x.AccountID == balanceHistoryLog.AccountID) ?? throw new Exception(Message.WALLET_NOT_FOUND);
                var transaction = await _context.Transaction.FirstOrDefaultAsync(x => x.TransactionID == balanceHistoryLog.TransactionID) ?? throw new Exception(Message.TRANSACTION_NOT_FOUND);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }   
    }
}
