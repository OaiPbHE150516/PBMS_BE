using System.ComponentModel.DataAnnotations.Schema;
namespace pbms_be.Data.Status;
[Table("vision_status_id", Schema = "public")]

public class VisionStatus
{
    // vision_status_id, vision_status_name
    [Column("vision_status_id")]
    public int VisionStatusID { get; set; }

    [Column("vision_status_name")]
    public string Name { get; set; } = String.Empty;
}

