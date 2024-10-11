using MediatR;
using MediatRHandler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatRHandler.Requests
{
    public class GetCustomerRequest : IRequest<Customer?>
    {
        public Ulid CustomerId { get; set; }
    }
}
