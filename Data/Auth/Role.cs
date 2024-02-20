using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.Auth;
[Table("role", Schema = "public")]

public class Role
{
    [Column("role_id")]
    [Key]
    public int RoleID { get; set; }

    [Column("role_name")]
    public string RoleName { get; set; } = String.Empty;

}

