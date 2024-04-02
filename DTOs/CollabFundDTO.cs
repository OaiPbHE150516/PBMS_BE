using pbms_be.Data.Auth;
using pbms_be.Data.CollabFund;
using pbms_be.Data.Status;
using pbms_be.Data.Trans;

namespace pbms_be.DTOs
{
    public class CollabFundDTO
    {
    }

    public class CollabFund_VM_DTO
    {
        public int CollabFundID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public long TotalAmount { get; set; }
        public string TotalAmountStr { get; set; } = String.Empty;
        public virtual bool isFundholder { get; set; } = false;

        public virtual List<AccountInCollabFundDTO> AccountInCollabFunds { get; set; } = null!;
        //public virtual List<CollabFundActivity> CollabFundActivities { get; set; } = null!;

    }

    public class CollabFundDetail_VM_DTO
    {
        public int CollabFundID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public long TotalAmount { get; set; }
        public string TotalAmountStr { get; set; } = String.Empty;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public string CreateTimeStr { get; set; } = String.Empty;
        public virtual List<AccountInCollabFundDTO> AccountInCollabFunds { get; set; } = null!;
        public virtual List<CollabFundActivity_MV_DTO> CollabFundActivities { get; set; } = null!;
    }

    // CreateCollabFundDTO
    public class CreateCollabFundDTO
    {
        public string AccountID { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ImageURL { get; set; } = String.Empty;
        public List<string> AccountIDs { get; set; } = null!;
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
        public string AccountID { get; set; } = String.Empty;
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

    public class CreateCfaNoTransactionHaveFileDTO
    {
        public int CollabFundID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public IFormFile? File { get; set; } = null!;
    }

    public class CreateCfaWithTransactionHaveFileDTO
    {
        public int CollabFundID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public IFormFile? File { get; set; } = null!;
        public int? TransactionID { get; set; }
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

    public class CollabFundAccountActiveStateDTO : Account
    {
        public int CFA_ActiveStateID { get; set; }
        public virtual ActiveState CFA_ActiveState { get; set; } = null!;
    }

    public class CF_DividingMoney_MV_DTO
    {
        public int CollabFundID { get; set; }
        public string TotalAmount { get; set; } = String.Empty;
        public string NumberParticipant { get; set; } = String.Empty;
        public string AverageAmount { get; set; } = String.Empty;
        public string RemainAmount { get; set; } = String.Empty;
        public virtual List<CF_DividingMoneyDetail_MV_DTO> CF_DividingMoneyDetails { get; set; } = null!;
    }

    // CF_DividingMoneyDetail_MV_DTO to response to client and convert long number to string, money format
    public class CF_DividingMoneyDetail_MV_DTO
    {
        public int CF_DividingMoneyDetailID { get; set; }
        public int CF_DividingMoneyID { get; set; }
        public string FromAccountID { get; set; } = String.Empty;
        public string ToAccountID { get; set; } = String.Empty;
        public string DividingAmount { get; set; } = String.Empty;
        public bool IsDone { get; set; }
        public string LastTime { get; set; } = String.Empty;
        public string MinusTimeNowString { get; set; } = String.Empty;
    }

    public class CF_DividingMoneyDetail_Wallet_DTO
    {
        public int CF_DividingMoneyDetailID { get; set; }
        public string FromAccountID { get; set; } = String.Empty;
        public string ToAccountID { get; set; } = String.Empty;
    }

    // CollabFundActivity_MV_DTO to response to client and convert long number to string, money format
    public class CollabFundActivity_MV_DTO
    {
        public int CollabFundActivityID { get; set; }
        public int CollabFundID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public virtual Account_VM_DTO Account { get; set; } = null!;
        public string Note { get; set; } = String.Empty;
        public string Filename { get; set; } = String.Empty;
        public int TransactionID { get; set; }
        public virtual Transaction_VM_DTO Transaction { get; set; } = null!;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public string CreateTimeString { get; set; } = String.Empty;
        public string MinusTimeNowString { get; set; } = String.Empty;
    }

    public class CollabAccountDTO
    {
        public int CollabFundID { get; set; }
        public string AccountID { get; set; } = String.Empty;
    }
}
