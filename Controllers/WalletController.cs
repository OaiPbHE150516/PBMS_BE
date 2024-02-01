using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;
using pbms_be.Data.WalletF;
using pbms_be.DataAccess;
using pbms_be.DTOs;

namespace pbms_be.Controllers
{
    [Route("api/wallet/")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public WalletController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("get/account/{accountID}")]
        public IActionResult GetWallets(string accountID)
        {
            WalletDA walletDA = new WalletDA(_context);
            var result = walletDA.GetWallets(accountID);
            return Ok(result);
        }

        [HttpGet("get/id/{walletID}/{accountID}")]
        public IActionResult GetWallet(int walletID, string accountID)
        {
            WalletDA walletDA = new WalletDA(_context);
            var result = walletDA.GetWallet(walletID, accountID);
            return Ok(result);
        }

        [HttpPost("create")]
        public IActionResult CreateWallet([FromBody] WalletCreateDTO wallet)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            WalletDA walletDA = new WalletDA(_context);
            var walletEntity = _mapper.Map<Wallet>(wallet);
            var result = walletDA.CreateWallet(walletEntity);
            if (result == null) return BadRequest();
            return Ok(result);
        }
    }
}
