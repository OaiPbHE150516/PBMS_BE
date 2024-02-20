using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.CollabFund;
[Table("collabfund", Schema = "public")]

public class CollabFund
{
    /*
     *  CREATE TABLE collabfund (
        collabfund_id serial PRIMARY KEY,
        cf_name VARCHAR ( 100 ) NOT NULL,
        cf_description VARCHAR ( 5000 ) NOT NULL,
        image_url VARCHAR ( 500 ) NOT NULL,
        total_amount BIGINT NOT NULL,
        createtime timestamp not null default current_timestamp,
        as_id INT NOT NULL DEFAULT 1,
        FOREIGN KEY (as_id) REFERENCES active_state (as_id)
        );
     */
    [Column("collabfund_id")]
    [Key]
    public int CollabFundID { get; set; }

    [Column("cf_name")]
    public string Name { get; set; } = String.Empty;

    [Column("cf_description")]
    public string Description { get; set; } = String.Empty;

    [Column("image_url")]
    public string ImageURL { get; set; } = String.Empty;

    [Column("total_amount")]
    public long TotalAmount { get; set; }

    [Column("createtime")]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;
}
