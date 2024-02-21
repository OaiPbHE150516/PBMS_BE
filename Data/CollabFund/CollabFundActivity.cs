using pbms_be.Data.Status;
using pbms_be.Data.Trans;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace pbms_be.Data.CollabFund;
[Table("collab_fun_activity", Schema = "public")]

public class CollabFundActivity
{
    /*
      CREATE TABLE collab_fun_activity (
            collab_fun_activity_id serial PRIMARY KEY,
            collabfund_id INT NOT NULL,
            account_id VARCHAR ( 100 ) NOT NULL,
            note VARCHAR ( 5000 ) NOT NULL DEFAULT '',
            create_time timestamp not null default current_timestamp,
            as_id INT NOT NULL DEFAULT 1,
            FOREIGN KEY (collabfund_id) REFERENCES collabfund (collabfund_id),
            FOREIGN KEY (account_id) REFERENCES account (account_id),
            FOREIGN KEY (as_id) REFERENCES active_state (as_id)
        );
     */

    [Column("collab_fun_activity_id")]
    [Key]
    public int CollabFundActivityID { get; set; }

    [Column("collabfund_id")]
    public int CollabFundID { get; set; }

    [Column("account_id")]
    public string AccountID { get; set; } = String.Empty;

    [Column("note")]
    public string Note { get; set; } = String.Empty;

    [Column("create_time")]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;

    public virtual ICollection<CollabFundActTransaction> CollabFundActTransactions { get; set; } = null!;

    // list transaction
    public virtual ICollection<Transaction> Transactions { get; set; } = null!;
}
