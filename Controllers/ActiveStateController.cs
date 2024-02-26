using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;

namespace pbms_be.Controllers
{
    [Route("api/activestate")]
    [ApiController]
    public class ActiveStateController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        // get all active states
        [HttpGet]
        public IActionResult GetActiveStates()
        {
            return Ok();
        }

    }
}
