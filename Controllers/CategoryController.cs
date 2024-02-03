using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;
using pbms_be.DataAccess;

namespace pbms_be.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public CategoryController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // get all category by account id
        [HttpGet("get/account/{accountID}")]
        public IActionResult GetCategories(string accountID)
        {
            try
            {
                if (string.IsNullOrEmpty(accountID)) return BadRequest("AccountID is required");
                CategoryDA categoryDA = new CategoryDA(_context);
                var result = categoryDA.GetCategories(accountID);
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
