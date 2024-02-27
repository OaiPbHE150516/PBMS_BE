using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
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

        public bool IsAccountExist(string AccountID)
        {
            var result = GetAccount(AccountID);
            return result != null;
        }

        public Account GetAccount(string AccountID)
        {
            try
            {
                var result = _context.Account.Where(a => a.AccountID == AccountID).FirstOrDefault();
                return result;
            } catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }

        public Account? CreateAccount(Account account)
        {
            if (!IsAccountExist(account.AccountID))
            {
                _context.Account.Add(account);
                _context.SaveChanges();
                return GetAccount(account.AccountID);
            }
            return null;
        }

        public Account? UpdateAccount(Account account)
        {
            if (!IsAccountExist(account.AccountID))
            {
                _context.Account.Update(account);
                _context.SaveChanges();
                return GetAccount(account.AccountID);
            }
            return null;
        }

        // get account by email
        // use to get account by email when 
        // - invite user to join collaboration fund
        // - 
        public Account? GetAccountByEmail(string Email)
        {
            var result = _context.Account.Where(a => a.EmailAddress.Contains(Email)).FirstOrDefault();
            return result;
        }

        internal object GetAllAccount()
        {
            try
            {
                var result = _context.Account.Where(a => a.RoleID == ConstantConfig.USER_ROLE_ID).ToList();
                return result;
            } catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
