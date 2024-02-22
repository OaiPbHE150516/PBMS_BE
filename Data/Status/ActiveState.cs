using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace pbms_be.Data.Status;
[Table("active_state", Schema = "public")]

public class ActiveState
{
    // as_id, as_name
    [Column("as_id")]
    [Key]
    public int ActiveStateID { get; set; }

    [Column("as_name")]
    public string Name { get; set; } = String.Empty;
}

