using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using pbms_be.Data;
using System.Security.Claims;
using pbms_be.Data.Auth;
using pbms_be.DataAccess;

namespace pbms_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PbmsDbContext _context;

        public AuthController(PbmsDbContext context)
        {
            _context = context;
        }

        // POST JWT: api/<AuthController>
        [HttpPost("postJWT")]
        public IActionResult PostJWT([FromBody] string jwt)
        {
            // decode jwt by using System.IdentityModel.Tokens.Jwt
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var sub = token.Claims.First(c => c.Type == "sub").Value;
            // check if account exist in database
            AuthDA authDA = new AuthDA(_context);
            var result = authDA.IsAccountExist(sub);
            if (result == false)
            {
                // create new account
                Account account = new Account();
                account.AccountName = token.Claims.First(c => c.Type == "name").Value;
                account.ClientID = token.Claims.First(c => c.Type == "aud").Value;
                account.EmailAddress = token.Claims.First(c => c.Type == "email").Value;
                account.EnCodedJWT = jwt;
                account.PictureURL = token.Claims.First(c => c.Type == "picture").Value;
                account.UniqueID = sub;
                // add new account to database
                var resultAccount = authDA.CreateAccount(account);
                return Ok(resultAccount);
            }
            else
            {
                 // get account from database
                 Account account = authDA.GetAccount(sub);
                // return account
                return Ok(account);
            }
        }
    }
}
