namespace pbms_be.DTOs
{
    public class AccountDTO
    {
        // same as Account.cs but create_time is in unix timestamp
        public string AccountID { get; set; }
        public string ClientID { get; set; }
        public string EmailAddress { get; set; }
        public string AccountName { get; set; }
        public string PictureURL { get; set; }
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
}
