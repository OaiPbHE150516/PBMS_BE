using Microsoft.AspNetCore.Mvc;
using pbms_be.Data.WalletF;

namespace pbms_be.Interface
{
    public interface IWallet
    {
        public IActionResult GetWallets(string accountID);
    }
}
