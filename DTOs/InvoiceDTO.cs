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
}
