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
