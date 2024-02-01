using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using pbms_be.Data;
using System.Security.Claims;
using pbms_be.Data.Auth;
using pbms_be.DataAccess;
using AutoMapper;
using pbms_be.DTOs;
using System;
using pbms_be.Configurations;

namespace pbms_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        // mapper
        private readonly IMapper _mapper;

        public AuthController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // POST JWT: api/<AuthController>
        [HttpPost("postJWT")]
        public IActionResult PostJWT([FromBody] string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                var sub = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_CLIENT_UNIQUEID).Value;
                AuthDA authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(sub))
                {
                    Account account = new Account();
                    account.AccountID = sub;
                    account.ClientID = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_CLIENT_ID).Value;
                    account.AccountName = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_NAME).Value;
                    account.EmailAddress = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_CLIENT_EMAIL).Value;
                    account.RoleID = ConstantConfig.USER_ROLE_ID;
                    account.PictureURL = token.Claims.First(c => c.Type == ConstantConfig.TOKEN_CLIENT_PICTURE).Value;
                    account.CreateTime = DateTime.UtcNow;
                    var resultAccount = authDA.CreateAccount(account);
                    return Ok(resultAccount);
                }
                else
                {
                    Account? account = authDA.GetAccount(sub);
                    return Ok(account);
                }
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/<AuthController>
        // update account info
        [HttpPost("update")]
        public IActionResult Update([FromBody] AccountUpdateDTO account)
        {
            try
            {
                AuthDA authDA = new AuthDA(_context);
                if (authDA.IsAccountExist(account.AccountID))
                {
                    var accountEntity = _mapper.Map<Account>(account);
                    var resultAccount = authDA.UpdateAccount(accountEntity);
                    return Ok(resultAccount);
                }
                else
                {
                    return NotFound();
                }
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
