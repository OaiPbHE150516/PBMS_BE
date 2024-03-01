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
}
