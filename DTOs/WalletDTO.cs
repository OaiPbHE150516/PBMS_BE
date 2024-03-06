using pbms_be.Data.WalletF;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.DTOs
{
    public class WalletDTO
    {
    }

    public class WalletCreateDTO
    {
        // AccountID, Name, Balance, CurrencyID, ActiveStateID
        public string AccountID { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public long Balance { get; set; }
        public int CurrencyID { get; set; }
    }

    public class Wallet_VM_DTO
    {
        public int WalletID { get; set; }
        public string Name { get; set; } = String.Empty;
        public long Balance { get; set; }
        public string BalanceStr { get; set; } = String.Empty;
        public int CurrencyID { get; set; }
        public virtual Currency Currency { get; set; } = null!;
        public string Note { get; set; } = String.Empty;
        public bool IsBanking { get; set; } = false;
        public string QRCodeURL { get; set; } = String.Empty;
        public string BankName { get; set; } = String.Empty;
        public string BankAccount { get; set; } = String.Empty;
        public string BankUsername { get; set; } = String.Empty;
        public string CreateTimeStr { get; set; } = String.Empty;

    }

    public class Wallet_Balance_VM_DTO
    {
        public int WalletID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Balance { get; set; } = String.Empty;
    }

    public class WalletDetail_VM_DTO
    {
        public int WalletID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public bool IsBanking { get; set; }
        public string QRCodeURL { get; set; } = String.Empty;
        public string BankName { get; set; } = String.Empty;
        public string BankAccount { get; set; } = String.Empty;
        public string BankUsername { get; set; } = String.Empty;

    }

    // all total balance of all wallets
    public class WalletTotalBalance_VM_DTO
    {
        public string TotalBalance { get; set; } = String.Empty;
    }

    // w
   
    public class WalletUpdateDTO
    {
        // AccountID, Name, Balance, CurrencyID, ActiveStateID
        public int WalletID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;

        // note, isbanking, qr_code_URL, bank_name, bank_account, bank_username
        public string Note { get; set; } = String.Empty;
        public bool IsBanking { get; set; } = false;
        public string QRCodeURL { get; set; } = String.Empty;
        public string BankName { get; set; } = String.Empty;
        public string BankAccount { get; set; } = String.Empty;
        public string BankUsername { get; set; } = String.Empty;

       
    }
    public class WalletDeleteDTO
    {
        
        public int WalletID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        

    }

    public class ChangeWalletActiveStateDTO
    {
        public string AccountID { get; set; } = String.Empty;
        public int WalletID { get; set; }
        public int ActiveStateID { get; set; }
    }

    public class WalletInTransaction_VM_DTO
    {
        public string Name { get; set; } = String.Empty;
        public string Balance { get; set; } = String.Empty;
    }
}
