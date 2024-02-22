using pbms_be.Data.Trans;

namespace pbms_be.DTOs
{
    public class CollabFundDTO
    {
    }

    // CreateCollabFundDTO
    public class CreateCollabFundDTO
    {
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public long TotalAmount { get; set; }
    }

    // UpdateCollabFundDTO
    public class UpdateCollabFundDTO
    {
        public int CollabFundID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public long TotalAmount { get; set; }
        public int ActiveStateID { get; set; }

    }

    // ChangeActiveStateDTO
    public class ChangeCollabFundActiveStateDTO
    {
        public int CollabFundID { get; set; }
        public int ActiveStateID { get; set; }
    }

    // CreateCfaNoTransactionDTO
    public class CreateCfaNoTransactionDTO
    {
        public int CollabFundID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public string Filename { get; set; } = String.Empty;
    }

    // CreateCfaWithTransactionDTO
    public class CreateCfaWithTransactionDTO
    {
        public int CollabFundID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public string Filename { get; set; } = String.Empty;
        public int TransactionID { get; set; }
    }

    public class MemberCollabFundDTO
    {
        public int CollabFundID { get; set; }
        public string AccountFundholderID { get; set; } = String.Empty;
        public string AccountMemberID { get; set; } = String.Empty;
    }
}
