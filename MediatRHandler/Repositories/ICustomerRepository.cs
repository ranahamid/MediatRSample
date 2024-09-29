using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatRHandler.Entities;

namespace MediatRHandler.Repositories
{
    public interface ICustomerRepository
    {
        public Task<int> CreateCustomer(Customer customer);
        public Task<Customer?> GetCustomer(int customerId);
    }
}
