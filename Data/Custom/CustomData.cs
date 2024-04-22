﻿using pbms_be.Data.Auth;
using pbms_be.Data.Balance;
using pbms_be.Data.CollabFund;
using pbms_be.Data.Filter;
using pbms_be.Data.LogModel;
using pbms_be.DTOs;
using pbms_be.Library;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.Custom;

public class DivideMoneyInfo
{
    // account_id, total_amount, transaction_count
    [Column("account_id")]
    [Key]
    public string AccountID { get; set; } = String.Empty;
    //public virtual Account Account { get; set; } = null!;

    [Column("total_amount")]
    public long TotalAmount { get; set; }

    [Column("transaction_count")]
    public int TransactionCount { get; set; }

}

public class DivideMoneyInfoWithAccount : DivideMoneyInfo
{
    public virtual Account Account { get; set; } = null!;
    public string TotalAmountStr { get; set; } = String.Empty;
    public long RemainAmount { get; set; }
    public string RemainAmountStr { get; set; } = String.Empty;
    public string MoneyActionStr { get; set; } = String.Empty;
}

public class CF_DivideMoney_DTO_VM : CF_DividingMoney
{
    public string TotalAmountStr { get; set; } = String.Empty;
    public string AverageAmountStr { get; set; } = String.Empty;
    public string RemainAmountStr { get; set; } = String.Empty;
    public virtual DayDetail CreateTimeDetail { get; set; } = null!;
    public virtual List<CF_DividingMoneyDetail_DTO_VM> List_CFDM_Detail_VM_DTO { get; set; } = null!;
}

public class CF_DividingMoneyDetail_DTO_VM : CF_DividingMoneyDetail
{
    public string FromAccountTotalAmountStr { get; set; } = String.Empty;
    public string DividingAmountStr { get; set; } = String.Empty;
}

public class DivideMoneyExecute
{
    // from_account_id, to_account_id, dividing_amount
    [Column("from_account_id")]
    [Key]
    public string FromAccountID { get; set; } = String.Empty;

    [Column("to_account_id")]
    [Key]
    public string ToAccountID { get; set; } = String.Empty;

    [Column("actual_amount")]
    public long ActualAmount { get; set; }

    [Column("remain_amount")]
    public long RemainAmount { get; set; }

    public int State { get; set; }
}

public class GenerateDefaultCategory
{
    public string NameVN { get; set; } = String.Empty;
    public string NameEN { get; set; } = String.Empty;

    public string ParentNameVN { get; set; } = String.Empty;
    public string ParentNameEN { get; set; } = String.Empty;
}

public class TransactionInDayCalendar
{
    public DateOnly Date { get; set; }
    public bool isHasTransaction { get; set; } = false;
    public long TotalAmount { get; set; }
    public string TotalAmountStr { get; set; } = String.Empty;
    public bool isHasTransactionIn { get; set; } = false;
    public long TotalAmountIn { get; set; } = 0;
    public string TotalAmountInStr { get; set; } = String.Empty;
    public bool isHasTransactionOut { get; set; } = false;
    public long TotalAmountOut { get; set; } = 0;
    public string TotalAmountOutStr { get; set; } = String.Empty;
    public int TransactionCount { get; set; } = 0;
    public virtual List<TransactionDetail_VM_DTO> Transactions { get; set; } = null!;
}

public class TransactionInLastDays
{
    public DayDetail DayDetail { get; set; } = null!;
    public int NumberOfTransactionIn { get; set; } = 0;
    public long TotalAmountIn { get; set; } = 0;
    public string TotalAmountInStr { get; set; } = String.Empty;
    public int NumberOfTransactionOut { get; set; } = 0;
    public long TotalAmountOut { get; set; } = 0;
    public string TotalAmountOutStr { get; set; } = String.Empty;
    public int TransactionCount { get; set; } = 0;
    public long TotalAmount { get; set; } = 0;
    public string TotalAmountStr { get; set; } = String.Empty;
}

public class TransactionDayByDay
{
    public DayDetail DayDetail { get; set; } = null!;
    public int NumberOfTransactionIn { get; set; } = 0;
    public long TotalAmountIn { get; set; } = 0;
    public string TotalAmountInStr { get; set; } = String.Empty;
    public int NumberOfTransactionOut { get; set; } = 0;
    public long TotalAmountOut { get; set; } = 0;
    public string TotalAmountOutStr { get; set; } = String.Empty;
    public int TransactionCount { get; set; } = 0;
    public long TotalAmount { get; set; } = 0;
    public string TotalAmountStr { get; set; } = String.Empty;
    public virtual List<TransactionInList_VM_DTO> Transactions { get; set; } = null!;
}

