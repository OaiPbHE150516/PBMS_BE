using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Auth;
using pbms_be.DataAccess;
using pbms_be.DTOs;

namespace pbms_be.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private AuthDA _authDA;

        public ProfileController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _authDA = new AuthDA(_context);
        }

        // get account by account id
        [HttpGet("get/{accountID}")]
        public IActionResult GetAccount(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _authDA.GetAccount(accountID);
                if (result is null) return NotFound();
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] AccountUpdateDTO account)
        {
            try
            {
                AuthDA authDA = new AuthDA(_context);
                if (authDA.IsAccountExist(account.AccountID))
                {
                    if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                    var resultAccount = authDA.UpdateAccount(_mapper.Map<Account>(account));
                    return Ok(resultAccount);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
