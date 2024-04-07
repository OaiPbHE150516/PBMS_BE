using pbms_be.Data.Invo;

namespace pbms_be.DTOs
{
    public class InvoiceDTO
    {
    }

    public class InvoiceCreateDTO
    {
        public string SupplierAddress { get; set; } = String.Empty;
        public string SupplierName { get; set; } = String.Empty;
        public string SupplierPhone { get; set; } = String.Empty;
        public string IDOfInvoice { get; set; } = String.Empty;
        public string InvoiceDate { get; set; } = DateTime.UtcNow.ToString();
        public long NetAmount { get; set; }
        public long TotalAmount { get; set; }
        public long TaxAmount { get; set; }
        public string InvoiceImageURL { get; set; } = String.Empty;
        public virtual List<ProductInInvoiceCreateDTO> Products { get; set; } = [];
    }

    public class Invoice_VM_DTO
    {
        public int InvoiceID { get; set; }
        public string SupplierAddress { get; set; } = String.Empty;
        public string SupplierName { get; set; } = String.Empty;
        public string SupplierPhone { get; set; } = String.Empty;
        public string IDOfInvoice { get; set; } = String.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public long NetAmount { get; set; }
        public string NetAmountStr { get; set; } = String.Empty;
        public long TotalAmount { get; set; }
        public string TotalAmountStr { get; set; } = String.Empty;
        public long TaxAmount { get; set; }
        public string TaxAmountStr { get; set; } = String.Empty;
        public string InvoiceImageURL { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public virtual List<Product_VM_DTO> Products { get; set; } = [];
    }
}
