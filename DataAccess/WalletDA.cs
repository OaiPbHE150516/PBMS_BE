using pbms_be.Data;
using pbms_be.Data.Wallet;

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
            var result = _context.Wallet.Where(w => w.WalletID == WalletID && w.AccountID == AccountID).FirstOrDefault();
            return result;
        }

        // get all wallets of an account by account id
        public List<Wallet> GetWallets(string AccountID)
        {
            var result = _context.Wallet.Where(w => w.AccountID == AccountID).ToList();
            return result;
        }

        // check if a wallet exist by wallet name and account id
        public bool IsWalletExist(string AccountID, string WalletName)
        {
            var result = GetWalletByName(AccountID, WalletName);
            return result != null;
        }

        // get a wallet by account id and wallet id
        public Wallet? GetWalletByName(string AccountID, string WalletName)
        {
            var result = _context.Wallet.Where(w => w.AccountID == AccountID && w.Name == WalletName).FirstOrDefault();
            return result;
        }

        // get wallet by wallet id
        public Wallet? GetWallet(int WalletID)
        {
            var result = _context.Wallet.Where(w => w.WalletID == WalletID).FirstOrDefault();
            return result;
        }

        // get wallet by status id and account id
        public List<Wallet> GetWalletsByStatusID(string AccountID, int VisionStatusID)
        {
            var result = _context.Wallet.Where(w => w.AccountID == AccountID && w.VisionStatusID == VisionStatusID).ToList();
            return result;
        }

        // create a wallet
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
        public Wallet? UpdateWallet(Wallet wallet)
        {
            if (IsWalletExist(wallet.AccountID, wallet.Name))
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
            var wallet = GetWallet(WalletID);
            if (wallet != null)
            {
                wallet.VisionStatusID = VisionStatusID;
                _context.Wallet.Update(wallet);
                _context.SaveChanges();
                return GetWallet(WalletID);
            }
            return null;
        }
    }
}
