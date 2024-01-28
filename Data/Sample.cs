using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data;
[Table("sample", Schema = "public")]

public class Sample
{
    [Column("id")]
    public int id { get; set; }

    [Column("name")]
    public string name { get; set; }

    [Column("description")]
    public string description { get; set; }
}