using AutoMapper;
using Google.Api;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Budget;
using pbms_be.Data.CollabFund;
using pbms_be.DataAccess;
using pbms_be.DTOs;

namespace pbms_be.Controllers
{
    [Route("api/budget/")]
    [ApiController]

    public class BudgetController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private readonly BudgetDA _budgetDA;
        public BudgetController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _budgetDA = new BudgetDA(context);
        }
        // get budget 

        [HttpGet("get/id/{budgetID}/{accountID}")]
        public IActionResult GetBudget(string accountID, int budgetID)
        {
            var result =_budgetDA.GetBudget(accountID, budgetID);
            return Ok(result);
        }
        [HttpGet("get/detail/{budgetID}/{accountID}")]
        public IActionResult GetBudgetDetail(string accountID, int budgetID)
        {
            // check cacs điều kiện 
            var result = _budgetDA.GetBudgetDetail(accountID, budgetID);
            return Ok(result);
        }
        // Create Budget
        [HttpPost("create")]
        public IActionResult CreateBudget([FromBody] CreateBudgetDTO budgetDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var budgetEntity = _mapper.Map<Budget>(budgetDTO);
                if (_budgetDA.IsBudgetExist(budgetEntity))
                    return BadRequest(Message.Budget_ALREADY_EXIST);
                var result = _budgetDA.CreateBudget(budgetEntity);
                
                return Ok(result);
                
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        // Delete Budget
        // delete a member from collab fund by collab fund id and account id, only fundholder can delete member
        [HttpDelete("delete/budget")]
        public IActionResult DeleteBudget([FromBody] Budget budget)
        {
            return Ok();
        }
    }

    

}
