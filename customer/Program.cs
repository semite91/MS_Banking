using Customer.Application.Commands.CreateCustomer;
using Customer.Domain.Entities;
using Customer.Infrastructure.Persistence;
using Customer.Infrastructure.Repositories;
using Customer.Infrastructure.UnitOfWork;
using MediatR;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configuration / Connection
var conn = builder.Configuration.GetConnectionString("Postgres")
		   ?? Environment.GetEnvironmentVariable("CUSTOMER_CONNECTION")
		   ?? "Host=localhost;Database=Banking_User;Username=postgres;Password=!P1o1s1t1";

// Services
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CustomerDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddDbContext<CustomerSagaDbContext>(opts => opts.UseNpgsql(conn));
builder.Services.AddScoped<Shared.Infrastructure.Abstractions.IRepository<Customer.Domain.Entities.Customer>, CustomerRepository>();
builder.Services.AddScoped<Shared.Infrastructure.Abstractions.IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(typeof(CreateCustomerCommand).Assembly);

// MassTransit (publisher) registration
builder.Services.AddMassTransit(x =>
{
	x.AddSagaStateMachine<Customer.Infrastructure.Sagas.CustomerStateMachine, Customer.Infrastructure.Sagas.CustomerState>()
		.EntityFrameworkRepository(r =>
		{
			r.AddDbContext<CustomerSagaDbContext, CustomerSagaDbContext>((provider, builder) =>
			{
				builder.UseNpgsql(conn, sql => sql.MigrationsAssembly(typeof(CustomerSagaDbContext).Assembly.GetName().Name));
			});
		});
	x.UsingRabbitMq((context, cfg) =>
	{
		var host = builder.Configuration.GetValue<string>("RabbitMq:Host") ?? Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
		cfg.Host(host);
		cfg.ConfigureEndpoints(context);
	});
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

// Example endpoint that delegates to MediatR (no business logic in endpoint)
app.MapPost("/customers", async (CreateCustomerDto dto, IMediator mediator) =>
{
	var cmd = new CreateCustomerCommand(dto.FirstName, dto.LastName, dto.BirthDate, dto.Email, dto.Phone);
	var id = await mediator.Send(cmd);
	return Results.Created($"/customers/{id}", new { id });
});

app.MapGet("/", () => Results.Ok("Customer Service is running."));

app.UseSwagger();
app.UseSwaggerUI();

app.Run();

public record CreateCustomerDto(string FirstName, string LastName, DateTime? BirthDate, string Email, string Phone);
