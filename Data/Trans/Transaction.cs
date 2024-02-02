using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using pbms_be.Data.Filter;
using pbms_be.Data.Status;
using pbms_be.Data.WalletF;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace pbms_be.Data.Trans;
[Table("transaction", Schema = "public")]

public class Transaction
{
    //CREATE TABLE transaction(
    //transaction_id serial PRIMARY KEY,
    //account_id VARCHAR ( 100 ) NOT NULL,
    //wallet_id INT NOT NULL,
    //    category_id INT NOT NULL,
    //    total_amount BIGINT NOT NULL,
    //    note VARCHAR( 5000 ) NOT NULL,
    //    transaction_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    //    from_person VARCHAR( 100 ),
    //    to_person VARCHAR( 100 ),
    //    image_url VARCHAR( 500 ),
    //    as_id INT NOT NULL DEFAULT 1,
    //    FOREIGN KEY(account_id) REFERENCES account(account_id),
    //    FOREIGN KEY(wallet_id) REFERENCES wallet(wallet_id),
    //    FOREIGN KEY(category_id) REFERENCES category(category_id),
    //    FOREIGN KEY(as_id) REFERENCES active_state(as_id)
    //); 

    [Column("transaction_id")]
    public int TransactionID { get; set; }

    [Column("account_id")]
    public string AccountID { get; set; } = String.Empty;

    [Column("wallet_id")]
    public int WalletID { get; set; }

    public virtual Wallet Wallet { get; set; } = null!;

    [Column("category_id")]
    public int CategoryID { get; set; }

    public virtual Category Category { get; set; } = null!;

    [Column("total_amount")]
    public long TotalAmount { get; set; }

    [Column("note")]
    public string Note { get; set; } = String.Empty;

    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; }

    [Column("from_person")]
    public string FromPerson { get; set; } = String.Empty;

    [Column("to_person")]
    public string ToPerson { get; set; } = String.Empty;

    [Column("image_url")]
    public string ImageURL { get; set; } = String.Empty;

    [Column("as_id")]  
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;


}
