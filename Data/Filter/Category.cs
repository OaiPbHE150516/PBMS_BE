using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace pbms_be.Data.Filter;
[Table("category", Schema = "public")]

public class Category
{
    //CREATE TABLE category(
    //   category_id serial PRIMARY KEY,
    //   account_id VARCHAR ( 100 ) NOT NULL,
    //   parent_category_id INT,
    //    category_name_vn VARCHAR( 100 ),
    //    category_name_en VARCHAR( 100 ),
    //    as_id INT NOT NULL DEFAULT 1,
    //    FOREIGN KEY(account_id) REFERENCES account(account_id),
    //    FOREIGN KEY(as_id) REFERENCES active_state(as_id)
    //);
    [Column("category_id")]
    [Key]
    public int CategoryID { get; set; }

    [Column("account_id")]
    public string AccountID { get; set; } = String.Empty;

    [Column("parent_category_id")]
    public int ParentCategoryID { get; set; }

    [Column("category_name_vn")]
    public string NameVN { get; set; } = String.Empty;

    [Column("category_name_en")]
    public string NameEN { get; set; } = String.Empty;

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;

}
