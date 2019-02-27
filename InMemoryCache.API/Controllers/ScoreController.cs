using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoryCache.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryCache.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IInMemoryCache _memory;

        public ScoreController(IInMemoryCache memory)
        {
            _memory = memory;
        }

        [HttpGet("{mykey}/ZCard")]
        public ActionResult<long> Get([FromRoute] string mykey)
        {
            return _memory.ZCard(mykey);
        }

        [HttpGet("{mykey}")]
        public ActionResult<long> Get([FromRoute] string mykey, [FromQuery] string value)
        {
            return _memory.ZRank(mykey, value);
        }

        [HttpGet("{mykey}/ZRange")]
        public ActionResult Get([FromRoute] string mykey, [FromQuery] int start, [FromQuery] int stop)
        {
            return Ok(string.Join("\n",
                _memory
                    .ZRange<string>(mykey, start, stop)
                    .Select((s, i) => $"{i + 1}) {s}")));
        }

        [HttpPost("{mykey}")]
        public async Task Post([FromRoute] string mykey, [FromQuery] long score)
        {
            var body = await GetBody();
            _memory.ZAdd(mykey, body, score);
        }       

        private async Task<string> GetBody()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.ASCII))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}