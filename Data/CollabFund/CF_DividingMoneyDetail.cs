using pbms_be.Configurations;
using pbms_be.Data.Auth;
using pbms_be.Data.Status;
using pbms_be.Library;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.CollabFund;
[Table("cf_dividing_money_detail", Schema = "public")]

public class CF_DividingMoneyDetail
{
    /*
        CREATE TABLE cf_dividing_money_detail (
            cf_dividing_money_detail_id serial PRIMARY KEY,
            cf_dividing_money_id INT NOT NULL,
            from_account_id VARCHAR ( 100 ) NOT NULL,
            to_account_id VARCHAR ( 100 ) NOT NULL,
            dividing_amount INT NOT NULL,
            isDone BOOLEAN NOT NULL DEFAULT FALSE,
            last_time timestamp not null default current_timestamp,
            FOREIGN KEY (cf_dividing_money_id) REFERENCES cf_dividing_money (cf_dividing_money_id)
        );
     */

    [Column("cf_dividing_money_detail_id")]
    [Key]
    public int CF_DividingMoneyDetailID { get; set; }

    [Column("cf_dividing_money_id")]
    public int CF_DividingMoneyID { get; set; }

    [Column("from_account_id")]
    public string FromAccountID { get; set; } = String.Empty;

    public virtual Account FromAccount { get; set; } = null!;

    [Column("to_account_id")]
    public string ToAccountID { get; set; } = String.Empty;

    public virtual Account ToAccount { get; set; } = null!;

    [Column("dividing_amount")]
    public int DividingAmount { get; set; }

    [Column("isdone")]
    public bool IsDone { get; set; } = false;

    [Column("last_time")]
    public DateTime LastTime { get; set; } = DateTime.UtcNow;

    public string LastTimeToString => LastTime.ToString(ConstantConfig.DEFAULT_DATETIME_FORMAT);

    public string MinusTimeNowString => LConvertVariable.ConvertMinusTimeNowString(LastTime);

}
