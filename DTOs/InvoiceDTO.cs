namespace pbms_be.DTOs
{
    public class InvoiceDTO
    {
    }

    public class InvoiceCreateDTO
    {
        public string SupplierAddress { get; set; } = String.Empty;
        public string SupplierEmail { get; set; } = String.Empty;
        public string SupplierName { get; set; } = String.Empty;
        public string SupplierPhone { get; set; } = String.Empty;
        public string ReceiverAddress { get; set; } = String.Empty;
        public string ReceiverEmail { get; set; } = String.Empty;
        public string ReceiverName { get; set; } = String.Empty;
        public string IDOfInvoice { get; set; } = String.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public string InvoiceType { get; set; } = String.Empty;
        public string PaymentTerms { get; set; } = String.Empty;
        public long NetAmount { get; set; }
        public long TotalAmount { get; set; }
        public long TaxAmount { get; set; }
        public long Discount { get; set; }
        public string InvoiceImageURL { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public virtual List<ProductInInvoiceCreateDTO> Products { get; set; } = new List<ProductInInvoiceCreateDTO>();
        public string InvoiceRawDatalog { get; set; } = String.Empty;

    }
}
