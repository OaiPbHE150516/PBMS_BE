using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;
using pbms_be.Data.WalletF;
using pbms_be.DataAccess;

namespace pbms_be.Controllers
{
    [Route("api/currency/")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public CurrencyController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("get")]
        public IActionResult GetCurrencies()
        {
            CurrencyDA currencyDA = new CurrencyDA(_context);
            var result = currencyDA.GetCurrencies();
            return Ok(result);
        }

        [HttpGet("get/name/{name}")]
        public IActionResult GetCurrencyByName(string name)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            CurrencyDA currencyDA = new CurrencyDA(_context);
            var result = currencyDA.GetCurrencyByName(name);
            return Ok(result);
        }

        [HttpGet("get/id/{id}")]
        public IActionResult GetCurrency(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            CurrencyDA currencyDA = new CurrencyDA(_context);
            var result = currencyDA.GetCurrency(id);
            return Ok(result);
        }

        [HttpPost("create")]
        public IActionResult CreateCurrency([FromBody] Currency currency)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            CurrencyDA currencyDA = new CurrencyDA(_context);
            var result = currencyDA.CreateCurrency(currency);
            return Ok(result);
        }

        [HttpPost("update")]
        public IActionResult UpdateCurrency([FromBody] Currency currency)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            CurrencyDA currencyDA = new CurrencyDA(_context);
            var result = currencyDA.UpdateCurrency(currency);
            return Ok(result);
        }

        [HttpPost("delete/{currency_id}")]
        public IActionResult DeleteCurrency(int currency_id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            CurrencyDA currencyDA = new CurrencyDA(_context);
            var currency = currencyDA.GetCurrency(currency_id);
            if (currency == null) return BadRequest();
            var result = currencyDA.DeleteCurrency(currency_id);
            return Ok(result);
        }
    }
}
