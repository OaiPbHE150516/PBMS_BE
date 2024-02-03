﻿using Microsoft.EntityFrameworkCore;
using pbms_be.Data;
using pbms_be.Data.Trans;

namespace pbms_be.DataAccess
{
    public class TransactionDA
    {
        private readonly PbmsDbContext _context;
        public TransactionDA(PbmsDbContext context)
        {
            _context = context;
        }

        public List<Transaction> GetTransactions(string AccountID)
        {
            var result = _context.Transaction
                .Where(t => t.AccountID == AccountID)
                .Include(t => t.ActiveState)
                //.Include(t => t.Category)
                //.Include(t => t.Wallet)
                .ToList();
            return result;
        }

        public Transaction? GetTransaction(int TransactionID)
        {
            var result = _context.Transaction
                .Where(t => t.TransactionID == TransactionID)
                .Include(t => t.ActiveState)
                .Include(t => t.Category)
                .Include(t => t.Wallet)
                .FirstOrDefault();
            return result;
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

        
    }
}