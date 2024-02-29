﻿using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.CollabFund;
using pbms_be.Data.WalletF;
using pbms_be.DTOs;

namespace pbms_be.DataAccess
{
    public class WalletDA
    {
        private readonly PbmsDbContext _context;
        public WalletDA(PbmsDbContext context)
        {
            _context = context;
        }

        // get wallet by wallet id and account id
        public Wallet? GetWallet(int WalletID, string AccountID)
        {
            var result = _context.Wallet
                .Where(w => w.WalletID == WalletID && w.AccountID == AccountID)
                .Include(w => w.Currency)
                .Include(w => w.ActiveState)
                .FirstOrDefault();
            return result;
        }

        // get all wallets of an account by account id
        public List<Wallet> GetWallets(string AccountID)
        {
            var result = _context.Wallet
                .Where(w => w.AccountID == AccountID)
                .Include(w => w.Currency)
                .Include(w => w.ActiveState)
                .ToList();
            return result;
        }

        // check if a wallet exist by wallet name and account id
        public bool IsWalletExist(string AccountID, string WalletName)
        {
            var result = GetWalletByName(AccountID, WalletName);
            return result != null;
        }
        public bool IsWalletExist(string AccountID, int WalletID)
        {
            var result = GetWallet(WalletID);
            return result != null;
        }

        // get a wallet by account id and wallet id
        public Wallet? GetWalletByName(string AccountID, string WalletName)
        {
            var result = _context.Wallet
                .Where(w => w.AccountID == AccountID && w.Name.Contains(WalletName))
                .Include(w => w.Currency)
                .Include(w => w.ActiveState)
                .FirstOrDefault();
            return result;
        }

        // get wallet by wallet id
        public Wallet? GetWallet(int WalletID)
        {
            // get wallet by wallet id
            var result = _context.Wallet
                .Where(w => w.WalletID == WalletID)
                .Include(w => w.Currency)
                .Include(w => w.ActiveState)
                .FirstOrDefault();
            return result;
        }

        // get wallet by status id and account id
        public List<Wallet> GetWalletsByStatusID(string AccountID, int VisionStatusID)
        {
            // get wallet by status id and account id
            var result = _context.Wallet
                .Where(w => w.AccountID == AccountID && w.ActiveStateID == VisionStatusID)
                .Include(w => w.Currency)
                .Include(w => w.ActiveState)
                .ToList();
            return result;
        }


        public Wallet? CreateWallet(Wallet wallet)
        {
            if (!IsWalletExist(wallet.AccountID, wallet.Name))
            {
                _context.Wallet.Add(wallet);
                _context.SaveChanges();
                return GetWallet(wallet.WalletID);
            }
            return null;
        }


       // update a wallet
       //check duplicate
        public Wallet? UpdateWallet(Wallet wallet)
        {
            if (IsWalletExist(wallet.AccountID, wallet.WalletID))
            {
                _context.Wallet.Update(wallet);
                _context.SaveChanges();
                return GetWallet(wallet.WalletID);
            }
            return null;
        }



        // change wallet status
        public Wallet? ChangeWalletStatus(int WalletID, int VisionStatusID)
        {
            //var wallet = GetWallet(WalletID);
            //if (wallet != null)
            //{
            //    wallet.VisionStatusID = VisionStatusID;
            //    _context.Wallet.Update(wallet);
            //    _context.SaveChanges();
            //    return GetWallet(WalletID);
            //}
            return null;
        }

        internal Wallet ChangeWalletActiveState(ChangeWalletActiveStateDTO changeActiveStateDTO)
        {
            try
            {
                var wallet = GetWallet(changeActiveStateDTO.WalletID);
                if (wallet == null) throw new Exception(Message.COLLAB_FUND_NOT_EXIST);
                wallet.ActiveStateID = changeActiveStateDTO.ActiveStateID;
                _context.SaveChanges();
                 return GetWallet(wallet.WalletID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


    }
}
