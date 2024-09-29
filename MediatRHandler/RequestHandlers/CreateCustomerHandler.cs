using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MediatRHandler.Repositories;
using MediatRHandler.Requests;

namespace MediatRHandler.RequestHandlers
{
    public class CreateCustomerHandler:IRequestHandler<CreateCustomerRequest,int>
    {
        private readonly ICustomerRepository _customerRepository;
        public CreateCustomerHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async  Task<int> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            return await _customerRepository.CreateCustomer(request.Customer);

        }
    }
}
