using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using pbms_be.Data;
using pbms_be.Data.Auth;
namespace pbms_be.DataAccess
{
    public class AuthDA
    {
        private readonly PbmsDbContext _context;
        public AuthDA(PbmsDbContext context)
        {
            _context = context;
        }

        public bool IsAccountExist(string uniqueID)
        {
            var result = _context.Account.FromSqlRaw("SELECT * FROM account WHERE unique_id = @p0", uniqueID).FirstOrDefault();
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Account GetAccount(string uniqueID)
        {
            var result = _context.Account.FromSqlRaw("SELECT * FROM account WHERE unique_id = @p0", uniqueID).FirstOrDefault();
            return result;
        }

        // add new account to database
        public Account CreateAccount(Account account)
        {
            // check if account exist in database
            var result = IsAccountExist(account.UniqueID);
            if (result == false)
            {
                _context.Account.Add(account);
                _context.SaveChanges();
                return account;
            }
            else
            {
                return null;
            }
        }

        // update account in database
        public Account? UpdateAccount(Account account)
        {
            // check if account exist in database
            var result = IsAccountExist(account.UniqueID);
            if (result == true)
            {
                _context.Account.Update(account);
                _context.SaveChanges();
                return account;
            }
            else
            {
                return null;
            }
        }
    }
}
