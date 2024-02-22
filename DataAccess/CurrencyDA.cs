using Microsoft.EntityFrameworkCore;
using pbms_be.Data;
using pbms_be.Data.Status;
using pbms_be.Data.WalletF;
using System.Linq;

namespace pbms_be.DataAccess
{
    public class CurrencyDA
    {
        private readonly PbmsDbContext _context;
        public CurrencyDA(PbmsDbContext context)
        {
            _context = context;
        }

        public List<Currency> GetCurrencies()
        {
            var result = _context.Currency.Include(c => c.ActiveState).ToList();
            return result;
        }

        public Currency? GetCurrency(int CurrencyID)
        {
            var result = _context.Currency.Where(c => c.CurrencyID == CurrencyID).FirstOrDefault();
            return result;
        }

        public List<Currency> GetCurrencyByName(string Name)
        {
            var result = _context.Currency.Where(c => c.Name.Contains(Name)).ToList();
            return result;
        }

        public bool IsCurrencyExist(int CurrencyID)
        {
            var result = GetCurrency(CurrencyID);
            return result != null;
        }

        public Currency? CreateCurrency(Currency currency)
        {
            if (!IsCurrencyExist(currency.CurrencyID))
            {
                _context.Currency.Add(currency);
                _context.SaveChanges();
                return GetCurrency(currency.CurrencyID);
            }
            return null;
        }

        public Currency? UpdateCurrency(Currency currency)
        {
            if (IsCurrencyExist(currency.CurrencyID))
            {
                _context.Currency.Update(currency);
                _context.SaveChanges();
                return GetCurrency(currency.CurrencyID);
            }
            return null;
        }

        public Currency? DeleteCurrency(int CurrencyID)
        {
            if (IsCurrencyExist(CurrencyID))
            {
                var currency = GetCurrency(CurrencyID);
                _context.Currency.Remove(currency);
                _context.SaveChanges();
                return currency;
            }
            return null;
        }
    }
}
