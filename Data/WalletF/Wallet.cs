using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using pbms_be.Data.Status;
using pbms_be.Data.WalletF;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace pbms_be.Data.WalletF;
[Table("wallet", Schema = "public")]

public class Wallet
{
    //CREATE TABLE wallet(
    //    wallet_id serial PRIMARY KEY,
    //account_id VARCHAR ( 100 ) NOT NULL,
    //    wallet_name VARCHAR( 100 ) NOT NULL,
    //    balance BIGINT NOT NULL DEFAULT 0,
    //    currency_id INT NOT NULL,
    //    as_id INT NOT NULL DEFAULT 1,
    //    create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    //    FOREIGN KEY(account_id) REFERENCES account(account_id),
    //    FOREIGN KEY(currency_id) REFERENCES currency_type(currency_id),
    //    FOREIGN KEY(as_id) REFERENCES active_state(as_id)
    //);
    [Column("wallet_id")]
    [Key]
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

