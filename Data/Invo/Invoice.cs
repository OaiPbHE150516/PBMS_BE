using Microsoft.AspNetCore.Http.HttpResults;
using pbms_be.Data.Status;
using pbms_be.Data.WalletF;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace pbms_be.Data.Invo;
[Table("invoice", Schema = "public")]

public class Invoice
{
    //CREATE TABLE invoice(
    //invoice_id serial PRIMARY KEY,
    //transaction_id INT NOT NULL,
    //supplier_address VARCHAR ( 500 ) NOT NULL,
    //supplier_email VARCHAR( 100 ) NOT NULL,
    //supplier_name VARCHAR( 100 ) NOT NULL,
    //supplier_phone VARCHAR( 20 ) NOT NULL,
    //receiver_address VARCHAR( 500 ) NOT NULL,
    //receiver_email VARCHAR( 100 ) NOT NULL,
    //receiver_name VARCHAR( 100 ) NOT NULL,
    //id_of_invoice VARCHAR( 100 ) NOT NULL,
    //invoice_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    //invoice_type VARCHAR( 100 ) NOT NULL,
    //payment_terms VARCHAR( 100 ) NOT NULL,
    //currency_id INT NOT NULL,
    //net_amount BIGINT NOT NULL,
    //total_amount BIGINT NOT NULL,
    //tax_amount BIGINT NOT NULL,
    //discount BIGINT NOT NULL,
    //invoice_image_url VARCHAR( 500 ) NOT NULL,
    //invoice_raw_datalog VARCHAR( 100000) NOT NULL,
    //note VARCHAR( 5000 ) NOT NULL,
    //as_id INT NOT NULL DEFAULT 1,
    //FOREIGN KEY(transaction_id) REFERENCES transaction(transaction_id),
    //FOREIGN KEY(currency_id) REFERENCES currency_type(currency_id),
    //FOREIGN KEY(as_id) REFERENCES active_state(as_id)
    //);
    [Column("invoice_id")]
    public int InvoiceID { get; set; }

    [Column("transaction_id")]
    public int TransactionID { get; set; }

    [Column("supplier_address")]
    public string SupplierAddress { get; set; } = String.Empty;

    [Column("supplier_email")]
    public string SupplierEmail { get; set; } = String.Empty;

    [Column("supplier_name")]
    public string SupplierName { get; set; } = String.Empty;

    [Column("supplier_phone")]
    public string SupplierPhone { get; set; } = String.Empty;

    [Column("receiver_address")]
    public string ReceiverAddress { get; set; } = String.Empty;

    [Column("receiver_email")]
    public string ReceiverEmail { get; set; } = String.Empty;

    [Column("receiver_name")]
    public string ReceiverName { get; set; } = String.Empty;

    [Column("id_of_invoice")]
    public string IDOfInvoice { get; set; } = String.Empty;

    [Column("invoice_date")]
    public DateTime InvoiceDate { get; set; }

    [Column("invoice_type")]
    public string InvoiceType { get; set; } = String.Empty;

    [Column("payment_terms")]
    public string PaymentTerms { get; set; } = String.Empty;

    [Column("currency_id")]
    public int CurrencyID { get; set; }
    public virtual Currency Currency { get; set; } = null!;

    [Column("net_amount")]
    public long NetAmount { get; set; }

    [Column("total_amount")]
    public long TotalAmount { get; set; }

    [Column("tax_amount")]
    public long TaxAmount { get; set; }

    [Column("discount")]
    public long Discount { get; set; }

    [Column("invoice_image_url")]
    public string InvoiceImageURL { get; set; } = String.Empty;

    [Column("invoice_raw_datalog")]
    public string InvoiceRawDatalog { get; set; } = String.Empty;

    [Column("note")]
    public string Note { get; set; } = String.Empty;

    [Column("as_id")]
    public int ActiveStateID { get; set; }
    public virtual ActiveState ActiveState { get; set; } = null!;

}
