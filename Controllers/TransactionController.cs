﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Invo;
using pbms_be.Data.Trans;
using pbms_be.DataAccess;
using pbms_be.DTOs;
using pbms_be.Library;
using System.Transactions;

namespace pbms_be.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private TransactionDA _transactionDA;

        public TransactionController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _transactionDA = new TransactionDA(_context);
        }

        // get all transaction by account id
        [HttpGet("get/{accountID}/{pageNumber}/{pageSize}")]
        public IActionResult GetTransactions(string accountID, int pageNumber, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (pageNumber <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.PAGE_NUMBER_REQUIRED);
                if (pageSize <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.PAGE_SIZE_REQUIRED);
                var result = _transactionDA.GetTransactions(accountID, pageNumber, pageSize, ConstantConfig.DESCENDING_SORT);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                // calculate total page
                var totalPage = _transactionDA.GetTotalPage(accountID, pageSize);
                return Ok(new { pageNumber, totalPage, pageSize, ConstantConfig.DESCENDING_SORT, resultDTO });
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get transaction by id
        [HttpGet("get/{transactionID}/{accountID}")]
        public IActionResult GetTransaction(int transactionID, string accountID)
        {
            try
            {
                if (transactionID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.TRANSACTION_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _transactionDA.GetTransaction(transactionID, accountID, _mapper);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get transactions by date time
        [HttpGet("get/bymonth/{accountID}/{month}/{year}")]
        public IActionResult GetTransactionsByMonth(string accountID, int month, int year)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (month <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.MONTH_REQUIRED);
                if (year <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.YEAR_REQUIRED);
                var result = _transactionDA.GetTransactionsByMonth(accountID, month, year);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get transactions by day
        [HttpGet("get/byday/{accountID}/{day}/{month}/{year}")]
        public IActionResult GetTransactionsByDay(string accountID, int day, int month, int year)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (day <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.DAY_REQUIRED);
                if (month <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.MONTH_REQUIRED);
                if (year <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.YEAR_REQUIRED);
                var result = _transactionDA.GetTransactionsByDay(accountID, day, month, year);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get transactions by date time range
        [HttpGet("get/bydaterange/{accountID}/{fromDate}/{toDate}")]
        public IActionResult GetTransactionsByDateTimeRange(string accountID, DateOnly fromDate, DateOnly toDate)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                // check if fromDate is greater than toDate
                if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);
                var fromDateTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
                var toDateTime = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                var result = _transactionDA.GetTransactionsByDateTimeRange(accountID, fromDateTime, toDateTime);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get transactions by month
        [HttpGet("get/calendar/{accountID}/{month}/{year}")]
        public IActionResult GetTransactionsByMonthCalendar(string accountID, int month, int year)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (month <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.MONTH_REQUIRED);
                if (year <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.YEAR_REQUIRED);
                var result = _transactionDA.GetTransactionsByMonthCalendar(accountID, month, year, _mapper);
                //if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                //var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get rectly transactions by account id and number of transactions
        [HttpGet("get/recently/{accountID}/{number}")]
        public IActionResult GetRecentlyTransactions(string accountID, int number)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (number <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.NUMBER_REQUIRED);
                var result = _transactionDA.GetRecentlyTransactions(accountID, number);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get transactions day by day by account id, from date, to date
        [HttpGet("get/daybyday/custom/{accountID}/{fromDateStr}/{toDateStr}")]
        public IActionResult GetTransactionsDayByDay(string accountID, string fromDateStr, string toDateStr)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (string.IsNullOrEmpty(fromDateStr)) return BadRequest(Message.FROM_DATE_REQUIRED);
                if (string.IsNullOrEmpty(toDateStr)) return BadRequest(Message.TO_DATE_REQUIRED);
                // convert string to date by format like "2/12/2021"
                var fromDate = DateTime.ParseExact(fromDateStr, ConstantConfig.DEFAULT_DATE_FORMAT_DASH, null);
                var toDate = DateTime.ParseExact(toDateStr, ConstantConfig.DEFAULT_DATE_FORMAT_DASH, null);
                // check if fromDate is greater than toDate
                if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);
                // from dateTime with utc time kind
                var fromDateTime = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
                var toDateTime = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
                var result = _transactionDA.GetTransactionsDayByDay(accountID, fromDateTime, toDateTime, _mapper);
                //if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                //var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get transactions day by day by account id, from date, to date
        [HttpGet("get/daybyday/last7/{accountID}")]
        public IActionResult GetTransactionsDayByDayCustom(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var now = DateTime.UtcNow;
                //  DateOnly fromDateTime = 7 days ago, toDateTime = now
                var fromDateTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-7).ToUniversalTime();
                var toDateTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).ToUniversalTime();
                var result = _transactionDA.GetTransactionsDayByDay(accountID, fromDateTime, toDateTime, _mapper);
                //if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                //var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        // get trasactions week by week by account id, from date string, to date string
        [HttpGet("get/weekbyweek/custom/{accountID}/{fromDateStr}/{toDateStr}")]
        public IActionResult GetTransactionsWeekByWeekCustom(string accountID, string fromDateStr, string toDateStr)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (string.IsNullOrEmpty(fromDateStr)) return BadRequest(Message.FROM_DATE_REQUIRED);
                if (string.IsNullOrEmpty(toDateStr)) return BadRequest(Message.TO_DATE_REQUIRED);

                var fromDateArr = fromDateStr.Split("-");
                var toDateArr = toDateStr.Split("-");

                var fromDate = new DateTime(int.Parse(fromDateArr[2]), int.Parse(fromDateArr[1]), int.Parse(fromDateArr[0]), 0, 0, 0);
                var toDate = new DateTime(int.Parse(toDateArr[2]), int.Parse(toDateArr[1]), int.Parse(toDateArr[0]), 23, 59, 59);
                if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);


                //var fromDate = DateTime.ParseExact(fromDateStr, ConstantConfig.DEFAULT_DATE_FORMAT_DASH, null);
                //var toDate = DateTime.ParseExact(toDateStr, ConstantConfig.DEFAULT_DATE_FORMAT_DASH, null);
                //if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);

                var result = _transactionDA.GetTransactionsWeekByWeek(accountID, fromDate, toDate, _mapper);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get/raw/data/{accountID}/{fromDateStr}/{toDateStr}")]
        public IActionResult GetRawData(string accountID, string fromDateStr, string toDateStr)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (string.IsNullOrEmpty(fromDateStr)) return BadRequest(Message.FROM_DATE_REQUIRED);
                if (string.IsNullOrEmpty(toDateStr)) return BadRequest(Message.TO_DATE_REQUIRED);

                var fromDate = DateTime.ParseExact(fromDateStr, ConstantConfig.DEFAULT_DATE_FORMAT_DASH, null);
                var toDate = DateTime.ParseExact(toDateStr, ConstantConfig.DEFAULT_DATE_FORMAT_DASH, null);
                if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);

                var result = _transactionDA.GetTransactionsByDateTimeRange(accountID, fromDate, toDate);
                // sort result by date
                result = [.. result.OrderByDescending(x => x.TransactionDate)];
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpGet("get/daybyday/specific/{accountID}/{datetimestr}")]
        public IActionResult GetTransactionsDayByDaySpecific(string accountID, string datetimestr)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (string.IsNullOrEmpty(datetimestr)) return BadRequest(Message.DATE_TIME_REQUIRED);
                var datetime = DateTime.ParseExact(datetimestr, ConstantConfig.DEFAULT_DATE_FORMAT_DASH, null);
                var fromDateTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0).ToUniversalTime();
                var toDateTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, 23, 59, 59).ToUniversalTime();
                var result = _transactionDA.GetTransactionsDayByDay(accountID, fromDateTime, toDateTime, _mapper);
                //if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                //var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get total amount in and out by account id in month
        [HttpGet("get/totalamount/{accountID}/{month}/{year}")]
        public IActionResult GetTotalAmountInAndOutByMonth(string accountID, int month, int year)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (month <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.MONTH_REQUIRED);
                if (year <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.YEAR_REQUIRED);
                var result = _transactionDA.GetTotalAmountInAndOutByMonth(accountID, month, year);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get total amount in and out by account id in last (number) days
        [HttpGet("get/totalamount/last7days/{accountID}")]
        public IActionResult GetTotalAmountInAndOutByLastDays(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _transactionDA.GetTotalAmountInAndOutByLastDays(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get expenses by account id in last (number) days
        [HttpGet("get/expenses/lastnumdays/{accountID}/{numdays}")]
        public IActionResult GetExpensesByLastDays(string accountID, int numdays)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (numdays <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.NUMBER_REQUIRED);
                var result = _transactionDA.GetExpensesByLastDays(accountID, numdays, _mapper);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get total amount of all categories by account id in month
        [HttpGet("get/filter/category/{month}/{year}/{accountID}")]
        public IActionResult getAllTransactionFilterCategory(int month, int year, string accountID)
        {
            try
            {
                if (month <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.MONTH_REQUIRED);
                if (year <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.YEAR_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _transactionDA.GetAllTransactionFilterCategory(month, year, accountID, _mapper);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get total amount of all categories by account id in month
        [HttpGet("get/filter/type/{month}/{year}/{accountID}")]
        public IActionResult getAllTransactionFilterType(int month, int year, string accountID)
        {
            try
            {
                if (month <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.MONTH_REQUIRED);
                if (year <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.YEAR_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _transactionDA.GetAllTransactionFilterType(month, year, accountID, _mapper);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #region Post Methods

        // create transaction with invoice and products
        [HttpPost("create")]
        public IActionResult CreateTransaction([FromBody] TransactionCreateDTO transactionDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var transaction = _mapper.Map<Data.Trans.Transaction>(transactionDTO);
                if (_transactionDA.IsTransactionExist(transaction)) return BadRequest(Message.TRANSACTION_EXISTED);
                var invoice = new Invoice
                {
                    IDOfInvoice = transactionDTO.Invoice.IDOfInvoice,
                    InvoiceDate = LConvertVariable.ConvertStringToDateTime(transactionDTO.Invoice.InvoiceDate),
                    TotalAmount = transactionDTO.Invoice.TotalAmount,
                    NetAmount = transactionDTO.Invoice.NetAmount,
                    TaxAmount = transactionDTO.Invoice.TaxAmount,
                    SupplierAddress = transactionDTO.Invoice.SupplierAddress,
                    SupplierName = transactionDTO.Invoice.SupplierName,
                    SupplierPhone = transactionDTO.Invoice.SupplierPhone,
                    InvoiceImageURL = transactionDTO.Invoice.InvoiceImageURL,
                };
                //var transDate = DateTime.UtcNow;
                //var imageurl = "https://storage.googleapis.com/pbms-user/invoice/"+ transactionDTO.AccountID + "/" + "invoice_" + transactionDTO.ImageURL + "_file.jpg";
                var resultTransaction = _transactionDA.CreateTransactionV2(transaction, transactionDTO.TransactionDate);
                var invoiceDA = new InvoiceDA(_context);
                var resultInvoice = invoiceDA.CreateInvoice(invoice, resultTransaction.TransactionID);
                if (resultTransaction is null || resultInvoice is null) return BadRequest(Message.TRANSACTION_CREATE_FAILED);
                var listProductInInvoice = _mapper.Map<List<ProductInInvoice>>(transactionDTO.Invoice.Products);
                var resultProduct = invoiceDA.CreateProduct(listProductInInvoice, resultInvoice.InvoiceID);
                // return 3 results
                return Ok(new { resultTransaction, resultInvoice, resultProduct });
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // create transaction without invoice and products
        [HttpPost("create/withoutinvoice")]
        public IActionResult CreateTransactionWithoutInvoice([FromBody] TransactionWithoutInvoiceCreateDTO transactionDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var transaction = _mapper.Map<Data.Trans.Transaction>(transactionDTO);
                if (_transactionDA.IsTransactionExist(transaction)) return BadRequest(Message.TRANSACTION_EXISTED);
                var result = _transactionDA.CreateTransactionV2(transaction, transactionDTO.TransactionDate);
                if (result is null) return BadRequest(Message.TRANSACTION_CREATE_FAILED);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //// create transaction with invoice and products, have image of them
        //[HttpPost("create/withimage")]
        //public async Task<IActionResult> CreateTransactionWithImage([FromForm] TransactionCreateWithImageDTO transactionDTO)
        //{
        //    try
        //    {
        //        return Ok(transactionDTO.AccountID);
        //        //if (!ModelState.IsValid) return BadRequest(ModelState);
        //        //if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
        //        //if (LValidation.IsCorrectPDFJPGPNG(image)) return BadRequest(Message.FILE_IS_NOT_JPG_PNG);
        //        //var transaction = _mapper.Map<Data.Trans.Transaction>(transactionDTO);
        //        //if (_transactionDA.IsTransactionExist(transaction)) return BadRequest(Message.TRANSACTION_EXISTED);
        //        //var result = _transactionDA.CreateTransactionWithImage(transaction, transactionDTO, image);
        //        //if (result is null) return BadRequest(Message.TRANSACTION_CREATE_FAILED);
        //        //return Ok(result);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        #endregion
    }
}
