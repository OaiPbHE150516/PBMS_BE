using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;
using pbms_be.DataAccess;

namespace pbms_be.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public TransactionController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // get all transaction by account id
        [HttpGet("get/account/{accountID}")]
        public IActionResult GetTransactions(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest("AccountID is required");
                TransactionDA transactionDA = new TransactionDA(_context);
                var result = transactionDA.GetTransactions(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
