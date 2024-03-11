using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pbms_be.Data;

namespace pbms_be.Controllers
{
    [Route("api/datetime")]
    [ApiController]
    public class DateTimeController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;

        public DateTimeController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
        }

    }
}
