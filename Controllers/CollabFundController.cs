﻿using AutoMapper;
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

        #region Get Methods
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
                if (collabFundID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (_collabFundDA.IsAccountInCollabFund(accountID, collabFundID) == false)
                    return BadRequest(Message.ACCOUNT_IS_NOT_IN_COLLAB_FUND);

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
                if (collabFundID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (_collabFundDA.IsAccountInCollabFund(accountID, collabFundID) == false)
                    return BadRequest(Message.ACCOUNT_IS_NOT_IN_COLLAB_FUND);
                var result = _collabFundDA.GetAllActivityCollabFund(collabFundID, accountID);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var resultEntity = _mapper.Map<List<CollabFundActivity_MV_DTO>>(result);
                return Ok(resultEntity);
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
                if (collabFundID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
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

        // get all invitation collab fund by collab fund id and account id

        //// get all dividing money and detail by collab fund id and account id
        //[HttpGet("get/dividing-money/{collabFundID}/{accountID}")]
        //public IActionResult GetAllDividingMoneyAndDetail(int collabFundID, string accountID)
        //{
        //    try
        //    {
        //        if (collabFundID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
        //        if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
        //        if (_collabFundDA.IsAccountInCollabFund(accountID, collabFundID) == false) return BadRequest(Message.ACCOUNT_IS_NOT_IN_COLLAB_FUND);
        //        var result = _collabFundDA.GetAllDividingMoneyWithDetail(collabFundID, accountID);
        //        return Ok(result);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        // get divide money information by collab fund id and account id before divide money action
        //[HttpGet("get/divide-money-info/{collabFundID}/{accountID}")]
        //public IActionResult GetDivideMoneyInfo(int collabFundID, string accountID)
        [HttpGet("get/divide-money-info/")]
        public IActionResult GetDivideMoneyInfo()
        {
            int collabFundID = 2;
            string accountID = "117911566377016615313";
            try
            {
                if (collabFundID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.COLLAB_FUND_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (_collabFundDA.IsAccountInCollabFund(accountID, collabFundID) == false) return BadRequest(Message.ACCOUNT_IS_NOT_IN_COLLAB_FUND);

                var dividemoneyinfor = _collabFundDA.GetDivideMoneyInfo(collabFundID, accountID);

                if (dividemoneyinfor is null) return BadRequest(Message.COLLAB_FUND_NOT_EXIST);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                // convert property in dividemoneyinfor to string
                //var cf_dividing_moneyEntity = _mapper.Map<CF_DividingMoney_MV_DTO>(dividemoneyinfor);
                return Ok(dividemoneyinfor);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion Get Methods

        #region Post Methods
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

        #endregion Post Methods

        #region Put Methods

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

        #endregion Put Methods

        #region Delete Methods
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

        #endregion Delete Methods

    }
}

/*
 Note: 27/02
I. Collab_Fund 
    - Thêm 1 mothod get được tất cả thông tin của 1 collab fund
    - OK: Thêm hàm để lấy tất cả số tiền đã chi ra của 
        một người tham gia collab fund (kể từ lần chia cuối cùng) đến thời điểm hiện tại.
        - trong thời gian chia tiền, nếu người dùng có thêm giao dịch thì để cho lần sau chia tiền.
    - Nếu đã thực hiện thao tác chia tiền rồi thì người dùng không thể update giao dịch ( thông qua hoạt động) đã xảy ra trước khi chia tiền
    - Thêm hàm để xem thông tin chia tiền của 1 lần chia tiền trước khi thực hiện hành động chia tiền
    - Thêm hàm để thực hiện hành động chia tiền. thêm popup để chọn người cần chia tiền ( thường là tất cả mọi người)
    - Thêm hàm để 1 người dùng lấy thông tin của ví người đích ( mã QR và banking infor) 
        cùng với số tiền để chuyển
    - Thêm hàm để sau khi người dùng chuyển tiền xong, thì cập nhật lại isDone của 
        cf_dividing_money_detail
III. Transaction
    1. Thêm hàm để chuyển tiền từ 1 người này sang người khác
    2. Thêm hàm để lấy thông tin của 1 transaction
III. Notification
    1. Thêm hàm để chủ động gửi thông báo cho người dùng khi thỏa mãn điều kiện nào đó




 */