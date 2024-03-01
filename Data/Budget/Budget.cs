using pbms_be.Data.Status;
using pbms_be.Data.Budget;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using System.Text;
using pbms_be.Data.Filter;

namespace pbms_be.Data.Budget;

[Table("budget", Schema = "public")]
public class Budget
{
    /*
     CREATE TABLE budget(
        budget_id serial PRIMARY KEY,
        account_id VARCHAR ( 100 ) NOT NULL,
        budget_name VARCHAR (100) NOT NULL DEFAULT '',
        amountdue BIGINT NOT NULL,
        current_amount BIGINT NOT NULL DEFAULT 0,
        beginDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        endDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        repeatEnterval int NOT NULL DEFAULT 0,
        note VARCHAR ( 500 ) DEFAULT '',
        create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        as_id int NOT NULL DEFAULT 1,
        FOREIGN KEY(account_id) REFERENCES account(account_id),
        FOREIGN KEY(as_id) REFERENCES active_state(as_id)
    );
     */

    [Column("budget_id")]
    [Key]
    public int BudgetID { get; set; }

    [Column("account_id")]
    public string AccountID { get; set; } = String.Empty;

    [Column("budget_name")]
    public string BudgetName { get; set; } = String.Empty;

    [Column("amountdue")]
    public long TargetAmount { get; set; }

    [Column("current_amount")]
    public long CurrentAmount { get; set; }

    [Column("begindate")]
    public DateTime BeginDate { get; set; } = DateTime.UtcNow;

    [Column("enddate")]
    public DateTime EndDate { get; set; } = DateTime.UtcNow;

    [Column("btype")]
    public int BudgetTypeID { get; set; }
    public virtual BudgetType BudgetType { get; set; } = null!;

    [Column("repeatenterval")]
    public int RepeatInterVal { get; set; }

    [Column("note")]
    public string Note { get; set; } = string.Empty;

    [Column("create_time")]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;

}

public class BudgetType
{
    public int BudgetTypeID { get; set; }
    public string TypeName { get; set; } = string.Empty;
}
