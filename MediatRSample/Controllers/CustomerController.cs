using MediatR;
using MediatRHandler.Entities;
using MediatRHandler.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MediatRSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        //[DisableRequestTimeout]
        [RequestTimeout(milliseconds: 1000)]
        [EnableRateLimiting("Sliding")]
        public async Task<Customer?> GetCustomer(int id)
        {
            int waitSeconds = 5;
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds), HttpContext.RequestAborted);

            var customer = await _mediator.Send(new GetCustomerRequest { CustomerId = id });
            return customer;
        }
        [HttpPost] 
        [RequestTimeout(milliseconds: 1000)]
        [EnableRateLimiting("Token")]
        public async Task<int> CreateCustomer(/*[FromBody]*/ Customer request)
        {
            int waitSeconds = 3;
            await Task.Delay(TimeSpan.FromSeconds(waitSeconds), HttpContext.RequestAborted);
            var customerId = await _mediator.Send(new CreateCustomerRequest
            {
                Customer = request
            });
            return customerId;
        }

    }
}
