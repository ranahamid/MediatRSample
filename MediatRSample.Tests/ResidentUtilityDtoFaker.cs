using MediatRSample.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
namespace MediatRSample.Tests
{
    public class ResidentUtilityDtoFaker : Faker<ResidentUtilityDto>
    {
        public ResidentUtilityDtoFaker(int customerId)
        {
            RuleFor(u => u.ResidentId, f => customerId);
            RuleFor(u => u.ElectricityBalance, f => f.Random.Double());
            RuleFor(u => u.WaterBalance, f => f.Random.Double());
            RuleFor(u => u.TrashBalance, f => f.Random.Double());
            RuleFor(u => u.TotalPastDueBalance, f => f.Random.Double());
            RuleFor(u => u.PeriodStart, f => f.Date.Past());
            RuleFor(u => u.PeriodEnd, f => f.Date.Past());
        }
    }
}