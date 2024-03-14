using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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

        #region Get Methods

        [HttpGet("get/account/{accountID}")]
        public IActionResult GetWallets(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(accountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);
                var result = _walletDA.GetWallets(accountID);
                if (result is null) return BadRequest(Message.WALLET_NOT_FOUND);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<List<Wallet_VM_DTO>>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get/id/{walletID}/{accountID}")]
        public IActionResult GetWallet(int walletID, string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (walletID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.WALLET_ID_REQUIRED);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(accountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);
                if (!_walletDA.IsWalletExist(accountID, walletID)) return BadRequest(Message.WALLET_NOT_FOUND);
                var result = _walletDA.GetWallet(walletID, accountID);
                if (result is null) return BadRequest(Message.WALLET_NOT_FOUND);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<Wallet_VM_DTO>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get all total amount of all wallets of an account
        [HttpGet("get/total-amount/{accountID}")]
        public IActionResult GetTotalAmount(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(accountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);
                var result = _walletDA.GetTotalAmount(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get all total amount of each wallets of an account
        [HttpGet("get/total-amount-each-wallet/{accountID}")]
        public IActionResult GetTotalAmountEachWallet(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(accountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);
                var result = _walletDA.GetTotalAmountEachWallet(accountID);
                if (result is null) return BadRequest();
                if(_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<List<Wallet_Balance_VM_DTO>>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Post Methods

        [HttpPost("create")]
        public IActionResult CreateWallet([FromBody] WalletCreateDTO wallet)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(wallet.AccountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);

                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var walletEntity = _mapper.Map<Wallet>(wallet);
                var result = _walletDA.CreateWallet(walletEntity);

                if (result is null) return BadRequest();
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Put Methods

        // update  
        [HttpPut("update")]
        public IActionResult UpdateWallet([FromBody] WalletUpdateDTO walletDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(walletDTO.AccountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);

                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var walletEntity = _mapper.Map<Wallet>(walletDTO);
                var result = _walletDA.UpdateWallet(walletEntity);
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
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(changeActiveStateDTO.AccountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);

                if (_walletDA.IsWalletExist(changeActiveStateDTO.AccountID, changeActiveStateDTO.WalletID))
                    return BadRequest(Message.WALLET_NOT_FOUND);
                var result = _walletDA.ChangeWalletActiveState(changeActiveStateDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Delete Methods

        [HttpDelete("delete")]
        public IActionResult DeleteWallet([FromBody] WalletDeleteDTO deleteDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (!_walletDA.IsWalletExist(deleteDTO.AccountID, deleteDTO.WalletID)) return BadRequest(Message.WALLET_NOT_FOUND);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(deleteDTO.AccountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);
                var result = _walletDA.DeleteWallet(deleteDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

    }
}
