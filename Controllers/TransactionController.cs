using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.DataAccess;
using pbms_be.DTOs;

namespace pbms_be.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private TransactionDA _transactionDA;

        public TransactionController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _transactionDA = new TransactionDA(_context);
        }

        // get all transaction by account id
        [HttpGet("get/account/{accountID}")]
        public IActionResult GetTransactions(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if(_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var result = _transactionDA.GetTransactions(accountID);
                var resultDTO = _mapper.Map<List<TransactionP_VM_DTO>>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
