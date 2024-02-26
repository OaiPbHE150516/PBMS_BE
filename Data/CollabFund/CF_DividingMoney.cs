using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.CollabFund;
[Table("cf_dividing_money", Schema = "public")]

public class CF_DividingMoney
{
    /*
        CREATE TABLE cf_dividing_money (
            cf_dividing_money_id serial PRIMARY KEY,
            collabfund_id INT NOT NULL,
            collab_fun_activity_id INT NOT NULL,
            total_amount INT NOT NULL,
            number_of_party INT NOT NULL,
            average_amount INT NOT NULL,
            remain_amount INT NOT NULL,
            as_id INT NOT NULL DEFAULT 1,
            FOREIGN KEY (collabfund_id) REFERENCES collabfund (collabfund_id),
            FOREIGN KEY (collab_fun_activity_id) REFERENCES collab_fun_activity (collab_fun_activity_id),
            FOREIGN KEY (as_id) REFERENCES active_state (as_id)
        );
     */

    [Column("cf_dividing_money_id")]
    [Key]
    public int CF_DividingMoneyID { get; set; }

    [Column("collabfund_id")]
    public int CollabFundID { get; set; }

    [Column("collab_fun_activity_id")]
    public int CollabFunActivityID { get; set; }

    [Column("total_amount")]
    public int TotalAmount { get; set; }

    [Column("number_of_party")]
    public int NumberOfParty { get; set; }

    [Column("average_amount")]
    public int AverageAmount { get; set; }

    [Column("remain_amount")]
    public int RemainAmount { get; set; }

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;

    public virtual List<CF_DividingMoneyDetail> CF_DividingMoneyDetails { get; set; } = null!;
}
