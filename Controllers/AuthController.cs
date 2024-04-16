using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using pbms_be.Data;
using System.Security.Claims;
using pbms_be.Data.Auth;
using pbms_be.DataAccess;
using AutoMapper;
using pbms_be.DTOs;
using System;
using pbms_be.Configurations;

namespace pbms_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PbmsDbContext _context;
        private readonly IMapper? _mapper;
        private AuthDA _authDA;

        public AuthController(PbmsDbContext context, IMapper? mapper)
        {
            _context = context;
            _mapper = mapper;
            _authDA = new AuthDA(_context);
        }

        // POST JWT: api/<AuthController>
        [HttpPost("signin")]
        public IActionResult SigninByJWT([FromBody] string jwt)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);
                // check jwt is valid or not
                if (token is null) return BadRequest("Invalid JWT");
                var result = _authDA.SigninByJWT(jwt);
                return Ok(result);
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/<AuthController>
        // update account info
       
    }
}
