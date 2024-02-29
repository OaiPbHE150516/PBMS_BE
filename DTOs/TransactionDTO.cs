namespace pbms_be.DTOs
{
    public class TransactionDTO
    {
    }

    // transaction of collab fund activity
    public class Transaction_VM_DTO
    {
        // TransactionID, Category_VM_DTO, TotalAmount, Note, TransactionDate,
        // FromPerson, ToPerson, ImageURL
        public int TransactionID { get; set; }
        public Category_VM_DTO Category { get; set; } = null!;
        public long TotalAmount { get; set; }
        public string Note { get; set; } = String.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string FromPerson { get; set; } = String.Empty;
        public string ToPerson { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
    }
}
