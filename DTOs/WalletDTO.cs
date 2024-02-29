﻿namespace pbms_be.DTOs
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
        public int ActiveStateID { get; set; }
    }

    public class Wallet_VM_DTO
    {
        public string Name { get; set; } = String.Empty;
    }
   
    public class WalletUpdateDTO
    {
        // AccountID, Name, Balance, CurrencyID, ActiveStateID
        public int WalletID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
       
    }
    public class WalletDeleteDTO
    {
        
        public int WalletID { get; set; }
        public string AccountID { get; set; } = String.Empty;
        

    }

    public class ChangeWalletActiveStateDTO
    {
        public int WalletID { get; set; }
        public int ActiveStateID { get; set; }
    }
}
