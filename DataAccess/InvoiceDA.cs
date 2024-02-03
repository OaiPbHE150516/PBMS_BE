﻿using Microsoft.EntityFrameworkCore;
using pbms_be.Data;
using pbms_be.Data.Invo;

namespace pbms_be.DataAccess
{
    public class InvoiceDA
    {
        private readonly PbmsDbContext _context;

        public InvoiceDA(PbmsDbContext context)
        {
            _context = context;
        }

        public List<Invoice> GetInvoices(string accountID)
        {
            TransactionDA transactionDA = new TransactionDA(_context);
            var transactions = transactionDA.GetTransactions(accountID);
            List<Invoice> invoices = new List<Invoice>();
            foreach (var transaction in transactions)
            {
                var invoice = GetInvoiceByTransaction(transaction.TransactionID);
                if (invoice != null)
                {
                    invoices.Add(invoice);
                }
            }
            return invoices;
        }

        public Invoice? GetInvoiceByTransaction(int transactionID)
        {
            var result = _context.Invoice
                .Where(i => i.TransactionID == transactionID)
                .Include(i => i.ActiveState)
                .FirstOrDefault();
            return result;
        }

        // get invoice by invoice id
        public Invoice? GetInvoice(int invoiceID)
        {
            var result = _context.Invoice
                .Where(i => i.InvoiceID == invoiceID)
                .Include(i => i.ActiveState)
                .FirstOrDefault();
            return result;
        }
    }
}