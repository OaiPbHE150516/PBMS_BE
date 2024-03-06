using pbms_be.Data.Status;
using pbms_be.Library;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.DTOs
{
    public class AccountDTO
    {
        // same as Account.cs but create_time is in unix timestamp
        public string AccountID { get; set; } = String.Empty;
        public string ClientID { get; set; } = String.Empty;
        public string EmailAddress { get; set; } = String.Empty;
        public string AccountName { get; set; } = String.Empty;
        public string PictureURL { get; set; } = String.Empty;
        public int RoleID { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class AccountUpdateDTO
    {
        public string AccountID { get; set; } = String.Empty;
        public string EmailAddress { get; set; } = String.Empty;
        public string AccountName { get; set; } = String.Empty;
        public string PictureURL { get; set; } = String.Empty;
    }

    public class Account_VM_DTO
    {
        public string AccountID { get; set; } = String.Empty;
        public string EmailAddress { get; set; } = String.Empty;
        public string AccountName { get; set; } = String.Empty;
        public string PictureURL { get; set; } = String.Empty;
    }

    public class AccountInCollabFundDTO
    {
        public string AccountID { get; set; } = String.Empty;
        public string EmailAddress { get; set; } = String.Empty;
        public string AccountName { get; set; } = String.Empty;
    }

    public class AccountDetailInCollabFundDTO
    {
        public string AccountID { get; set; } = String.Empty;
        public string ClientID { get; set; } = String.Empty;
        public string EmailAddress { get; set; } = String.Empty;
        public string AccountName { get; set; } = String.Empty;
        public string PictureURL { get; set; } = String.Empty;
        public bool IsFundholder { get; set; }
        public int ActiveStateID { get; set; }
        public virtual ActiveState ActiveState { get; set; } = null!;
        public DateTime LastTime { get; set; } = DateTime.UtcNow;
        public string LastTimeStr { get; set; } = LConvertVariable.ConvertDateTimeToString(DateTime.UtcNow);
    }
}