public class TransactionWeekByWeek
{
    public WeekDetail WeekDetail { get; set; } = null!;
    public int NumberOfTransactionIn { get; set; } = 0;
    public long TotalAmountIn { get; set; } = 0;
    public string TotalAmountInStr { get; set; } = String.Empty;
    public int NumberOfTransactionOut { get; set; } = 0;
    public long TotalAmountOut { get; set; } = 0;
    public string TotalAmountOutStr { get; set; } = String.Empty;
    public int TransactionCount { get; set; } = 0;
    public long TotalAmount { get; set; } = 0;
    public string TotalAmountStr { get; set; } = String.Empty;
    public virtual Dictionary<DateOnly, DayInByWeek> TransactionsByDay { get; set; } = null!;
}

public class TransactionWeekByWeek2
{
    public WeekDetail WeekDetail { get; set; } = null!;
    public int NumberOfTransactionIn { get; set; } = 0;
    public long TotalAmountIn { get; set; } = 0;
    public string TotalAmountInStr { get; set; } = String.Empty;
    public int NumberOfTransactionOut { get; set; } = 0;
    public long TotalAmountOut { get; set; } = 0;
    public string TotalAmountOutStr { get; set; } = String.Empty;
    public int TransactionCount { get; set; } = 0;
    public long TotalAmount { get; set; } = 0;
    public string TotalAmountStr { get; set; } = String.Empty;
    public virtual List<DayInByWeek> TransactionByDayW { get; set; } = null!;
}

public class DayInByWeek
{
    public DayDetail DayDetail { get; set; } = null!;
    public long TotalAmountIn { get; set; } = 0;
    public string TotalAmountInStr { get; set; } = String.Empty;
    public long TotalAmountOut { get; set; } = 0;
    public string TotalAmountOutStr { get; set; } = String.Empty;
    public long TotalAmount { get; set; } = 0;
    public string TotalAmountStr { get; set; } = String.Empty;
    public virtual List<TransactionInList_VM_DTO> Transactions { get; set; } = null!;
}

public class DayDetail
{
    public DayOfWeek DayOfWeek { get; set; }
    public string Short_EN { get; set; } = String.Empty;
    public string Full_EN { get; set; } = String.Empty;
    public string Short_VN { get; set; } = String.Empty;
    public string Full_VN { get; set; } = String.Empty;
    public string ShortDate { get; set; } = String.Empty;
    public string FullDate { get; set; } = String.Empty;

    public string DayStr { get; set; } = String.Empty;
    public string MonthYearStr { get; set; } = String.Empty;

    public DateOnly Date { get; set; }
}

public class WeekDetail
{
    public DateOnly StartDate { get; set; }
    public string StartDateStrShort { get; set; } = String.Empty;
    public string StartDateStrFull { get; set; } = String.Empty;
    public string DayOfWeekStartStr { get; set; } = String.Empty;
    public DateOnly EndDate { get; set; }
    public string EndDateStrShort { get; set; } = String.Empty;
    public string EndDateStrFull { get; set; } = String.Empty;
    public string DayOfWeekEndStr { get; set; } = String.Empty;
}

public class GenerateRandomTransactions
{
    public string AccountID { get; set; } = String.Empty;
    [Range(1, 1111)]
    public int numberOfTransactions { get; set; } = 1;
    public string randomString { get; set; } = String.Empty;
    [Range(1, 100)]
    public int minStringLength { get; set; } = 10;
    [Range(1, 100)]
    public int maxStringLength { get; set; } = 30;

    [Range(1, 28)]
    public int minDay { get; set; } = 1;
    [Range(1, 12)]
    public int minMonth { get; set; } = 1;
    public int minYear { get; set; } = 2001;
    [Range(1, 28)]
    public int maxDay { get; set; } = 5;
    [Range(1, 28)]
    public int maxMonth { get; set; } = 3;
    public int maxYear { get; set; } = 2024;
    [Range(1000, 1000000)]
    public long minAmount { get; set; } = 1000;
    [Range(1000, 100000000)]
    public long maxAmount { get; set; } = 5000000;
    public bool isRoundAmount { get; set; } = true;

}

public class CustomBalanceHisLogByDate
{
    public DateOnly Date { get; set; }
    public long TotalAmount { get; set; }
    public string TotalAmountStr { get; set; } = String.Empty;
    public int TransactionCount { get; set; }
    public virtual List<BalanceHisLog_VM_DTO> BalanceHistoryLogs { get; set; } = null!;

}

