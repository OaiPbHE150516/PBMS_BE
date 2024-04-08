using AutoMapper;
using Google.Api;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Budget;
using pbms_be.Data.CollabFund;
using pbms_be.Data.Filter;
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

        #region Get Methods

        // get all budget type 
        [HttpGet("get/budgettype")]
        public IActionResult GetBudgetType()
        {
            try
            {
                return Ok(_budgetDA.GetBudgetType());
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("get/all/{accountID}")]
        public IActionResult GetBudget(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(accountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);
                if ( _mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                //var listBudget = new List<BudgetWithCategoryDTO>();
                var result = _budgetDA.GetBudgets(accountID, _mapper);
                //foreach (var item in result)
                //{
                //    var categoriesResult = new List<Category>();
                //    var budget = new Budget();

                //    //_budgetDA.GetBudgetDetail(accountID, item.BudgetID, out categoriesResult, out budget);
                //    var budgetDTO = _mapper.Map<BudgetWithCategoryDTO>(budget);
                //    budgetDTO.Categories = categoriesResult;
                //    listBudget.Add(budgetDTO);
                //}
                //return Ok(listBudget);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("get/detail/{budgetID}/{accountID}")]
        public IActionResult GetBudgetDetail(string accountID, int budgetID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (budgetID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.BUDGET_ID_REQUIRED);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(accountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);

                var categoriesResult = new List<Category>();
                var budget = new Budget();

                _budgetDA.GetBudgetDetail(accountID, budgetID, out categoriesResult, out budget);

                if (budget is null) return BadRequest(Message.BUDGET_NOT_FOUND);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var budgetDTO = _mapper.Map<BudgetWithCategoryDTO>(budget);
                budgetDTO.Categories = categoriesResult;

                return Ok(budgetDTO);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Post Methods

        // Create Budget
        [HttpPost("create")]
        public IActionResult CreateBudget([FromBody] CreateBudgetDTO budgetDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                // if budgetDTO.BudgetTypeID is not exist in budget type, return error
                if (!_budgetDA.IsBudgetTypeExist(budgetDTO.BudgetTypeID)) return BadRequest(Message.BUDGET_TYPE_NOT_FOUND);
                // check budget exist, if exist return error
                // check budget name exist, if exist return error
                var result = _budgetDA.CreateBudget(budgetDTO);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Put Methods
        
        // update budget

        #endregion

        #region Delete Methods

        // Delete Budget
        // delete a member from collab fund by collab fund id and account id, only fundholder can delete member
        [HttpDelete("delete/budget")]
        public IActionResult DeleteBudget([FromBody] DeleteBudgetDTO budget)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (string.IsNullOrEmpty(budget.AccountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (budget.BudgetID <= ConstantConfig.DEFAULT_ZERO_VALUE) return BadRequest(Message.BUDGET_ID_REQUIRED);
                var authDA = new AuthDA(_context);
                if (!authDA.IsAccountExist(budget.AccountID)) return BadRequest(Message.ACCOUNT_NOT_FOUND);
                if (!_budgetDA.IsBudgetExist(budget.AccountID, budget.BudgetID)) return BadRequest(Message.BUDGET_NOT_FOUND);
                var result = _budgetDA.DeleteBudget(budget, _mapper);
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
