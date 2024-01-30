using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using pbms_be.Data;
using System.Net;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace pbms_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly PbmsDbContext _context;

        public ValuesController(PbmsDbContext context)
        {
            _context = context;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<Sample> Get()
        {
            //IEnumerable<Sample> result = _context.Sample;
            IEnumerable<Sample> result = _context.Sample.FromSqlRaw("SELECT * FROM sample");
            return result;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public Sample Get(int id)
        {
            var result = _context.Sample.Find(id);
            return result;
        }

        [HttpGet("getLocalPublicIP")]
        public string GetLocalPublicIP()
        {
            //string ip = "";
            //string hostName = Dns.GetHostName();
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            //foreach (IPAddress address in ipHostInfo.AddressList)
            //{
            //    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            //    {
            //        ip = address.ToString();
            //    }
            //}
            //return ip;

            var ip = new WebClient().DownloadString("http://icanhazip.com");
            return ip;
        }
    }
}