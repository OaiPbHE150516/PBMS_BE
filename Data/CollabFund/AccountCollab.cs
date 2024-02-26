using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace pbms_be.Data.CollabFund;
[Table("collab_account", Schema = "public")]
public class AccountCollab
{
    /*
     *  CREATE TABLE collab_account (
        account_collab_id serial PRIMARY KEY,
        collabfund_id INT NOT NULL,
        account_id VARCHAR ( 100 ) NOT NULL,
        isFundholder BOOLEAN NOT NULL DEFAULT FALSE,
        as_id INT NOT NULL DEFAULT 1,
        last_time timestamp not null default current_timestamp,
        FOREIGN KEY (collabfund_id) REFERENCES collabfund (collabfund_id),
        FOREIGN KEY (account_id) REFERENCES account (account_id),
        FOREIGN KEY (as_id) REFERENCES active_state (as_id)
        );
     */

    [Column("account_collab_id")]
    [Key]
    public int AccountCollabID { get; set; }

    [Column("collabfund_id")]
    public int CollabFundID { get; set; }

    [Column("account_id")]
    public string AccountID { get; set; } = String.Empty;

    [Column("isfundholder")]
    public bool IsFundholder { get; set; }

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;

    [Column("last_time")]
    public DateTime LastTime { get; set; } = DateTime.UtcNow;
}
