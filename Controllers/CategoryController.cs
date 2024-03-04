using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Configurations;
using pbms_be.Data;
using pbms_be.Data.Filter;
using pbms_be.DataAccess;
using pbms_be.DTOs;

namespace pbms_be.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private readonly CategoryDA _categoryDA;

        public CategoryController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _categoryDA = new CategoryDA(context);
        }

        // get all category by account id
        [HttpGet("get/{accountID}")]
        public IActionResult GetCategories(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                var result = _categoryDA.GetCategories(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get default categories
        [HttpGet("get/default")]
        public IActionResult GetDefaultCategories()
        {
            try
            {
                var result = _categoryDA.GetDefaultCategories();
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // get all category type
        [HttpGet("get/categorytype")]
        public IActionResult GetCategoryType()
        {
            try
            {
                var result = _categoryDA.GetCategoryTypes();
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //// get all category by account id
        //[HttpGet("getall/{accountID}")]
        //public IActionResult GetAllCategories(string accountID)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
        //        var result = _categoryDA.GetAllCategories(accountID);
        //        return Ok(result);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        //// generate default categories for an account
        //[HttpPost("generate/default")]
        //public IActionResult GenerateDefaultCategories(string accountID)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
        //        var result = _categoryDA.GenerateDefaultCategories(accountID);
        //        return Ok(result);
        //    }
        //    catch (System.Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        // create category
        [HttpPost("create")]
        public IActionResult CreateCategory([FromBody] CategoryCreateDTO categoryDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var category = _mapper.Map<Category>(categoryDTO);
                if (_categoryDA.IsCategoryExist(category)) return BadRequest(Message.CATEGORY_ALREADY_EXIST);
                var result = _categoryDA.CreateCategory(category);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // update category
        [HttpPut("update")]
        public IActionResult UpdateCategory([FromBody] CategoryUpdateDTO categoryDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (_mapper is null) return BadRequest(Message.MAPPER_IS_NULL);
                var category = _mapper.Map<Category>(categoryDTO);
                if (_categoryDA.GetCategory(category.CategoryID, category.AccountID) is null) return BadRequest(Message.CATEGORY_NOT_FOUND);
                if (_categoryDA.IsCategoryExist(category)) return BadRequest(Message.CATEGORY_ALREADY_EXIST);
                var result = _categoryDA.UpdateCategory(category);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // delete category
        [HttpDelete("delete/{categoryID}/{accountID}")]
        public IActionResult DeleteCategory(int categoryID, string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest(Message.ACCOUNT_ID_REQUIRED);
                if (_categoryDA.GetCategory(categoryID, accountID) is null) return BadRequest(Message.CATEGORY_NOT_FOUND);
                var result = _categoryDA.DeleteCategory(categoryID, accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
