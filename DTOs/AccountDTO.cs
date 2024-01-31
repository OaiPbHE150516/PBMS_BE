namespace pbms_be.DTOs
{
    public class AccountDTO
    {
        // same as Account.cs but create_time is in unix timestamp
        public int AccountID { get; set; }
        public string UniqueID { get; set; }
        public string ClientID { get; set; }
        public string EmailAddress { get; set; }
        public string AccountName { get; set; }
        public string PictureURL { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
