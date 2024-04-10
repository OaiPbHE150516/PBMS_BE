using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.CollabFund;
using pbms_be.Data.WalletF;
using pbms_be.DTOs;
using pbms_be.Library;
using System.Reflection.Metadata;

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
            try
            {
                var result = _context.Wallet
                .Where(w => w.WalletID == WalletID 
                && w.AccountID == AccountID 
                && w.ActiveStateID == ActiveStateConst.ACTIVE
                || w.ActiveStateID == ActiveStateConst.INACTIVE
                )
                .Include(w => w.Currency)
                .Include(w => w.ActiveState)
                .FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // get all wallets of an account by account id
        public List<Wallet> GetWallets(string AccountID)
        {
            try
            {
                var result = _context.Wallet
                            .Where(w => w.AccountID == AccountID 
                            && w.ActiveStateID == ActiveStateConst.ACTIVE
                            || w.ActiveStateID == ActiveStateConst.INACTIVE
                            )
                            .Include(w => w.Currency)
                            .Include(w => w.ActiveState)
                            .OrderBy(w => w.Balance)
                            .ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // check if a wallet exist by wallet name and account id
        public bool IsWalletExist(string AccountID, string WalletName)
        {
            try
            {
                var result = _context.Wallet.Any(w => w.Name.Contains(WalletName) 
                && w.AccountID == AccountID 
                && w.ActiveStateID == ActiveStateConst.ACTIVE
                || w.ActiveStateID == ActiveStateConst.INACTIVE
                );
                return result;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public bool IsWalletExist(string AccountID, int WalletID)
        {
            try
            {
                var result = _context.Wallet.Any(w => w.WalletID == WalletID 
                && w.AccountID == AccountID 
                && w.ActiveStateID == ActiveStateConst.ACTIVE
                || w.ActiveStateID == ActiveStateConst.INACTIVE
                );
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        // get a wallet by account id and wallet id
        public Wallet? GetWalletByName(string AccountID, string WalletName)
        {
            var result = _context.Wallet
                .Where(w => w.AccountID == AccountID && w.Name.Contains(WalletName) 
                && w.ActiveStateID == ActiveStateConst.ACTIVE
                || w.ActiveStateID == ActiveStateConst.INACTIVE
                )
                .Include(w => w.Currency)
                .Include(w => w.ActiveState)
                .FirstOrDefault();
            return result;
        }

        // get wallet by wallet id
        public Wallet? GetWallet(int WalletID)
        {
            try
            {
                // get wallet by wallet id
                var result = _context.Wallet
                    .Where(w => w.WalletID == WalletID 
                    && w.ActiveStateID == ActiveStateConst.ACTIVE
                    || w.ActiveStateID == ActiveStateConst.INACTIVE
                    )
                    .Include(w => w.Currency)
                    .Include(w => w.ActiveState)
                    .FirstOrDefault();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
            try
            {
                if (!IsWalletExist(wallet.AccountID, wallet.Name))
                {
                    wallet.ActiveStateID = ActiveStateConst.ACTIVE;
                    _context.Wallet.Add(wallet);
                    _context.SaveChanges();
                    return GetWallet(wallet.WalletID);
                }
                return null;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        // update a wallet
        //check duplicate
        public Wallet? UpdateWallet(Wallet wallet)
        {
            try
            {
                if (IsWalletExist(wallet.AccountID, wallet.WalletID))
                {
                    var result = _context.Wallet.Where(w => w.WalletID == wallet.WalletID && w.AccountID == wallet.AccountID && w.ActiveStateID == ActiveStateConst.ACTIVE || w.ActiveStateID == ActiveStateConst.INACTIVE)
                    .FirstOrDefault();
                    if (result is not null)
                    {
                        result.Name = wallet.Name;
                        result.Note = wallet.Note;
                        result.IsBanking = wallet.IsBanking;
                        result.QRCodeURL = wallet.QRCodeURL;
                        result.BankName = wallet.BankName;
                        result.BankAccount = wallet.BankAccount;
                        result.BankUsername = wallet.BankUsername;
                        _context.SaveChanges();
                        return GetWallet(wallet.WalletID);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

        internal Wallet? ChangeWalletActiveState(ChangeWalletActiveStateDTO changeActiveStateDTO)
        {
            try
            {
                var wallet = _context.Wallet
                    .Where(w => w.WalletID == changeActiveStateDTO.WalletID)
                    .Include(w => w.Currency)
                    .Include(w => w.ActiveState)
                    .FirstOrDefault();
                if (wallet is null) throw new Exception(Message.WALLET_NOT_FOUND);
                wallet.ActiveStateID = changeActiveStateDTO.ActiveStateID;
                _context.SaveChanges();
                return GetWallet(wallet.WalletID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object DeleteWallet(WalletDeleteDTO deleteDTO)
        {
            try
            {
                var wallet = GetWallet(deleteDTO.WalletID);
                if (wallet is null) throw new Exception(Message.COLLAB_FUND_NOT_EXIST);
                wallet.ActiveStateID = ActiveStateConst.DELETED;
                _context.SaveChanges();
                var returnlist = GetWallets(deleteDTO.AccountID);
                return returnlist;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal object GetTotalAmount(string accountID)
        {
            try
            {
                // get total amount of all wallets have currency id = 2

                var result = _context.Wallet
                    .Where(w => w.AccountID == accountID 
                    && w.CurrencyID == CurrencyConst.DEFAULT_CURRENCY_ID_VND
                    && w.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Sum(w => w.Balance);
                var totalBalance = new WalletTotalBalance_VM_DTO
                {
                    TotalBalance = LConvertVariable.ConvertToMoneyFormat(result)
                };
                return totalBalance;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal List<Wallet> GetTotalAmountEachWallet(string accountID)
        {
            try
            {
                //var result = _context.Wallet
                //    .Where(w => w.AccountID == accountID)
                //    .Select(w => new
                //    {
                //        w.WalletID,
                //        w.Name,
                //        w.Balance
                //    })
                //    .ToList();
                var result = _context.Wallet
                            .Where(w => w.AccountID == accountID 
                            && w.ActiveStateID == ActiveStateConst.ACTIVE
                            || w.ActiveStateID == ActiveStateConst.INACTIVE
                            )
                            .ToList();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal void UpdateWalletAmount(int walletID, long totalAmount, int categoryTypeID)
        {
            try
            {
                
                var wallet = GetWallet(walletID) ?? throw new Exception(Message.WALLET_NOT_FOUND);
                if (categoryTypeID == ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_EXPENSE)
                {
                    wallet.Balance -= totalAmount;
                    Console.WriteLine("Expense: " + totalAmount);
                }
                else
                {
                    wallet.Balance += totalAmount;
                    Console.WriteLine("Income: " + totalAmount);
                }
                // save changes that wallet
                wallet.ActiveStateID = ActiveStateConst.ACTIVE;
                _context.SaveChanges();
                // check save changes success or not
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
