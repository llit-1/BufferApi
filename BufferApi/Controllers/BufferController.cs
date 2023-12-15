using BufferApi.Buffer;
using BufferApi.DB.RKNET;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace BufferApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BufferController : ControllerBase
    {
        CalculatorLogRepository CalculatorLogRepository { get; set; }
        public BufferController(CalculatorLogRepository calculatorLogRepository)
        {
            CalculatorLogRepository = calculatorLogRepository;
        }

        [HttpPost(Name = "StartBufferJob")]
        public IActionResult Post([FromBody]CalculatorLogsTest calculatorLog)
        {
            CalculatorLogRepository.Add(calculatorLog, DateTime.Now, DateTime.Now.AddDays(1));
            return Ok();
        }

        [HttpGet]
        public int Get()
        {          
            return CalculatorLogRepository.RepositoryItems.Count;
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            CalculatorLogRepository.RepositoryItems.Clear();
            return Ok();
        }

    }
}