using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.WalletF;
[Table("currency_type", Schema = "public")]
public class Currency
{
    // currency_id, currency_name, currency_country, currency_symbol
    [Column("currency_id")]
    public int CurrencyID { get; set; }

    [Column("currency_name")]
    public string Name { get; set; } = String.Empty;

    [Column("currency_country")]
    public string Country { get; set; } = String.Empty;

    [Column("currency_symbol")]
    public string Symbol { get; set; } = String.Empty;

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;

}

