using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Auth;
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
        private readonly CollabFundDA _collabFundDA;

        public CollabFundController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _collabFundDA = new CollabFundDA(_context);
        }

        /*=======================           Get Methods          =======================*/

        // get all collab fund by account id
        [HttpGet("get/account/{accountID}")]
        public IActionResult GetCollabFunds(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _collabFundDA.GetCollabFunds(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get detail collab fund by collab fund id and account id
        [HttpGet("get/detail/{collabFundID}/{accountID}")]
        public IActionResult GetDetailCollabFund(int collabFundID, string accountID)
        {
            try
            {
                if (collabFundID <= 0) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                CollabFund collabfund = new CollabFund();
                bool isExist = false;
                _collabFundDA.GetDetailCollabFund(collabFundID, accountID, out isExist, out collabfund);
                if (!isExist) return BadRequest(Message.COLLAB_FUND_NOT_EXIST);
                return Ok(collabfund);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get all activity collab fund by collab fund id and account id
        [HttpGet("get/activity/{collabFundID}/{accountID}")]
        public IActionResult GetAllActivityCollabFund(int collabFundID, string accountID)
        {
            try
            {
                if (collabFundID <= 0) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _collabFundDA.GetAllActivityCollabFund(collabFundID, accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get all accounts ( as parties) of collab fund by collab fund id and account id
        [HttpGet("get/member/{collabFundID}/{accountID}")]
        public IActionResult GetAllMemberCollabFund(int collabFundID, string accountID)
        {
            try
            {
                if (collabFundID <= 0) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _collabFundDA.GetAllMemberCollabFund(collabFundID, accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get list account by contain email address and their state with collab fund
        [HttpGet("get/account/{collabfundID}/{email}")]
        public IActionResult GetAccountByEmail(int collabfundID, string email)
        {
            try
            {
                if (collabfundID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
                if (string.IsNullOrEmpty(email)) return BadRequest(Message.EMAIL_ADDRESS_REQUIRED);
                List<Account> accountEmail = _collabFundDA.GetAccountByEmail(email);
                if (accountEmail is null) return BadRequest(Message.ACCOUNT_NOT_FOUND);
                var result = _collabFundDA.GetAccountByEmailAndCollabFundID(collabfundID, accountEmail);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        /*=======================           End of Get Methods          =======================*/

        /*=======================           Post Methods          =======================*/

        // create collab fund
        [HttpPost("create")]
        public IActionResult CreateCollabFund([FromBody] CreateCollabFundDTO collabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var collabFundEntity = _mapper.Map<CollabFund>(collabFundDTO);
                if (_collabFundDA.IsCollabFundExist(collabFundEntity))
                    return BadRequest(Message.COLLAB_FUND_ALREADY_EXIST);
                collabFundEntity.ActiveStateID = ActiveStateConst.ACTIVE;
                var result = _collabFundDA.CreateCollabFund(collabFundEntity);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // create collab fund activity
        [HttpPost("create/activity/notrans")]
        public IActionResult CreateCollabFundActivity([FromBody] CreateCfaNoTransactionDTO collabFundActivityDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var collabFundActivityEntity = _mapper.Map<CollabFundActivity>(collabFundActivityDTO);
                collabFundActivityEntity.TransactionID = ConstantConfig.DEFAULT_NULL_TRANSACTION_ID;
                collabFundActivityEntity.ActiveStateID = ActiveStateConst.ACTIVE;
                var result = _collabFundDA.CreateCollabFundActivity(collabFundActivityEntity);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // create collab fund activity with transaction id
        [HttpPost("create/activity/withtrans")]
        public IActionResult CreateCollabFundActivityWithTransaction([FromBody] CreateCfaWithTransactionDTO collabFundActivityDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var collabFundActivityEntity = _mapper.Map<CollabFundActivity>(collabFundActivityDTO);
                collabFundActivityEntity.ActiveStateID = ActiveStateConst.ACTIVE;
                var result = _collabFundDA.CreateCollabFundActivity(collabFundActivityEntity);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // add member to collab fund by collab fund id and account id, only fundholder can add member
        [HttpPost("invite")]
        public IActionResult InviteMemberCollabFund([FromBody] MemberCollabFundDTO addMemberCollabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _collabFundDA.InviteMemberCollabFund(addMemberCollabFundDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        

        /*=======================           End of Post Methods          =======================*/

        /*=======================           Put Methods          =======================*/

        // update collab fund
        [HttpPut("update")]
        public IActionResult UpdateCollabFund([FromBody] UpdateCollabFundDTO collabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var collabFundEntity = _mapper.Map<CollabFund>(collabFundDTO);
                var result = _collabFundDA.UpdateCollabFund(collabFundEntity);
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
                var result = _collabFundDA.ChangeCollabFundActiveState(changeActiveStateDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // accept invitation to join collab fund by collab fund id and account id
        [HttpPut("accept")]
        public IActionResult AcceptMemberCollabFund([FromBody] MemberCollabFundDTO acceptMemberCollabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _collabFundDA.AcceptMemberCollabFund(acceptMemberCollabFundDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        
        /*=======================           End of Put Methods          =======================*/


        /*=======================           Delete Methods          =======================*/

        // delete a member from collab fund by collab fund id and account id, only fundholder can delete member
        [HttpDelete("delete/member")]
        public IActionResult DeleteMemberCollabFund([FromBody] MemberCollabFundDTO deleteMemberCollabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                CollabFundDA collabFundDA = new CollabFundDA(_context);
                var result = collabFundDA.DeleteMemberCollabFund(deleteMemberCollabFundDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // decline invitation to join collab fund by collab fund id and account id
        [HttpDelete("decline")]
        public IActionResult DeclineMemberCollabFund([FromBody] MemberCollabFundDTO declineMemberCollabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _collabFundDA.DeclineMemberCollabFund(declineMemberCollabFundDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // delete invitation to join collab fund by collab fund id and account id, only fundholder can delete invitation
        [HttpDelete("delete/invitation")]
        public IActionResult DeleteInvitationCollabFund([FromBody] MemberCollabFundDTO deleteInvitationCollabFundDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _collabFundDA.DeleteInvitationCollabFund(deleteInvitationCollabFundDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
