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

        // get by walletID and accountID and time range
        [HttpGet("get/each/{accountID}/{walletID}/{fromDateStr}/{toDateStr}")]
        public async Task<IActionResult> GetBalanceHistoryLog(string accountID, int walletID, string fromDateStr, string toDateStr)
        {
            try
            {
                // check if accountID is null or empty, then throw exception
                if (string.IsNullOrEmpty(accountID)) throw new Exception(Message.ACCOUNT_NOT_FOUND);
                if (string.IsNullOrEmpty(fromDateStr)) return BadRequest(Message.FROM_DATE_REQUIRED);
                if (string.IsNullOrEmpty(toDateStr)) return BadRequest(Message.TO_DATE_REQUIRED);
                var fromDateArr = fromDateStr.Split("-");
                var toDateArr = toDateStr.Split("-");

                var fromDate = new DateTime(int.Parse(fromDateArr[2]), int.Parse(fromDateArr[1]), int.Parse(fromDateArr[0]), 0, 0, 0).ToUniversalTime();
                var toDate = new DateTime(int.Parse(toDateArr[2]), int.Parse(toDateArr[1]), int.Parse(toDateArr[0]), 23, 59, 59).ToUniversalTime();
                if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);
                // log to console fromDate and toDate
                Console.WriteLine("fromDate: " + fromDate.ToString());
                Console.WriteLine("toDate: " + toDate.ToString());

                var result = await _balanceHisLogDA.GetBalanceHistoryLog(accountID, walletID, fromDate, toDate, _mapper);
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
