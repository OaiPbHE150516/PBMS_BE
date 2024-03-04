using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.Filter;
[Table("default_category", Schema = "public")]

public class DefaultCategory
{
    /*
        CREATE TABLE default_category (
            category_id serial PRIMARY KEY,
            category_name_vn VARCHAR ( 100 ) NOT NULL,
            category_name_en VARCHAR ( 100 ) NOT NULL,
            parent_category_id INT NOT NULL,
            category_type_id INT NOT NULL DEFAULT 2,
            isRoot bool NOT NULL DEFAULT false,
            FOREIGN KEY (category_type_id) REFERENCES category_type (category_type_id)        
        );
     */
    [Column("category_id")]
    [Key]
    public int CategoryID { get; set; }

    [Column("category_name_vn")]
    public string NameVN { get; set; } = String.Empty;

    [Column("category_name_en")]
    public string NameEN { get; set; } = String.Empty;

    [Column("parent_category_id")]
    public int ParentCategoryID { get; set; }

    [Column("category_type_id")]
    public int CategoryTypeID { get; set; }
    public virtual CategoryType CategoryType { get; set; } = null!;

    [Column("isroot")]
    public bool IsRoot { get; set; } = false;
}
