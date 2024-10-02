using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MediatRSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilityController : ControllerBase
    {
        [HttpGet("randomnumber")]
        public int GetRandomNumber()
        {
            var randomizer = new Random();
            int randomNumber = randomizer.Next(0, 5);
            return randomNumber;
        }

        [HttpGet("randomnumberwithdelay")]
        public async Task<int> GetRandomNumberWithDelay()
        {
            var randomizer = new Random();
            int randomDelay = randomizer.Next(0, 5);

            await Task.Delay(TimeSpan.FromSeconds(randomDelay));

            return randomDelay;
        }

        [HttpGet("reverse")]
        public string Reverse([FromQuery] string input)
        {
            var reverse = new StringBuilder(input.Length);
            for (int i = input.Length - 1; i >= 0; i--)
            {
                reverse.Append(input[i]);
            }
            Thread.Sleep(500);
            return reverse.ToString();
        }
    }
}
