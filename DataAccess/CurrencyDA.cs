using Microsoft.EntityFrameworkCore;
using pbms_be.Data;
using pbms_be.Data.Wallet;

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
            //var result = _context.Currency.ToList();
            //return result;

            // get all currencies and join with vision status table
            //var result = _context.Currency.Join(_context.VisionStatus,
            //                                currency => currency.VisionStatusID,
            //                                visionStatus => visionStatus.VisionStatusID,
            //                                (currency, visionStatus) => new Currency
            //                                {
            //                                    CurrencyID = currency.CurrencyID,
            //                                    Name = currency.Name,
            //                                    Symbol = currency.Symbol,
            //                                    VisionStatusID = currency.VisionStatusID,
            //                                    VisionStatus = visionStatus
            //                                }).ToList();
            //return result;

            // get currency_id, currency_name, currency_country, currency_symbol, vision_status_name from currency_type and vision_status
            //var result = _context.Currency.Join(_context.VisionStatus,
            //                                               currency => currency.VisionStatusID,
            //                                                                                          visionStatus => visionStatus.VisionStatusID,
            //                                                                                                                                     (currency, visionStatus) => new Currency
            //                                                                                                                                     {
            //                                    CurrencyID = currency.CurrencyID,
            //                                    Name = currency.Name,
            //                                    Country = currency.Country,
            //                                    Symbol = currency.Symbol,
            //                                    VisionStatusID = currency.VisionStatusID,
            //                                    VisionStatus = visionStatus
            //                                }).ToList();
            //return result;
            var sql = "SELECT currency_id, currency_name, currency_country, currency_symbol, vision_status_id, vision_status_name FROM currency_type left join vision_status on currency_type.vision_status_id = vision_status.vision_status_id;";
            var result = _context.Currency.FromSqlRaw(sql).ToList();
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
