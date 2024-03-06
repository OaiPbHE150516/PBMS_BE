using pbms_be.Data.Filter;
using pbms_be.Data.WalletF;

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

    public class TransactionInList_VM_DTO
    {
        public int TransactionID { get; set; }
        public int WalletID { get; set; }
        public virtual WalletInTransaction_VM_DTO Wallet { get; set; } = null!;
        public int CategoryID { get; set; }
        public virtual CategoryInTransaction_VM_DTO Category { get; set; } = null!;
        public string TotalAmount { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string TransactionDateStr { get; set; } = String.Empty;
        public string TransactionDateMinus { get; set; } = String.Empty;
        public string FromPerson { get; set; } = String.Empty;
        public string ToPerson { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
    }

    public class TransactionDetail_VM_DTO
    {
        public int TransactionID { get; set; }
        public int WalletID { get; set; }
        public virtual WalletInTransaction_VM_DTO Wallet { get; set; } = null!;
        public int CategoryID { get; set; }
        public virtual CategoryInTransaction_VM_DTO Category { get; set; } = null!;
        public string TotalAmount { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string TransactionDateStr { get; set; } = String.Empty;
        public string TransactionDateMinus { get; set; } = String.Empty;
        public string FromPerson { get; set; } = String.Empty;
        public string ToPerson { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
    }

    public class TransactionCreateDTO
    {
        public string AccountID { get; set; } = String.Empty;
        public int WalletID { get; set; }
        public int CategoryID { get; set; }
        public long TotalAmount { get; set; }
        public string Note { get; set; } = String.Empty;
        public string FromPerson { get; set; } = String.Empty;
        public string ToPerson { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public virtual InvoiceCreateDTO Invoice { get; set; } = null!;
    }
}
