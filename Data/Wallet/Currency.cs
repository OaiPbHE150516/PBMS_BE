using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.Wallet;
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

    [Column("vision_status_id")]
    public int VisionStatusID { get; set; }

    public virtual VisionStatus VisionStatus { get; set; } = null!;
}

