using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.DataAccess;

namespace pbms_be.Controllers
{
    [Route("api/balanceHisLogController")]
    [ApiController]
    public class BalanceHisLogController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private BalanceHisLogDA _balanceHisLogDA;

        public BalanceHisLogController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _balanceHisLogDA = new BalanceHisLogDA(_context);
        }

        // get all balance history log by accountID
        [HttpGet("get/all/{accountID}")]
        public async Task<IActionResult> GetBalanceHistoryLog(string accountID)
        {
            try
            {
                var result = await _balanceHisLogDA.GetBalanceHistoryLog(accountID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get by walletID and accountID
        [HttpGet("get/each/{accountID}/{walletID}")]
        public async Task<IActionResult> GetBalanceHistoryLog(string accountID, int walletID)
        {
            try
            {
                // check if accountID is null or empty, then throw exception
                if (string.IsNullOrEmpty(accountID))
                {
                    throw new Exception(Message.ACCOUNT_NOT_FOUND);
                }
                var result = await _balanceHisLogDA.GetBalanceHistoryLog(accountID, walletID, _mapper);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get all balance history log by accountID
        [HttpGet("get/all/byday/{accountID}")]
        public async Task<IActionResult> GetBalanceHistoryLogByDay(string accountID)
        {
            try
            {
                var result = await _balanceHisLogDA.GetBalanceHistoryLogByDay(accountID, _mapper);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get all balance history log by accountID, from date and to date
        [HttpGet("get/all/bydate/{accountID}/{fromDateOnly}/{toDateOnly}")]
        public async Task<IActionResult> GetBalanceHistoryLogByDate(string accountID, DateOnly fromDateOnly, DateOnly toDateOnly)
        {
            try
            {
                var result = await _balanceHisLogDA.GetBalanceHistoryLogByDate(accountID, fromDateOnly, toDateOnly, _mapper);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
