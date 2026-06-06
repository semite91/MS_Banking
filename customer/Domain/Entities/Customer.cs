using System;
using System.Collections.Generic;

namespace Customer.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime? BirthDate { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<Address> _addresses = new();
        public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

        private Customer() { }

        public Customer(Guid id, string firstName, string lastName, DateTime? birthDate, string email, string phone)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            BirthDate = birthDate;
            Email = email;
            Phone = phone;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddAddress(Address address)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));
            _addresses.Add(address);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContact(string email, string phone)
        {
            Email = email;
            Phone = phone;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
