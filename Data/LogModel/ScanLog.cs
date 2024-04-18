using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using pbms_be.Data.Auth;

namespace pbms_be.Data.LogModel;
[Table("scan_log", Schema = "public")]

public class ScanLog
{
    /*
    CREATE TABLE scan_log (
        scan_log_id SERIAL PRIMARY KEY,
        account_id VARCHAR(255) NOT NULL,
        scan_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        scan_type VARCHAR(255) NOT NULL DEFAULT 'MANUAL',
        scan_result VARCHAR(255) NOT NULL DEFAULT 'SUCCESS',
        FOREIGN KEY (account_id) REFERENCES account(account_id)
    );
    */
    [Column("scan_log_id")]
    [Key]
    public int ScanLogID { get; set; }

    [Column("account_id")]
    public string AccountID { get; set; } = String.Empty;

    public virtual Account Account { get; set; } = null!;

    [Column("scan_time")]
    public DateTime ScanTime { get; set; } = DateTime.Now;

    [Column("scan_type")]
    public string ScanType { get; set; } = String.Empty;

    [Column("scan_result")]
    public string ScanResult { get; set; } = String.Empty;
}


