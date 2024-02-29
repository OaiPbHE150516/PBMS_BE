using pbms_be.Data.Status;
using pbms_be.Data.Budget;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using System.Text;

namespace pbms_be.Data.Budget
{
    [Table("budget", Schema = "public")]
    public class Budget
    {

        //CREATE TABLE budget(
        //budget_id serial PRIMARY KEY,
        //budget_name VARCHAR (100) NOT NULL,
        //budget_amount FLOAT NOT NULL,
        //account_id VARCHAR( 100 ) NOT NULL,
        //wallet_id int NOT NULL,
        //budgetCategody_id int NOT NULL,
        //fistDayOfMonth TIMESTAMP NOT NULL,
        //fistDayOfWeek TIMESTAMP NOT NULL ,
        //beginDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        //endDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        //repeatEnterval int NOT NULL,
        //note VARCHAR( 205 ) NULL,
        //create_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        //last_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        //as_id int NOT NULL,
        //FOREIGN KEY(account_id) REFERENCES account(account_id),
        //FOREIGN KEY(wallet_id) REFERENCES wallet(wallet_id),
        //FOREIGN KEY(as_id) REFERENCES active_state(as_id)
        //FOREIGN KEY() REFERENCES active_state(as_id)


        [Column("budget_id")]
        [Key]
        public int BudgetID { get; set; }

        [Column("budget_name")]
        public string BudgetName { get; set; } = String.Empty;

        [Column("budget_amount")]
        public float BudgetAmount  { get; set; }

        [Column("account_id")]
        public string AccountID { get; set; } = String.Empty;

        [Column("wallet_id")]
        public int WalletID { get; set; }

        [Column("budgetCategody_id")]
        public int BudgetCategoryID { get; set; }

        [Column("firstDayOfMonth")]
        public DateTime FirstDayOfMonth { get; set; }

        [Column("firstDayOfWeek")]
        public DateTime FirstDayOfWeek { get; set; }

        [Column("beginDate")]
        public DateTime BeginDate { get; set; } = DateTime.Now;

        [Column("endDate")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [Column("repeatEnterval")]
        public int RepeatInterVal { get; set; }

        [Column("note")]
        public string Note { get; set; } = string.Empty;

        [Column("create_time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Column("last_time")]
        public DateTime LastTime { get; set; } = DateTime.Now;

        [Column("as_id")]
        public int ActiveStateID { get; set; }
        public virtual ActiveState ActiveState { get; set; } = null!;
    }
}
