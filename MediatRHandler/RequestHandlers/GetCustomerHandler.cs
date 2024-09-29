using MediatR;
using MediatRHandler.Entities;
using MediatRHandler.Repositories;
using MediatRHandler.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatRHandler.RequestHandlers
{
    public  class GetCustomerHandler : IRequestHandler<GetCustomerRequest, Customer?>
    {
        private readonly ICustomerRepository _customerRepository;
        public GetCustomerHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<Customer?> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
        {
           return await _customerRepository.GetCustomer(request.CustomerId);
        }
    }
}
