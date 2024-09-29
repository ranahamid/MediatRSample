using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatRHandler.Entities;

namespace MediatRHandler.Repositories
{
    public  class CustomerRepository: ICustomerRepository
    {
        public async  Task<int> CreateCustomer(Customer customer)
        {
            return 1;
        }

        public async  Task<Customer?> GetCustomer(int customerId)
        {
            return new Customer 
            {
                FirstName= "John",
                LastName = "Doe",
                EmailAddress = "test@sls.com",
                Address = "1234567890"
            };
        }
    }
}
