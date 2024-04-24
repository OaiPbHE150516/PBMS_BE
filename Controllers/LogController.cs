using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;
using pbms_be.ThirdParty;

namespace pbms_be.Controllers
{
    [Route("api/log")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private readonly HandleScanLog _handleScanLog;

        public LogController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _handleScanLog = new HandleScanLog(_context);
        }

        // a get method to get all scan logs of a day
        [HttpGet("scanLog/bydate/{date}")]
        public IActionResult GetScanLogs(string date)
        {
            try
            {
                var result = _handleScanLog.GetScanLogs(date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        } 
        [HttpGet("scanLog/all/bydate/{date}")]
        public IActionResult GetScanLogsAllByDate(string date)
        {
            try
            {
                var result = _handleScanLog.GetScanLogsAllByDate(date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get total log and log by accountID
        [HttpGet("scanLog/all")]
        public IActionResult GetScanLogs()
        {
            try
            {
                var result = _handleScanLog.GetScanLogs();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get last 30 days log
        [HttpGet("scanLog/lastNumDays/{day}")]
        public IActionResult GetLast30DaysLog(int day)
        {
            try
            {
                var result = _handleScanLog.GetLastNumbersDaysLog(day);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}
