using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;
using pbms_be.DataAccess;

namespace pbms_be.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class AATestingController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public AATestingController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // get all account
        [HttpGet("getAllAccount")]
        public IActionResult GetAllAccount()
        {
            var authDA = new AuthDA(_context);
            var accounts = authDA.GetAllAccount();
            return Ok(accounts);
        }
    }
}
