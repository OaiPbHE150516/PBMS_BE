using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.CollabFund;
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
        private WalletDA _walletDA;

        public WalletController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _walletDA = new WalletDA(context);
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

        // update  
        [HttpPut("update")]
        public IActionResult UpdateWallet([FromBody] WalletUpdateDTO walletDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var walletEntity = _mapper.Map<Wallet>(walletDTO);
                var walletDA = new WalletDA(_context);
                var result = walletDA.UpdateWallet(walletEntity);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("change/active-state")]
        public IActionResult ChangeWalletActiveState([FromBody] ChangeWalletActiveStateDTO changeActiveStateDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                WalletDA walletDA = new WalletDA(_context);
                var result = walletDA.ChangeWalletActiveState(changeActiveStateDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteWallet([FromBody] WalletDeleteDTO deleteDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_walletDA.IsWalletExist(deleteDTO.AccountID, deleteDTO.WalletID))
                return BadRequest("Not Found");
                var result = _walletDA.DeleteWallet(deleteDTO);
                return Ok(result);



            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
