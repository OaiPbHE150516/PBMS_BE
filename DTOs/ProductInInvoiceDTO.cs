namespace pbms_be.DTOs
{
    public class ProductInInvoiceDTO
    {
    }

    public class ProductInInvoiceCreateDTO
    {
        public string ProductName { get; set; } = String.Empty;
        public int Quanity { get; set; } = 1;
        public long UnitPrice { get; set; } = 0;
        public long TotalAmount { get; set; }
        public string Tag { get; set; } = String.Empty;
    }
}
