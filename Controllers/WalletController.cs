using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;
using pbms_be.Data.Wallet;
using pbms_be.DataAccess;

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

        [HttpGet("get-by-account-id/{accountID}")]
        public IActionResult GetWallets(string accountID)
        {
            WalletDA walletDA = new WalletDA(_context);
            var result = walletDA.GetWallets(accountID);
            return Ok(result);
        }

        [HttpGet("get-by-id/{walletID}/{accountID}")]
        public IActionResult GetWallet(int walletID, string accountID)
        {
            WalletDA walletDA = new WalletDA(_context);
            var result = walletDA.GetWallet(walletID, accountID);
            return Ok(result);
        }

        [HttpPost("create")]
        public IActionResult CreateWallet([FromBody] Wallet wallet)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            WalletDA walletDA = new WalletDA(_context);
            var result = walletDA.CreateWallet(wallet);
            if (result == null) return BadRequest();
            return Ok(result);
        }
    }
}
