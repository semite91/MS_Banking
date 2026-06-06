using System;

namespace Customer.Domain.Entities
{
    public class Address
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public string Type { get; private set; }
        public string Line1 { get; private set; }
        public string? Line2 { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string PostalCode { get; private set; }

        private Address() { }

        public Address(Guid customerId, string type, string line1, string city, string state, string country, string postalCode, string? line2 = null)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            Type = type ?? "Home";
            Line1 = line1 ?? throw new ArgumentNullException(nameof(line1));
            Line2 = line2;
            City = city ?? throw new ArgumentNullException(nameof(city));
            State = state ?? throw new ArgumentNullException(nameof(state));
            Country = country ?? throw new ArgumentNullException(nameof(country));
            PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        }
    }
}
