namespace pbms_be.DTOs
{
    public class BalanceHisLogDTO
    {
    }

    public class BalanceHisLog_VM_DTO
    {
        public int BalanceHistoryLogID { get; set; }

        public string AccountID { get; set; } = string.Empty;

        public int WalletID { get; set; }

        public long Balance { get; set; }

        public string BalanceStr { get; set; } = String.Empty;

        public int TransactionID { get; set; }

        public DateTime HisLogDate { get; set; } = DateTime.UtcNow;

        public string HisLogDateStr { get; set; } = String.Empty;
    }
}
