using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using pbms_be.Data;
using pbms_be.Data.Custom;
using pbms_be.DataAccess;
using pbms_be.Library;

namespace pbms_be.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class AATestingController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public AATestingController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // get all account
        [HttpGet("getAllAccount")]
        public IActionResult GetAllAccount()
        {
            var authDA = new AuthDA(_context);
            var accounts = authDA.GetAllAccount();
            return Ok(accounts);
        }

        // delete cf_activity has note = "Test chia tiền"
        [HttpDelete("deleteCFActivity")]
        public IActionResult DeleteCFActivity()
        {
            var collabFundDA = new CollabFundDA(_context);
            var result = collabFundDA.DeleteCFActivity();
            return Ok(result);
        }

        // get all category by account id
        [HttpGet("getCategories")]
        public IActionResult GetCategories()
        {
            var accountID = "117911566377016615313";
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest("AccountID is required");
                CategoryDA categoryDA = new CategoryDA(_context);
                var result = categoryDA.GetCategories(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // method post to generate data
        [HttpPost("generate/transaction")]
        public IActionResult GenerateTransactionData([FromBody] GenerateRandomTransactions data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = LDataGenerator.GenerateRandomTransactionsEF(data, _context);
            return Ok(result);
        }
    }
}
