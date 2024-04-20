using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.DataAccess;
using pbms_be.Library;

namespace pbms_be.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private readonly DashboardCalculator _dashboardCalculator;

        public DashboardController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _dashboardCalculator = new DashboardCalculator(_context);
        }

        // get wallet balance log
        //[HttpGet("get/walletbalancelog/{accountID}")]

        // get total amount of each category in range of date of accountID
        [HttpGet("get/totalamount/category/{type}/{accountID}/{fromDateStr}/{toDateStr}")]
        public IActionResult GetTotalAmountByCategory(int type, string accountID, string fromDateStr, string toDateStr)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (string.IsNullOrEmpty(fromDateStr)) return BadRequest(Message.FROM_DATE_REQUIRED);
                if (string.IsNullOrEmpty(toDateStr)) return BadRequest(Message.TO_DATE_REQUIRED);
                if (type != ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_INCOME
                    && type != ConstantConfig.DEFAULT_CATEGORY_TYPE_ID_EXPENSE)
                    return BadRequest(Message.VALUE_TYPE_IS_NOT_VALID);

                var fromDateArr = fromDateStr.Split("-");
                var toDateArr = toDateStr.Split("-");

                var fromDate = new DateTime(int.Parse(fromDateArr[2]), int.Parse(fromDateArr[1]), int.Parse(fromDateArr[0]), 0, 0, 0).ToUniversalTime();
                var toDate = new DateTime(int.Parse(toDateArr[2]), int.Parse(toDateArr[1]), int.Parse(toDateArr[0]), 23, 59, 59).ToUniversalTime();
                if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);

                var result = _dashboardCalculator.GetTotalAmountByCategory(type, accountID, fromDate, toDate, _mapper);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get/totalamount/type/{accountID}/{fromDateStr}/{toDateStr}")]
        public IActionResult GetTotalAmountByType(string accountID, string fromDateStr, string toDateStr)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (string.IsNullOrEmpty(fromDateStr)) return BadRequest(Message.FROM_DATE_REQUIRED);
                if (string.IsNullOrEmpty(toDateStr)) return BadRequest(Message.TO_DATE_REQUIRED);

                var fromDateArr = fromDateStr.Split("-");
                var toDateArr = toDateStr.Split("-");

                var fromDate = new DateTime(int.Parse(fromDateArr[2]), int.Parse(fromDateArr[1]), int.Parse(fromDateArr[0]), 0, 0, 0).ToUniversalTime();
                var toDate = new DateTime(int.Parse(toDateArr[2]), int.Parse(toDateArr[1]), int.Parse(toDateArr[0]), 23, 59, 59).ToUniversalTime();
                if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);

                var result = _dashboardCalculator.GetTotalAmountByType(accountID, fromDate, toDate, _mapper);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // get total amount of tag of product in range of date of accountID
        [HttpGet("get/totalamount/tag/{accountID}/{fromDateStr}/{toDateStr}")]
        public IActionResult GetTotalAmountByTag(string accountID, string fromDateStr, string toDateStr)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (string.IsNullOrEmpty(fromDateStr)) return BadRequest(Message.FROM_DATE_REQUIRED);
                if (string.IsNullOrEmpty(toDateStr)) return BadRequest(Message.TO_DATE_REQUIRED);

                var fromDateArr = fromDateStr.Split("-");
                var toDateArr = toDateStr.Split("-");

                var fromDate = new DateTime(int.Parse(fromDateArr[2]), int.Parse(fromDateArr[1]), int.Parse(fromDateArr[0]), 0, 0, 0).ToUniversalTime();
                var toDate = new DateTime(int.Parse(toDateArr[2]), int.Parse(toDateArr[1]), int.Parse(toDateArr[0]), 23, 59, 59).ToUniversalTime();
                if (fromDate > toDate) return BadRequest(Message.FROM_DATE_GREATER_THAN_TO_DATE);

                var result = _dashboardCalculator.GetTotalAmountByTag(accountID, fromDate, toDate, _mapper);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
