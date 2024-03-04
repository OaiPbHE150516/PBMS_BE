using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.Filter;
[Table("category_type", Schema = "public")]

public class CategoryType
{
    /*
        create table category_type (
             category_type_id serial PRIMARY KEY,
             category_type_name VARCHAR ( 100 ) NOT NULL
        );
     */
    [Column("category_type_id")]
    [Key]
    public int CategoryTypeID { get; set; }

    [Column("category_type_name")]
    public string Name { get; set; } = String.Empty;
}
