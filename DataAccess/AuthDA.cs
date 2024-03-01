using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Auth;
using System.IdentityModel.Tokens.Jwt;
namespace pbms_be.DataAccess
{
    public class AuthDA
    {
        private readonly PbmsDbContext _context;
        public AuthDA(PbmsDbContext context)
        {
            _context = context;
        }

        // sign in by jwt
        public Account? SigninByJWT(string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var sub = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_CLIENT_UNIQUEID).Value;
                if (!IsAccountExist(sub))
                {
                    Account account = new Account();
                    account.AccountID = sub;
                    account.ClientID = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_CLIENT_ID).Value;
                    account.AccountName = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_NAME).Value;
                    account.EmailAddress = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_CLIENT_EMAIL).Value;
                    account.RoleID = ConstantConfig.USER_ROLE_ID;
                    account.PictureURL = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_CLIENT_PICTURE).Value;
                    account.CreateTime = DateTime.UtcNow;
                    var resultAccount = CreateAccount(account);
                    return resultAccount;
                }
                else
                {
                    Account? account = GetAccount(sub);
                    return account;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        public bool IsAccountExist(string AccountID)
        {
            var result = _context.Account.Any(a => a.AccountID == AccountID);
            return result;
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

        private Account? CreateAccount(Account account)
        {
            try
            {
                _context.Account.Add(account);
                _context.SaveChanges();
                return GetAccount(account.AccountID);
            } catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
            
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
