using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace pbms_be.Data.Balance;
[Table("balance_history_log", Schema = "public")]
public class BalanceHistoryLog
{
    /*
        CREATE TABLE balance_history_log (
            balance_history_log_id SERIAL PRIMARY KEY,
            account_id VARCHAR(255) NOT NULL,
            wallet_id INT NOT NULL,
            balance BIGINT NOT NULL,
            transaction_id INT NOT NULL,
            hislog_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
            as_id INT NOT NULL,
            FOREIGN KEY (account_id) REFERENCES account(account_id),
            FOREIGN KEY (wallet_id) REFERENCES wallet(wallet_id),
            FOREIGN KEY (transaction_id) REFERENCES transaction(transaction_id),    
            FOREIGN KEY (as_id) REFERENCES  active_state (as_id)
        );
     */
    [Column("balance_history_log_id")]
    [Key]
    public int BalanceHistoryLogID { get; set; }

    [Column("account_id")]
    public string AccountID { get; set; } = string.Empty;

    [Column("wallet_id")]
    public int WalletID { get; set; }

    [Column("balance")]
    public long Balance { get; set; }

    [Column("transaction_id")]
    public int TransactionID { get; set; }

    [Column("hislog_date")]
    public DateTime HisLogDate { get; set; } = DateTime.UtcNow;

    [Column("as_id")]
    public int ActiveStateID { get; set; }
}
