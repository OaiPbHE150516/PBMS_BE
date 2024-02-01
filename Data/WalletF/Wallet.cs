using pbms_be.Data.Status;
using pbms_be.Data.WalletF;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.WalletF;
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

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;
}

