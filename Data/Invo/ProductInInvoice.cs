using pbms_be.Data.Status;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace pbms_be.Data.Invo;
[Table("product_in_invoice", Schema = "public")]

public class ProductInInvoice
{
    //CREATE TABLE product_in_invoice(
    //product_id serial PRIMARY KEY,
    //invoice_id INT NOT NULL,
    //product_name VARCHAR ( 100 ) NOT NULL DEFAULT '',
    //    quanity INT DEFAULT 1,
    //    unit_price BIGINT DEFAULT 1,
    //    total_amount BIGINT NOT NULL,
    //    note VARCHAR( 5000 ) DEFAULT '',
    //    tag_id INT NOT NULL DEFAULT 1,
    //    as_id INT NOT NULL DEFAULT 1,
    //    FOREIGN KEY(invoice_id) REFERENCES invoice(invoice_id),
    //    FOREIGN KEY(tag_id) REFERENCES tag(tag_id),
    //    FOREIGN KEY(as_id) REFERENCES active_state(as_id)
    //);

    [Column("product_id")]
    [Key]
    public int ProductID { get; set; }

    [Column("invoice_id")]
    public int InvoiceID { get; set; }
    public virtual Invoice Invoice { get; set; } = null!;

    [Column("product_name")]
    public string ProductName { get; set; } = String.Empty;

    [Column("quanity")]
    public int Quanity { get; set; } = 1;

    [Column("unit_price")]
    public long UnitPrice { get; set; } = 1;

    [Column("total_amount")]
    public long TotalAmount { get; set; }

    [Column("note")]
    public string Note { get; set; } = String.Empty;

    [Column("tag_id")]
    public int TagID { get; set; }

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;
}
