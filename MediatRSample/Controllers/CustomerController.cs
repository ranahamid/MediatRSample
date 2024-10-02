using MediatR;
using MediatRHandler.Entities;
using MediatRHandler.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using BenchmarkDotNet.Running;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Tags = new[] { "Customer" })]
        public async Task<Customer?> GetCustomer(int id)
        {
            //Benchmarking C# classes
           // var summary = BenchmarkRunner.Run<StringUtilityHelperBenchmark>();
            
            var customer = await _mediator.Send(new GetCustomerRequest { CustomerId = id });
            return customer;
        }
        [HttpPost] 
        [RequestTimeout(milliseconds: 1000)]
        [EnableRateLimiting("Token")]
        [SwaggerOperation(Tags = new[] { "Customer" })]
        public async Task<object> CreateCustomer(/*[FromBody]*/ Customer request)
        { 
            if(request == null)
            {
                throw new System.ArgumentNullException(); 
            }
            var customerId = await _mediator.Send(new CreateCustomerRequest
            {
                Customer = request
            });
            return customerId;
        }

    }
}
