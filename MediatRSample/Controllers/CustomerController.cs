using MediatR;
using MediatRHandler.Entities;
using MediatRHandler.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<Customer?> GetCustomer(int id)
        {
            var customer = await _mediator.Send(new GetCustomerRequest { CustomerId = id });
            return customer;
        }
        [HttpPost]
        public async Task<int> CreateCustomer(/*[FromBody]*/ Customer request)
        {
            var customerId = await _mediator.Send(new CreateCustomerRequest
            {
                Customer = request
            });
            return customerId;
        }

    }
}
