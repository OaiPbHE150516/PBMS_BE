using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.CollabFund;
using pbms_be.DataAccess;
using pbms_be.DTOs;

namespace pbms_be.Controllers
{
    [Route("api/collabfund")]
    [ApiController]
    public class CollabFundController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public CollabFundController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // get all collab fund by account id
        [HttpGet("get/account/{accountID}")]
        public IActionResult GetCollabFunds(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                CollabFundDA collabFundDA = new CollabFundDA(_context);
                var result = collabFundDA.GetCollabFunds(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get a collab fund by collab fund id and account id
        [HttpGet("get/id/{collabFundID}/account/{accountID}")]
        public IActionResult GetCollabFund(int collabFundID, string accountID)
        {
            try
            {
                if (collabFundID <= 0) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                CollabFundDA collabFundDA = new CollabFundDA(_context);
                var result = collabFundDA.GetCollabFund(collabFundID, accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // create collab fund
        [HttpPost("create")]
        public IActionResult CreateCollabFund([FromBody] CreateCollabFundDTO collabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var collabFundEntity = _mapper.Map<CollabFund>(collabFundDTO);
                CollabFundDA collabFundDA = new CollabFundDA(_context);
                if (collabFundDA.IsCollabFundExist(collabFundEntity))
                    return BadRequest(Message.COLLAB_FUND_ALREADY_EXIST);
                collabFundEntity.ActiveStateID = ActiveStateConst.ACTIVE;
                var result = collabFundDA.CreateCollabFund(collabFundEntity);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // update collab fund
        [HttpPut("update")]
        public IActionResult UpdateCollabFund([FromBody] UpdateCollabFundDTO collabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var collabFundEntity = _mapper.Map<CollabFund>(collabFundDTO);
                CollabFundDA collabFundDA = new CollabFundDA(_context);
                var result = collabFundDA.UpdateCollabFund(collabFundEntity);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // change collab fund active state
        [HttpPut("change/active-state")]
        public IActionResult ChangeCollabFundActiveState([FromBody] ChangeCollabFundActiveStateDTO changeActiveStateDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                CollabFundDA collabFundDA = new CollabFundDA(_context);
                var result = collabFundDA.ChangeCollabFundActiveState(changeActiveStateDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
