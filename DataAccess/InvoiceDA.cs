﻿using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Invo;
using pbms_be.Library;

namespace pbms_be.DataAccess
{
    public class InvoiceDA
    {
        private readonly PbmsDbContext _context;


        public InvoiceDA(PbmsDbContext context)
        {
            _context = context;
        }

        public bool IsCorrectJPGPNG(IFormFile file)
        {
            try
            {
                var result = file.ContentType != ConstantConfig.MINE_TYPE_PDF
                               && file.ContentType != ConstantConfig.MINE_TYPE_JPEG
                               && file.ContentType != ConstantConfig.MINE_TYPE_JPG
                               && file.ContentType != ConstantConfig.MINE_TYPE_PNG;
                return result;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Invoice CreateInvoice(Invoice invoice, int transactionID)
        {
            try
            {
                // if date is not in Universal Time, convert to Universal Time
                if (invoice.InvoiceDate.Kind != DateTimeKind.Utc)
                {
                    invoice.InvoiceDate = invoice.InvoiceDate.ToUniversalTime();
                }
                invoice.TransactionID = transactionID;
                invoice.ActiveStateID = ActiveStateConst.ACTIVE;
                invoice.CurrencyID = CurrencyConst.DEFAULT_CURRENCY_ID_VND;
                _context.Invoice.Add(invoice);
                _context.SaveChanges();
                return invoice;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<ProductInInvoice> CreateProduct(List<ProductInInvoice> listProductInInvoice, int invoiceID)
        {
            try
            {
                foreach (var productInInvoice in listProductInInvoice)
                {
                    productInInvoice.InvoiceID = invoiceID;
                    productInInvoice.ActiveStateID = ActiveStateConst.ACTIVE;
                }
                _context.ProductInInvoice.AddRange(listProductInInvoice);
                _context.SaveChanges();
                return listProductInInvoice;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        //public List<Invoice> GetInvoices(string accountID)
        //{
        //    TransactionDA transactionDA = new TransactionDA(_context);
        //    var transactions = transactionDA.GetTransactions(accountID);
        //    List<Invoice> invoices = new List<Invoice>();
        //    foreach (var transaction in transactions)
        //    {
        //        var invoice = GetInvoiceByTransaction(transaction.TransactionID);
        //        if (invoice != null)
        //        {
        //            invoices.Add(invoice);
        //        }
        //    }
        //    return invoices;
        //}

        //public Invoice? GetInvoiceByTransaction(int transactionID)
        //{
        //    var result = _context.Invoice
        //        .Where(i => i.TransactionID == transactionID)
        //        .Include(i => i.ActiveState)
        //        .FirstOrDefault();
        //    return result;
        //}

        //// get invoice by invoice id
        //public Invoice? GetInvoice(int invoiceID)
        //{
        //    var result = _context.Invoice
        //        .Where(i => i.InvoiceID == invoiceID)
        //        .Include(i => i.ActiveState)
        //        .FirstOrDefault();
        //    return result;
        //}
    }
}
