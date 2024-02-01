using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.Wallet;
[Table("wallet", Schema = "public")]

public class Wallet
{
    [Column("wallet_id")]
    public int WalletID { get; set; }

    [Column("account_id")]
    public string AccountID { get; set; } = String.Empty;

    [Column("wallet_name")]
    public string Name { get; set; } = String.Empty;

    [Column("balance")]
    public long Balance { get; set; }

    [Column("currency_id")]
    public int CurrencyID { get; set; }
    public virtual Currency Currency { get; set; } = null!;

    [Column("vision_status_id")]
    public int VisionStatusID { get; set; }
    public virtual VisionStatus VisionStatus { get; set; } = null!;

    [Column("create_time")]
    public DateTime CreateTime { get; set; }
}

