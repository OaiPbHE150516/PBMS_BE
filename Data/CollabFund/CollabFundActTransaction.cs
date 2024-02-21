using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.CollabFund;
[Table("collab_fund_act_transaction", Schema = "public")]

public class CollabFundActTransaction
{
    /*
        CREATE TABLE collab_fund_act_transaction (
            collab_fund_act_transaction_id serial PRIMARY KEY,
            collab_fun_activity_id INT NOT NULL,
            transaction_id INT NOT NULL,
            as_id INT NOT NULL DEFAULT 1,
            FOREIGN KEY (collab_fun_activity_id) REFERENCES collab_fun_activity (collab_fun_activity_id),
            FOREIGN KEY (transaction_id) REFERENCES transaction (transaction_id),
            FOREIGN KEY (as_id) REFERENCES active_state (as_id)
        );
     */

    [Column("collab_fund_act_transaction_id")]
    [Key]
    public int CollabFundActTransactionID { get; set; }

    [Column("collab_fun_activity_id")]
    public int CollabFundActivityID { get; set; }

    [Column("transaction_id")]
    public int TransactionID { get; set; }

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;

}
