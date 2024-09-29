using MediatR;
using MediatRHandler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatRHandler.Requests
{
    public class CreateCustomerRequest : IRequest<int>
    {
        public Customer Customer { get; set; }
    }
}
