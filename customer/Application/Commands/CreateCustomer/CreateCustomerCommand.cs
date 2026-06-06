using MediatR;
using System;

namespace Customer.Application.Commands.CreateCustomer
{
    public record CreateCustomerCommand(string FirstName, string LastName, DateTime? BirthDate, string Email, string Phone) : IRequest<Guid>;
}
