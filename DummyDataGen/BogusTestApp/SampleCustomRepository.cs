using Bogus;
using System;
using System.Collections.Generic;

namespace BogusTestApp
{
    public class SampleCustomRepository
    {
        public IEnumerable<Customer> GetCustomers()
        {
            Randomizer.Seed = new Random(123456);
            var genOrder = new Faker<Order>()
                .RuleFor(o => o.Id, Guid.NewGuid)
                .RuleFor(o => o.Date, f => f.Date.Past(1))
                .RuleFor(o => o.OrderValue, f => f.Finance.Amount(10, 10000))
                .RuleFor(o => o.Shipped, f => f.Random.Bool(0.9f));

            var getCustomer = new Faker<Customer>()
                .RuleFor(o => o.Id, Guid.NewGuid)
                .RuleFor(o => o.Name, f => f.Company.CompanyName())
                .RuleFor(o => o.Address, f => f.Address.StreetAddress())
                .RuleFor(o => o.Phone, f => f.Phone.PhoneNumber("010-####-####"))
                .RuleFor(o => o.ContactName, f => f.Name.FullName())
                .RuleFor(o => o.Orders, f => genOrder.Generate(f.Random.Number(1, 2)));

            return getCustomer.Generate(1);
        }
    }
}
