using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;

namespace pbms_be.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public DashboardController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // get wallet balance log
        //[HttpGet("get/walletbalancelog/{accountID}")]
    }
}