public class ADateOnly
{
    public DateOnly Date { get; set; }
}

public class KeyExtractor
{
    public string Key { get; set; } = String.Empty;
}

public class FileWithAccountID
{
    public string AccountID { get; set; } = String.Empty;
    public IFormFile File { get; set; } = null!;
    public string FileName { get; set; } = String.Empty;
}

public class FileWithTextPrompt
{
    public IFormFile File { get; set; } = null!;
    public string TextPrompt { get; set; } = String.Empty;
}

public class AccountIDWithFile
{
    public string AccountID { get; set; } = String.Empty;
    public IFormFile File { get; set; } = null!;
}

public class InvoiceCustom_VM_Scan
{
    public string SupplierAddress { get; set; } = String.Empty;
    public string SupplierPhone { get; set; } = String.Empty;
    public string SupplierName { get; set; } = String.Empty;
    public long TotalAmount { get; set; }
    public long NetAmount { get; set; }
    public long TaxAmount { get; set; }
    public string IDOfInvoice { get; set; } = String.Empty;
    public string InvoiceDate { get; set; } = String.Empty;
    public List<ProductInInvoice_VM_Scan> ProductInInvoices { get; set; } = null!;
}

public class ProductInInvoice_VM_Scan
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = String.Empty;
    public int Quanity { get; set; } = 1;
    public long UnitPrice { get; set; } = 1;
    public long TotalAmount { get; set; }
    public string Tag { get; set; } = String.Empty;
}

public class MoneyInvoice
{
    public long NetAmount { get; set; }
    public long TaxAmount { get; set; }
    public long TotalAmount { get; set; }
}

//public class CategoryWithAllTransaction
//{
//    public int CategoryID { get; set; }
//    public string NameVN { get; set; } = String.Empty;
//    public string NameEN { get; set; } = String.Empty;
//    public long TotalAmount { get; set; }
//    public string TotalAmountStr { get; set; } = String.Empty;
//    public int NumberOfTransaction { get; set; }
//    //public virtual List<TransactionDetail_VM_DTO> Transactions { get; set; } = null!;
//}

public class CategoryWithTransactionData
{
    public int CategoryNumber { get; set; }
    public CategoryDetail_VM_DTO Category { get; set; } = null!;
    public long TotalAmount { get; set; }
    public string TotalAmountStr { get; set; } = String.Empty;
    public int NumberOfTransaction { get; set; }
    public double Percentage { get; set; }
    public string PercentageStr { get; set; } = String.Empty;
}

public class CategoryWithTransactionData2
{
    public int CategoryTypeNumber { get; set; }
    public CategoryType CategoryType { get; set; } = null!;
    public long TotalAmount { get; set; }
    public string TotalAmountStr { get; set; } = String.Empty;
    public int NumberOfTransaction { get; set; }
    public double Percentage { get; set; }
    public string PercentageStr { get; set; } = String.Empty;

}

public class LogWithDetailAccount
{
    public int ID { get; set; }
    public int NumberOfLogs { get; set; }
    public string AccountID { get; set; } = String.Empty;
    public virtual Account? Account { get; set; } = null!;
    public virtual List<ScanLog> ScanLogs { get; set; } = null!;
}

public class LogWithDayDetail
{
    public int ID { get; set; }
    public int NumberOfLogs { get; set; }
    public DateOnly Date { get; set; }
    public virtual List<ScanLog> ScanLogs { get; set; } = null!;
}

public class TagWithProductData
{
    public int TagNumber { get; set; }
    public TagDetail_VM_DTO Tag { get; set; } = null!;
    public List<TagWithTotalAmount> TagWithTotalAmounts { get; set; } = null!;
    public long TotalAmount { get; set; }
    public string TotalAmountStr { get; set; } = String.Empty;
    public int NumberOfProduct { get; set; }
    public double Percentage { get; set; }
    public string PercentageStr { get; set; } = String.Empty;
}

public class TagDetail_VM_DTO
{
    public string PrimaryTag { get; set; } = String.Empty;
    public virtual List<string> ChildTags { get; set; } = null!;
}

public class TagWithTotalAmount
{
    public int TagNumber { get; set; }
    public string Tag { get; set; } = String.Empty;
    public int NumberOfProduct { get; set; }
    public long TotalAmount { get; set; }
    public string TotalAmountStr { get; set; } = String.Empty;
}