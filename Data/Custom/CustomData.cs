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

    [Column("total_amount")]
    public long TotalAmount { get; set; }

    [Column("transaction_count")]
    public int TransactionCount { get; set; }
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
