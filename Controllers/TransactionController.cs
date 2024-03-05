﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.DataAccess;
using pbms_be.DTOs;

namespace pbms_be.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private TransactionDA _transactionDA;

        public TransactionController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _transactionDA = new TransactionDA(_context);
        }

        // get all transaction by account id
        [HttpGet("get/{accountID}/{pageNumber}/{pageSize}")]
        public IActionResult GetTransactions(string accountID, int pageNumber, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (pageNumber <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.PAGE_NUMBER_REQUIRED);
                if (pageSize <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.PAGE_SIZE_REQUIRED);
                var result = _transactionDA.GetTransactions(accountID, pageNumber, pageSize, ConstantConfig.DESCENDING_SORT);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<List<TransactionInList_VM_DTO>>(result);
                // calculate total page
                var totalPage = _transactionDA.GetTotalPage(accountID, pageSize);
                return Ok(new { pageNumber, totalPage, pageSize, ConstantConfig.DESCENDING_SORT, resultDTO });
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get transaction by id
        [HttpGet("get/{transactionID}/{accountID}")]
        public IActionResult GetTransaction(int transactionID, string accountID)
        {
            try
            {
                if (transactionID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.TRANSACTION_ID_REQUIRED);
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _transactionDA.GetTransaction(transactionID, accountID);
                if (_mapper is null) throw new Exception(Message.MAPPER_IS_NULL);
                var resultDTO = _mapper.Map<TransactionDetail_VM_DTO>(result);
                return Ok(resultDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
