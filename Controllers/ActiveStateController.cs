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

        public ActiveStateController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // get all active states
        [HttpGet("all")]
        public IActionResult GetActiveStates()
        {
            try
            {
                var result = _context.ActiveState.ToList();
                return Ok(result);
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
